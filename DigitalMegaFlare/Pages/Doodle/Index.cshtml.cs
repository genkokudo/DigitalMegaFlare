using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DigitalMegaFlare.Data;
using DigitalMegaFlare.Models;
using DigitalMegaFlare.Pages.SimpleGenerate.Razor;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using Mintea.HtmlToDom;
using RazorLight;
// TODO:Dictionaryを拡張して、存在したら追加、キーが存在しなかったらnew Listするメソッドと、new Dictionaryする拡張メソッドを作る。作れる？
namespace DigitalMegaFlare.Pages.Doodle
{
    /// <summary>
    /// 落書き帳
    /// </summary>
    public class IndexModel : PageModel
    {
        /// <summary>
        /// パス取得に使用する
        /// </summary>
        private readonly IWebHostEnvironment _hostEnvironment = null;
        public IndexModel(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public string Test { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var engine = new RazorLightEngineBuilder()
                          .UseEmbeddedResourcesProject(typeof(Program))
                          .UseMemoryCachingProvider()
                          .DisableEncoding()
                          .Build();

            // テンプレート読み込み
            // ファイルアクセス処理
            var fileDirectry = Path.Combine(_hostEnvironment.WebRootPath, "files", "razors", "asp");

            using (PhysicalFileProvider provider = new PhysicalFileProvider(fileDirectry))
            {
                // ファイル情報を取得
                IFileInfo fileInfo = provider.GetFileInfo("Model.dat");

                // ファイル存在チェック
                if (fileInfo.Exists)
                {
                    // Razorスクリプト読み込み
                    var template = System.IO.File.ReadAllText(fileInfo.PhysicalPath);

                    // Excelから読み込み
                    var excelDirectry = Path.Combine(_hostEnvironment.WebRootPath, "files", "excels");
                    var excel = ReadExcel(excelDirectry, "Model.xlsx");

                    // Modelの作成
                    var model = CreateModel(excel);

                    // 生成
                    string result = await engine.CompileRenderStringAsync("templateKey", template, model);

                    ViewData["Message"] = result;
                }
                else
                {
                    ViewData["Error"] = "ファイルが存在しません。";
                }
            }

            return Page();

        }

        /// <summary>
        /// 生成するシートの順番を作成する
        /// </summary>
        /// <param name="excel"></param>
        /// <param name="errors"></param>
        /// <returns>シートの順番が書かれたList</returns>
        private List<string> MakeSequence(Dictionary<string, List<List<string>>> excel, List<string> errors)
        {
            // 親リスト作成
            var parentList = MakeParentList(excel, errors);

            // 名前リスト作成の為の意味のないリスト
            var nameList = new Dictionary<string, string>();
            foreach (var item in excel)
            {
                nameList.Add(item.Key, item.Key);
            }

            // 木構造作成
            var tree = TreeNode<string>.MakeTree(new TreeNode<string>(string.Empty), nameList, parentList);

            // 深さ優先探索
            var result = new List<string>();
            var resultTreeList = tree.DepthList();

            // 名前を取り出す
            foreach (var item in resultTreeList)
            {
                if(item.Value != string.Empty)
                {
                    result.Add(item.Value);
                }
            }

            return result;
        }

        /// <summary>
        /// シート内のParent列を取得する
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private int GetParentIndex(List<List<string>> sheet)
        {
            var result = -1;

            if (sheet.Count > 2)
            {
                for (int i = 0; i < sheet[0].Count; i++)
                {
                    if (sheet[0][i] == "Parent")
                    {
                        return i;
                    }
                }
            }

            return result;
        }

        #region MakeParentList:親リスト作成
        private Dictionary<string, string> MakeParentList(Dictionary<string, List<List<string>>> excel, List<string> errors)
        {
            var parentList = new Dictionary<string, string>();

            foreach (var sheetName in excel.Keys)
            {
                // リスト
                var sheet = excel[sheetName];

                // Parentの列番号を取得
                var parentIndex = GetParentIndex(sheet);

                // Parentがある場合
                if (parentIndex >= 0)
                {
                    for (int i = 2; i < sheet.Count; i++)
                    {
                        if (!sheet[i][parentIndex].Contains("."))
                        {
                            errors.Add($"Parentに'.'が入ってない。sheet:{sheetName} row:{i} column:{parentIndex} value:{sheet[i][parentIndex]}");
                        }
                        else
                        {
                            // 子情報を登録する
                            var splited = sheet[i][parentIndex].Split('.');

                            if (parentList.Keys.Contains(sheetName))
                            {
                                if(parentList[sheetName] != splited[0])
                                {
                                    errors.Add($"同じParent列に違う親が書かれている。sheet:{sheetName} row:{i} column:{parentIndex} value:{sheet[i][parentIndex]}");
                                }
                            }
                            else
                            {
                                parentList.Add(sheetName, splited[0]);
                            }
                        }
                    }
                }
            }
            return parentList;
        }
        #endregion

        #region MakeChildData:子データ作成
        ///// <summary>
        ///// 子データを先に作っておきます
        ///// </summary>
        ///// <param name="excel"></param>
        ///// <param name="errors"></param>
        ///// <returns>0:親Sheet名、1:子Sheet名重複なし</returns>
        //private Dictionary<string, List<string>> MakeChildData(Dictionary<string, List<List<string>>> excel, List<string> errors)
        //{
        //    var childList = new Dictionary<string, List<string>>();

        //    foreach (var sheetName in excel.Keys)
        //    {
        //        if (sheetName.EndsWith("List"))
        //        {
        //            // リスト
        //            var sheet = excel[sheetName];

        //            if (sheet.Count > 2)
        //            {
        //                var parentIndex = -1;
        //                for (int i = 0; i < sheet[0].Count; i++)
        //                {
        //                    if (sheet[0][i] == "Parent")
        //                    {
        //                        parentIndex = i;
        //                        break;
        //                    }
        //                }
        //                // Parentがある場合
        //                if (parentIndex > 0)
        //                {
        //                    for (int i = 2; i < sheet.Count; i++)
        //                    {
        //                        if (!sheet[i][parentIndex].Contains("."))
        //                        {
        //                            errors.Add($"Parentに'.'が入ってない。sheet:{sheetName} row:{i} column:{parentIndex} value:{sheet[i][parentIndex]}");
        //                        }
        //                        else
        //                        {
        //                            // 子情報を登録する
        //                            var splited = sheet[i][parentIndex].Split('.');

        //                            if (!childList.Keys.Contains(splited[0]))
        //                            {
        //                                childList.Add(splited[0], new List<string>());
        //                            }
        //                            if (!childList[splited[0]].Contains(splited[1]))
        //                            {
        //                                childList[splited[0]].Add(splited[1]);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return childList;
        //}
        #endregion

        /// <summary>
        /// 子データを追加
        /// </summary>
        /// <param name="children">子データを格納する配列</param>
        /// <param name="parentName">親の名前</param>
        /// <param name="parentKey">親のキー</param>
        /// <param name="childSheetName">子の名前( = 親のフィールド名)</param>
        private void addChildrenData(Dictionary<string, Dictionary<string, Dictionary<string, dynamic>>> children, string parentName, string parentKey, string childSheetName, dynamic data)
        {
            if (!children.ContainsKey(parentName))
            {
                children.Add(parentName, new Dictionary<string, Dictionary<string, dynamic>>());
            }
            if (!children[parentName].ContainsKey(parentKey))
            {
                children[parentName].Add(parentKey, new Dictionary<string, dynamic>());
            }
            if (!children[parentName][parentKey].ContainsKey(childSheetName))
            {
                children[parentName][parentKey].Add(childSheetName, new Dictionary<string, dynamic>());
            }
            children[parentName][parentKey][childSheetName] = data;

        }

        /// <summary>
        /// Razorに入力するModelを作成する
        /// </summary>
        /// <returns></returns>
        private dynamic CreateModel(Dictionary<string, List<List<string>>> excel)
        {
            var errors = new List<string>();

            // データ作成順を決める
            var sequence = MakeSequence(excel, errors);

            // 各シートのデータ：キーはシート名
            var dataList = new Dictionary<string, dynamic>();

            // データ作成
            foreach (var sheetName in sequence)
            {
                // 1つのシート
                var sheet = excel[sheetName];

                // 子シートデータ
                var children = new Dictionary<string, Dictionary<string, Dictionary<string, dynamic>>>();
                
                if (sheetName == "Parse")
                {
                    // 組み立て情報の予定
                }
                else if (sheetName == "Outputs")
                {
                    // 吐き出し情報の予定
                }
                else if (sheetName.EndsWith("List"))
                {
                    // リスト
                    if (sheet.Count > 2)
                    {
                        // 親があるか
                        var parentIndex = GetParentIndex(sheet);
                        
                        // 親別、親が無い場合はシート全体のデータ（親キー、1行データ）
                        var dataByParent = new Dictionary<string, List<dynamic>>();

                        // 2行目まで読まない
                        var parentName = "";
                        for (int row = 2; row < sheet.Count; row++)
                        {
                            // 1行読む
                            var key = "-1";
                            var data = new Dictionary<string, object>();
                            for (int col = 0; col < sheet[row].Count; col++)
                            {
                                if(col == parentIndex)
                                {
                                    // 親参照はスクリプトデータから除外
                                    var split = sheet[row][col].Split(".");
                                    parentName = split[0];
                                    key = split[1];
                                }
                                else
                                {
                                    data.Add(sheet[0][col], sheet[row][col]);
                                }
                            }
                            // TODO:子を追加
                            // data.Add(子のシート名, children[sheetName][この行のキー][子のシート名]);   // 子情報いるじゃん

                            // 親Key別に追加
                            if (dataByParent.ContainsKey(key))
                            {
                                dataByParent.Add(key, new List<dynamic>());
                            }
                            dataByParent[key].Add(InputDynamic(data));  // 親キーの種類だけ件数入ってるよ
                        }

                        foreach (var dataByParentKey in dataByParent.Keys)
                        {
                            if (parentIndex >= 0)
                            {
                                // 親がある場合は、データを溜めておく
                                addChildrenData(children, parentName, dataByParentKey, sheetName, dataByParent[dataByParentKey]);
                            }
                            else
                            {
                                // 親が無いシートはトップにデータを入れる
                                dataList.Add(sheetName, dataByParent[dataByParentKey]);
                            }
                        }
                    }
                }
                else
                {
                    // 通常シート：1列目が名前、2列目が値
                    if (sheet.Count > 2)
                    {
                        var data = new Dictionary<string, object>();
                        // 2行目まで読まない
                        for (int row = 2; row < sheet.Count; row++)
                        {
                            var name = sheet[row][0];
                            var value = sheet[row][1];
                            data.Add(name, value);
                        }
                        // TODO:子を追加
                        dataList.Add(sheetName, InputDynamic(data));
                    }
                }
            }

            // TODO:組み立て
            dynamic project = InputDynamic(new Dictionary<string, object>() { { "Name", "DigitalMegaFlare" } });
            dynamic fieldListRow0 = InputDynamic(new Dictionary<string, object>() { { "Name", "Name" }, { "Comment", "名前" }, { "Attribute", "[StringLength(100)]" }, { "Type", "string" } });
            dynamic fieldListRow1 = InputDynamic(new Dictionary<string, object>() { { "Name", "Score" }, { "Comment", "点数" }, { "Attribute", "" }, { "Type", "int" } });
            dynamic fieldList = new List<dynamic> { fieldListRow0, fieldListRow1 };
            dynamic modelList = InputDynamic(new Dictionary<string, object>() { { "Name", "Test" }, { "Comment", "テストモデル" }, { "IsMaster", false }, { "FieldList", fieldList } });
            dynamic model = InputDynamic(new Dictionary<string, object>() { { "Project", project }, { "ModelList", modelList } });

            return model;
        }

        /// <summary>
        /// Excelを読み込む
        /// </summary>
        /// <param name="directry">ディレクトリ</param>
        /// <param name="filename">拡張子付きのファイル名</param>
        /// <returns></returns>
        private Dictionary<string, List<List<string>>> ReadExcel(string directry, string filename = "Model.xlsx")
        {
            // ファイルの読み込み
            var xlsx = new Dictionary<string, List<List<string>>>();
            using (PhysicalFileProvider provider = new PhysicalFileProvider(directry))
            {
                // ファイル情報を取得
                IFileInfo fileInfo = provider.GetFileInfo(filename);

                // ファイル存在チェック
                if (fileInfo.Exists)
                {
                    using (var wb = new XLWorkbook(fileInfo.PhysicalPath))
                    {
                        foreach (var ws in wb.Worksheets)
                        {
                            // ワークシート
                            List<List<string>> sheet = new List<List<string>>();

                            for (int i = 1; i <= ws.LastCellUsed().Address.RowNumber; i++)
                            {
                                List<string> raw = new List<string>();
                                for (int j = 1; j <= ws.LastCellUsed().Address.ColumnNumber; j++)
                                {
                                    raw.Add(ws.Cell(i, j).Value.ToString());
                                }
                                sheet.Add(raw);
                            }

                            // シート名と一緒に登録
                            xlsx.Add(ws.Name, sheet);
                        }
                    }
                }

                return xlsx;
            }
        }

        /// <summary>
        /// 動的にdynamic型を生成する
        /// </summary>
        /// <param name="Fields">フィールド名とそのオブジェクト(このメソッドで生成したdynamicでも良い)の組み合わせ</param>
        /// <returns></returns>
        private dynamic InputDynamic(Dictionary<string, object> Fields)
        {
            dynamic result = new ExpandoObject();
            IDictionary<string, object> work = result;
            foreach (var item in Fields) { work.Add(item.Key, item.Value); }

            return result;
        }
    }

}
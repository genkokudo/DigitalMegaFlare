using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalMegaFlare.Infrastructure
{
    /// <summary>
    /// Dictionary 型の拡張メソッドを管理するクラス
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// 指定したキーと値をディクショナリに追加します
        /// 指定したキーが既に格納されている場合は何もしません
        /// </summary>
        public static void AddIfNotExists<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key, TValue value)
        {
            TValue result;
            if (!self.TryGetValue(key, out result))
            {
                self.Add(key, value);
            }
        }

        /// <summary>
        /// 値を取得、keyがなければデフォルト値を設定し、デフォルト値を取得
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key">キー</param>
        /// <param name="defaultValue">値が存在しなかった場合の戻り値を指定、指定が無ければそのクラスのデフォルト値</param>
        /// <returns></returns>
        public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue defaultValue = default)
        {
            return dic.TryGetValue(key, out TValue result) ? result : defaultValue;
        }

        /// <summary>
        /// 動的にdynamic型を生成する
        /// </summary>
        /// <typeparam name="TKey">string</typeparam>
        /// <typeparam name="TValue">何でもよい</typeparam>
        /// <param name="fields">フィールド名とそのオブジェクトの組み合わせ</param>
        /// <returns>辞書のキーをフィールドとしたdynamic型データ</returns>
        public static dynamic ToDynamic<TKey, TValue>(this Dictionary<TKey, TValue> fields)
        {
            dynamic result = new ExpandoObject();
            IDictionary<TKey, TValue> work = result;
            foreach (var item in fields) { work.Add(item.Key, item.Value); }

            return result;
        }

        #region 拡張してみる
        public static void AddList<TPKey, TValue>(this Dictionary<TPKey, List<TValue>> dic, TPKey key, TValue value)
        {
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, new List<TValue>());
            }
            dic[key].Add(value);
        }

        public static void AddDictionary<TPKey, TKey, TValue>(this Dictionary<TPKey, Dictionary<TKey, TValue>> dic, TPKey key, TKey childKey, TValue value)
        {
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, new Dictionary<TKey, TValue>());
            }
            dic[key].Add(childKey, value);
        }
        #endregion
    }
}

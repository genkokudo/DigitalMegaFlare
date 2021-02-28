using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalMegaFlare.Models
{
    /// <summary>
    /// ユーザが作成したデータ
    /// </summary>
    public class UserData
    {
#pragma warning disable IDE0051, CS0169 // Remove unused private members
        /// <summary>
        /// ユーザID
        /// </summary>
        private string _userId;
#pragma warning restore IDE0051, CS0169 // Remove unused private members

        /// <summary>
        /// 論理削除フラグ
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}

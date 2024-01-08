using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.ViewModels
{
    /// <summary>
    /// 상품 정보 클래스
    /// </summary>
    public class ItemInfo
    {
        /// <summary>
        /// 상품 번호
        /// </summary>
        public Guid ItemNo { get; set; }
        /// <summary>
        /// 상품 명
        /// </summary>
        public string? ItemName { get; set; }

    }
}

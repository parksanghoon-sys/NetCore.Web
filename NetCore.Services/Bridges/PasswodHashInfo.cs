using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Bridges
{
    public class PasswodHashInfo
    {
        public string? GUIDSalt { get; set; }
        public string? RNGSalt { get; set; }
        public string? PasswodHash { get; set; }
    }
}

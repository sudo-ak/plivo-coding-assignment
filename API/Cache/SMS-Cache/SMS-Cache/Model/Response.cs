using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMSCache.Model
{
    public partial class Response
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
    }
}

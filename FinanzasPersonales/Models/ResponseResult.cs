using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinanzasPersonales.Models
{
    public class ResponseResult
    {
        public bool Success { get; set; }
        public object Data { get; set; }
    }
}
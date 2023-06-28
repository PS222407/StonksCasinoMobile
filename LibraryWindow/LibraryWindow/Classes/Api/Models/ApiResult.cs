using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryWindow.classes.Api.Models
{
    class ApiResult
    {
        [JsonProperty("result")]
        public bool Result { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotcakespicemanager
{
    public class Category
    {
        [JsonProperty("Bvin")]
        public string Bvin { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("ParentId")]
        public string ParentId { get; set; }

        [JsonProperty("RewriteUrl")]
        public string RewriteUrl { get; set; }
    }
    public class CategoryResponse
    {
        [JsonProperty("Content")]
        public List<Category> Content { get; set; }
    }
}

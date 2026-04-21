using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hotcakespicemanager
{
    public class Product
    {
        [JsonProperty("Bvin")]
        public string Bvin { get; set; }

        [JsonProperty("Sku")]
        public string Sku { get; set; }

        [JsonProperty("ProductName")]
        public string ProductName { get; set; }

        [JsonProperty("ListPrice")]
        public decimal ListPrice { get; set; }

        [JsonProperty("SitePrice")]
        public decimal SitePrice { get; set; }

        [JsonProperty("StoreId")]
        public int StoreId { get; set; }

        [JsonProperty("UrlSlug")]
        public string UrlSlug { get; set; }

        [JsonIgnore]
        public decimal? NewPrice { get; set; }
    }

    public class ProductContent
    {
        [JsonProperty("Products")]
        public List<Product> Products { get; set; }
    }

    public class ProductListResponse
    {
        [JsonProperty("Content")]
        public ProductContent Content { get; set; }
    }
}
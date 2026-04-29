using System;
using System.Collections.Generic;
using System.Linq;

namespace Hotcakespicemanager
{
    public static class PriceManagerLogic
    {
        public static decimal CalculateNewPrice(decimal oldPrice, decimal value, string mode)
        {
            if (mode == "%")
            {
                return oldPrice * (1 + value / 100m);
            }

            return oldPrice + value;
        }

        public static decimal RoundToNearest1000(decimal price)
        {
            return Math.Round(price / 1000m, 0) * 1000m;
        }

        public static List<Product> FilterProducts(
            IEnumerable<Product> products,
            decimal? minPrice,
            decimal? maxPrice,
            string skuPrefix)
        {
            IEnumerable<Product> query = products;

            query = query.Where(p => p.StoreId == 1);

            if (minPrice.HasValue)
                query = query.Where(p => p.SitePrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.SitePrice <= maxPrice.Value);

            if (!string.IsNullOrWhiteSpace(skuPrefix))
            {
                query = query.Where(p =>
                    !string.IsNullOrWhiteSpace(p.Sku) &&
                    p.Sku.StartsWith(skuPrefix));
            }

            return query.ToList();
        }

        public static string GetSkuPrefixFromCategoryName(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return null;

            string name = categoryName.ToLower();

            if (name.Contains("elektromos eszköz") || name.Contains("elektromos eszkoz"))
                return "ID 1";

            if (name.Contains("kiegészítő") || name.Contains("kiegeszito"))
                return "ID 2";

            if (name.Contains("alkatrész") || name.Contains("alkatresz"))
                return "ID 3";

            if (name.Contains("ruhak") || name.Contains("ruhák"))
                return "ID 4";

            if (name.Contains("roller"))
                return "ID 12";

            if (name.Contains("gördeszkák") || name.Contains("gordeszkak"))
                return "ID 13";

            if (name.Contains("hoverboard"))
                return "ID 14";

            if (name.Contains("csengő") || name.Contains("csengo"))
                return "ID 21";

            if (name.Contains("lámpa") || name.Contains("lampa"))
                return "ID 22";

            if (name.Contains("hordozó") || name.Contains("hordozo"))
                return "ID 23";

            if (name.Contains("kerékpár") || name.Contains("kerekpar"))
                return "ID 11";

            if (name.Contains("gumi"))
                return "ID 31";

            if (name.Contains("kerék") || name.Contains("kerek"))
                return "ID 32";

            if (name.Contains("fék") || name.Contains("fek"))
                return "ID 33";

            if (name.Contains("nyereg"))
                return "ID 34";

            if (name.Contains("akku") || name.Contains("akkumulátor"))
                return "ID 35";

            if (name.Contains("kesztyű") || name.Contains("kesztyu"))
                return "ID 41";

            if (name.Contains("mez"))
                return "ID 42";

            if (name.Contains("sisak"))
                return "ID 43";

            if (name.Contains("szemüveg") || name.Contains("szemuveg"))
                return "ID 44";

            if (name.Contains("cipő") || name.Contains("cipo"))
                return "ID 45";

            return null;
        }

        public static List<Product> GetProductsToModify(
            List<Product> filteredProducts,
            List<Product> selectedProducts,
            bool applyToAllFiltered)
        {
            if (applyToAllFiltered)
                return filteredProducts ?? new List<Product>();

            return selectedProducts ?? new List<Product>();
        }
    }
}
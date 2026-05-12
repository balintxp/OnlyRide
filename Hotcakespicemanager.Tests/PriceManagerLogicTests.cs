using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Hotcakespicemanager;

namespace Hotcakespicemanager.Tests
{
    [TestClass]
    public class PriceManagerLogicTests
    {
        // Azt teszteli, hogy százalékos módban helyesen növeli az árat.
        [TestMethod]
        public void CalculateNewPrice_PercentageIncrease_ReturnsCorrectValue()
        {
            var result = PriceManagerLogic.CalculateNewPrice(10000m, 10m, "%");

            Assert.AreEqual(11000m, result);
        }

        // Azt teszteli, hogy százalékos módban helyesen csökkenti az árat.
        [TestMethod]
        public void CalculateNewPrice_PercentageDecrease_ReturnsCorrectValue()
        {
            var result = PriceManagerLogic.CalculateNewPrice(10000m, -20m, "%");

            Assert.AreEqual(8000m, result);
        }

        // Azt teszteli, hogy fix összegű módban helyesen növeli az árat.
        [TestMethod]
        public void CalculateNewPrice_FixedIncrease_ReturnsCorrectValue()
        {
            var result = PriceManagerLogic.CalculateNewPrice(10000m, 2500m, "normál");

            Assert.AreEqual(12500m, result);
        }

        // Azt teszteli, hogy fix összegű módban helyesen csökkenti az árat.
        [TestMethod]
        public void CalculateNewPrice_FixedDecrease_ReturnsCorrectValue()
        {
            var result = PriceManagerLogic.CalculateNewPrice(10000m, -3000m, "normál");

            Assert.AreEqual(7000m, result);
        }

        // Azt teszteli, hogy az 1000 Ft-ra kerekítés helyesen működik.
        [TestMethod]
        public void RoundToNearest1000_ReturnsCorrectRoundedValue()
        {
            var result = PriceManagerLogic.RoundToNearest1000(1499m);

            Assert.AreEqual(1000m, result);
        }

        // Azt teszteli, hogy az árszűrés csak a megadott tartományban lévő termékeket hagyja meg.
        [TestMethod]
        public void FilterProducts_ByPriceRange_ReturnsOnlyProductsInRange()
        {
            var products = new List<Product>
            {
                new Product { ProductName = "A", SitePrice = 1000m, StoreId = 1, Sku = "ID 12-001" },
                new Product { ProductName = "B", SitePrice = 5000m, StoreId = 1, Sku = "ID 12-002" },
                new Product { ProductName = "C", SitePrice = 10000m, StoreId = 1, Sku = "ID 12-003" },
                new Product { ProductName = "D", SitePrice = 6000m, StoreId = 2, Sku = "ID 12-004" }
            };

            var result = PriceManagerLogic.FilterProducts(products, 2000m, 8000m, null);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("B", result[0].ProductName);
        }

        // Azt teszteli, hogy a kategórianév alapján a megfelelő SKU prefix kerül kiválasztásra.
        [TestMethod]
        public void GetSkuPrefixFromCategoryName_ReturnsCorrectPrefix()
        {
            var result = PriceManagerLogic.GetSkuPrefixFromCategoryName("Elektromos roller");

            Assert.AreEqual("ID 12", result);
        }

        // Azt teszteli, hogy a kategória szerinti szűrés csak a megfelelő prefixű termékeket adja vissza.
        [TestMethod]
        public void FilterProducts_ByCategoryPrefix_ReturnsOnlyMatchingProducts()
        {
            var products = new List<Product>
            {
                new Product { ProductName = "Roller A", SitePrice = 5000m, StoreId = 1, Sku = "ID 12-001" },
                new Product { ProductName = "Alkatrész B", SitePrice = 5000m, StoreId = 1, Sku = "ID 3-001" },
                new Product { ProductName = "Roller C", SitePrice = 7000m, StoreId = 1, Sku = "ID 12-002" }
            };

            var result = PriceManagerLogic.FilterProducts(products, null, null, "ID 12");

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("Roller A", result[0].ProductName);
            Assert.AreEqual("Roller C", result[1].ProductName);
        }

        // Azt teszteli, hogy a "minden szűrt termék" logika a szűrt listát adja vissza.
        [TestMethod]
        public void GetProductsToModify_WhenApplyToAllFilteredIsTrue_ReturnsFilteredProducts()
        {
            var filteredProducts = new List<Product>
            {
                new Product { ProductName = "A" },
                new Product { ProductName = "B" }
            };

            var selectedProducts = new List<Product>
            {
                new Product { ProductName = "X" }
            };

            var result = PriceManagerLogic.GetProductsToModify(filteredProducts, selectedProducts, true);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("A", result[0].ProductName);
            Assert.AreEqual("B", result[1].ProductName);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Linq;

namespace Hotcakespicemanager
{
    public partial class Form1 : Form
    {
        private decimal maxSitePrice = 0;
        private const string BASE_URL = "http://20.107.173.235/DesktopModules/Hotcakes/API/rest/v1/";
        private const string API_KEY = "1-5029c1c4-7e0e-40db-8814-17f68db1f619";

        private static readonly HttpClient httpClient = new HttpClient();
        private List<Product> allProducts = new List<Product>();
        private List<Product> filteredProducts = new List<Product>();
        private List<Category> allCategories = new List<Category>();


        public Form1()
        {
            InitializeComponent();
            InitializeApi();
            InitializeGrid();
            _ = LoadCategoriesAsync();
        }
        private void InitializeApi()
        {
            httpClient.BaseAddress = new Uri(BASE_URL);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            httpClient.DefaultRequestHeaders.Add("X-ApiKey", API_KEY);
        }
        private void InitializeGrid()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = true;
            dataGridView1.ReadOnly = true;

            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Sku",
                HeaderText = "SKU",
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProductName",
                HeaderText = "Terméknév",
                Width = 350
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SitePrice",
                HeaderText = "Ár",
                Width = 100
            });
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await LoadProductsAsync();
        }
        private async Task LoadProductsAsync()
        {
            try
            {
                allProducts = await LoadAllProductsFast();
                ApplyFilter();

                if (allProducts.Any())
                {
                    maxSitePrice = allProducts.Max(p => p.SitePrice);

                    trackBar2.Minimum = 0;
                    trackBar2.Maximum = 1000;
                    trackBar2.Value = 0;

                    trackBar1.Minimum = 0;
                    trackBar1.Maximum = 1000;
                    trackBar1.Value = 1000;

                    textBox1.Text = "0";
                    textBox2.Text = Math.Round(maxSitePrice, 0).ToString();
                }
                else
                {
                    textBox1.Text = "0";
                    textBox2.Text = "0";
                }

                MessageBox.Show($"Betöltve: {filteredProducts.Count} termék");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {

            ApplyFilter();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (trackBar2.Value > trackBar1.Value)
                trackBar2.Value = trackBar1.Value;

            decimal selectedMin = (trackBar2.Value / 1000m) * maxSitePrice;
            textBox1.Text = Math.Round(selectedMin, 0).ToString();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (trackBar1.Value < trackBar2.Value)
                trackBar1.Value = trackBar2.Value;

            decimal selectedMax = (trackBar1.Value / 1000m) * maxSitePrice;
            textBox2.Text = Math.Round(selectedMax, 0).ToString();
        }

        private void ApplyFilter()
        {
            decimal? min = null;
            decimal? max = null;

            if (decimal.TryParse(textBox1.Text, out decimal minVal))
                min = minVal;

            if (decimal.TryParse(textBox2.Text, out decimal maxVal))
                max = maxVal;

            IEnumerable<Product> query = allProducts;
            query = query.Where(p => p.StoreId == 1);

            if (min.HasValue)
                query = query.Where(p => p.SitePrice >= min.Value);

            if (max.HasValue)
                query = query.Where(p => p.SitePrice <= max.Value);

            string selectedPrefix = GetSelectedSkuPrefix();

            if (!string.IsNullOrEmpty(selectedPrefix))
            {
                query = query.Where(p =>
                    !string.IsNullOrEmpty(p.Sku) &&
                    p.Sku.StartsWith(selectedPrefix)
                );
            }

            filteredProducts = query.ToList();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = filteredProducts;
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var endpoint = $"categories?key={API_KEY}";
                var response = await httpClient.GetAsync(endpoint);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Kategória hiba:\n" + json);
                    return;
                }

                var result = JsonConvert.DeserializeObject<CategoryResponse>(json);

                allCategories = result?.Content ?? new List<Category>();

                FillCategoryListBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba kategória betöltésnél:\n" + ex.Message);
            }
        }

        private void FillCategoryListBox()
        {
            listBox1.DataSource = null;

            var displayCategories = new List<Category>();

            displayCategories.Add(new Category
            {
                Bvin = "ALL",
                Name = "Minden termék",
                RewriteUrl = "all"
            });

            var uniqueCategories = allCategories
                .Where(c => !((c.Name ?? "").ToLower().Contains("szervíz")))
                .GroupBy(c => c.Name)
                .Select(g => g.First())
                .OrderBy(c => GetCategoryOrder(c))
                .ThenBy(c => c.Name)
                .ToList();

            displayCategories.AddRange(uniqueCategories);

            listBox1.DisplayMember = "Name";
            listBox1.ValueMember = "Bvin";
            listBox1.DataSource = displayCategories;

            listBox1.SelectedIndex = 0;
        }
        private async Task<List<Product>> LoadAllProductsFast(string categoryId = null)
        {
            string endpoint = $"products?key={API_KEY}";

            var response = await httpClient.GetAsync(endpoint);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show($"API hiba: {response.StatusCode}\n\n{json}");
                return new List<Product>();
            }

            var result = JsonConvert.DeserializeObject<ProductListResponse>(json);

            return result?.Content?.Products ?? new List<Product>();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private string GetSelectedSkuPrefix()
        {
            if (listBox1.SelectedItem is not Category selectedCategory)
                return null;

            if (selectedCategory.Bvin == "ALL")
                return null;

            string name = selectedCategory.Name.ToLower();

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

        private int GetCategoryOrder(Category c)
        {
            string name = (c.Name ?? "").ToLower();

            if (name.Contains("elektromos eszköz")) return 1;
            if (name.Contains("elektromos kerékpár")) return 2;
            if (name.Contains("elektromos roller")) return 3;
            if (name.Contains("elektromos gördeszkák")) return 4;
            if (name.Contains("hoverboard")) return 5;

            if (name.Contains("kiegészítő")) return 10;
            if (name.Contains("csengő")) return 11;
            if (name.Contains("lámpa")) return 12;
            if (name.Contains("hordozó")) return 13;

            if (name.Contains("alkatrész")) return 20;
            if (name.Contains("gumi")) return 21;
            if (name.Contains("kerék")) return 22;
            if (name.Contains("fék")) return 23;
            if (name.Contains("nyereg")) return 24;
            if (name.Contains("akkumlátor")) return 25;

            if (name.Contains("ruhák")) return 30;
            if (name.Contains("kesztyű")) return 31;
            if (name.Contains("mez")) return 32;
            if (name.Contains("sisak")) return 33;
            if (name.Contains("szemüveg")) return 34;
            if (name.Contains("cipő")) return 35;

            return 999;
        }


    }
}

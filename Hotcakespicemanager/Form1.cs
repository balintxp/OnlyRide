using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

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
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = true;
            dataGridView1.ReadOnly = true;
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
                var endpoint = $"products?key={API_KEY}";
                var response = await httpClient.GetAsync(endpoint);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Hiba a lekéréskor: {response.StatusCode}\n\n{json}");
                    return;
                }

                var result = JsonConvert.DeserializeObject<ProductListResponse>(json);

                allProducts = result?.Content?.Products ?? new List<Product>();
                filteredProducts = allProducts.ToList();

                if (allProducts.Any())
                {
                    maxSitePrice = allProducts.Max(p => p.SitePrice);
                    trackBar2.Minimum = 0;
                    trackBar2.Maximum = 1000;
                    trackBar2.Value = 0;
                    trackBar2.TickFrequency = 50;
                    trackBar2.SmallChange = 1;
                    trackBar2.LargeChange = 25;

                    trackBar1.Minimum = 0;
                    trackBar1.Maximum = 1000;
                    trackBar1.Value = 1000;
                    trackBar1.TickFrequency = 50;
                    trackBar1.SmallChange = 1;
                    trackBar1.LargeChange = 25;

                    textBox1.Text = "0";
                    textBox2.Text = Math.Round(maxSitePrice, 0).ToString();
                }


                dataGridView1.DataSource = null;
                dataGridView1.DataSource = filteredProducts;
                await LoadCategoriesAsync();

                MessageBox.Show($"Betöltve: {filteredProducts.Count} termék");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message);
            }
        }
       

        private void button2_Click(object sender, EventArgs e)
        {
            decimal? min = null;
            decimal? max = null;

            if (decimal.TryParse(textBox1.Text, out decimal minVal))
                min = minVal;

            if (decimal.TryParse(textBox2.Text, out decimal maxVal))
                max = maxVal;

            var query = allProducts.AsEnumerable();

            if (min.HasValue)
                query = query.Where(p => p.SitePrice >= min.Value);

            if (max.HasValue)
                query = query.Where(p => p.SitePrice <= max.Value);

            filteredProducts = query.ToList();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = filteredProducts;
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

            if (min.HasValue)
                query = query.Where(p => p.SitePrice >= min.Value);

            if (max.HasValue)
                query = query.Where(p => p.SitePrice <= max.Value);

            var selectedIds = listBox1.SelectedItems
                .Cast<Category>()
                .Select(c => c.RewriteUrl)
                .ToList();

            if (selectedIds.Any())
            {
                query = query.Where(p =>
                    selectedIds.Any(cat =>
                        (p.UrlSlug ?? "").ToLower().Contains(cat.ToLower())
                    )
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

            listBox1.DataSource = allCategories;
            listBox1.DisplayMember = "Name";   // amit látsz
            listBox1.ValueMember = "Bvin";     // ID
        }
    }
}

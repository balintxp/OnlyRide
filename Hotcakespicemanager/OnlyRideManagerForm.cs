using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Hotcakespicemanager
{
    public partial class OnlyRideManagerForm : Form
    {
        private decimal maxSitePrice = 0;
        private const string BASE_URL = "http://20.107.173.235/DesktopModules/Hotcakes/API/rest/v1/";
        private const string API_KEY = "1-5029c1c4-7e0e-40db-8814-17f68db1f619";

        private static readonly HttpClient httpClient = new HttpClient();
        private List<Product> allProducts = new List<Product>();
        private List<Product> filteredProducts = new List<Product>();
        private List<Category> allCategories = new List<Category>();
        private List<ServiceBooking> allBookings = new List<ServiceBooking>();


        public OnlyRideManagerForm()
        {
            InitializeComponent();
            InitializeApi();
            InitializeGrid();

            comboBox2.Items.Clear();
            comboBox2.Items.Add("%");
            comboBox2.Items.Add("normál");
            comboBox2.SelectedIndex = 0;

            this.StartPosition = FormStartPosition.CenterScreen;

            this.Size = new Size(1200, 750);

            this.MinimumSize = new Size(1200, 750);

            this.MaximumSize = new Size(1200, 750);

            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            this.MaximizeBox = false;

            _ = LoadCategoriesAsync();

            cmbType.SelectedIndexChanged += (s, e) => ApplyBookingFilters();
            cmbStatus.SelectedIndexChanged += (s, e) => ApplyBookingFilters();
            dtpDate.ValueChanged += (s, e) => ApplyBookingFilters();
            textBox4.TextChanged += (s, e) => ApplyBookingFilters();

            //kijeloels
            dataGridView1.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(76, 175, 80);

            dataGridView1.DefaultCellStyle.SelectionForeColor =
                Color.White;

            dataGridView1.RowHeadersVisible = false;

            dataGridView1.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            
            //fejlec
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(35, 35, 35);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionBackColor =
                Color.FromArgb(35, 35, 35);

            dataGridView1.ColumnHeadersDefaultCellStyle.SelectionForeColor =
                Color.White;

            //ar
            dataGridView1.Columns[2].DefaultCellStyle.Format = "N0";
            dataGridView1.Columns[3].DefaultCellStyle.Format = "N0";
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            trackBar1.TickStyle = TickStyle.None;
            trackBar2.TickStyle = TickStyle.None;



            checkBox1.FlatStyle = FlatStyle.Flat;
            checkBox2.FlatStyle = FlatStyle.Flat;

            checkBox1.Font = new Font("Segoe UI", 9);
            checkBox2.Font = new Font("Segoe UI", 9);
            //listbox kijeloles
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;

            listBox1.DrawItem += (s, e) =>
            {
                if (e.Index < 0) return;

                bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

                Color backColor = selected
                    ? Color.FromArgb(76, 175, 80)
                    : Color.White;

                Color textColor = selected
                    ? Color.White
                    : Color.Black;

                using (SolidBrush bg = new SolidBrush(backColor))
                using (SolidBrush fg = new SolidBrush(textColor))
                {
                    e.Graphics.FillRectangle(bg, e.Bounds);

                    var category = listBox1.Items[e.Index] as Category;
                    string text = category?.Name ?? listBox1.Items[e.Index].ToString();

                    e.Graphics.DrawString(
                        text,
                        e.Font,
                        fg,
                        e.Bounds);
                }

                e.DrawFocusRectangle();
            };
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

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "NewPrice",
                HeaderText = "Új ár",
                Width = 100
            });
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private async void button4_Click(object sender, EventArgs e)
        {
            ShowBookingsView();

            SetupBookingsGrid();

            var bookings = (await LoadBookingsAsync())
                .OrderByDescending(b => b.BookingId)
                .ToList();

            allBookings = bookings;

            FillBookingFilters();

            ApplyBookingFilters();
        }

        private void FillBookingFilters()
        {
            cmbType.Items.Clear();
            cmbType.Items.Add("Összes");

            cmbType.Items.AddRange(allBookings
                .Select(x => x.ServiceTypeName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToArray());

            cmbType.SelectedIndex = 0;


            cmbStatus.Items.Clear();
            cmbStatus.Items.Add("Összes");

            cmbStatus.Items.AddRange(allBookings
                .Select(x => x.Status)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToArray());

            cmbStatus.SelectedIndex = 0;

            dtpDate.Checked = false;
        }

        private void ApplyBookingFilters()
        {
            var filtered = allBookings.AsEnumerable();

            // Típus
            if (cmbType.SelectedIndex > 0)
            {
                filtered = filtered.Where(x =>
                    x.ServiceTypeName == cmbType.SelectedItem.ToString());
            }

            // Státusz
            if (cmbStatus.SelectedIndex > 0)
            {
                filtered = filtered.Where(x =>
                    x.Status == cmbStatus.SelectedItem.ToString());
            }

            // Dátum
            if (dtpDate.Checked)
            {
                filtered = filtered.Where(x =>
                    x.CreatedOnDate.Date == dtpDate.Value.Date);
            }

            // Megjegyzés
            if (!string.IsNullOrWhiteSpace(textBox4.Text))
            {
                string search = textBox4.Text.ToLower();

                filtered = filtered.Where(x =>
                    x.CustomNote != null &&
                    x.CustomNote.ToLower().Contains(search));
            }

            dataGridView1.DataSource = null;

            dataGridView1.DataSource = filtered
                .OrderByDescending(x => x.BookingId)
                .ToList();
        }

        private void ShowBookingsView()
        {
            listBox1.Visible = false;
            groupBox1.Visible = false; 

            panel3.Visible = false; 

            trackBar1.Visible = false;
            trackBar2.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            label2.Visible = false; 
            label3.Visible = false; 

            comboBox2.Visible = false;
            textBox3.Visible = false;
            checkBox1.Visible = false;
            checkBox2.Visible = false;
            button3.Visible = false;
            button2.Visible = false;

            label4.Visible = false;
            label6.Visible = false;

            cmbStatus.Visible = true;
            cmbType.Visible = true;
            textBox4.Visible = true;
            dtpDate.Visible = true;

            dataGridView1.Location = new Point(20, 170);
            dataGridView1.Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 140);
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            cmbStatus.Location = new Point(358, 145);
            cmbStatus.Size = new Size(145,10);
            cmbType.Location = new Point(192, 145);
            cmbType.Size = new Size(145, 10);
            textBox4.Location = new Point(1010, 145);
            textBox4.Size = new Size(145, 10);
            dtpDate.Location = new Point(518, 145);
            dtpDate.Size = new Size(145, 10);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            InitializeGrid();

            dataGridView1.DataSource = null;
            dataGridView1.DataSource = filteredProducts;
            ShowProductsView();
            await LoadProductsAsync();

        }
        private void ShowProductsView()
        {
            listBox1.Visible = true;
            groupBox1.Visible = true;

            panel1.Visible = true;

            trackBar1.Visible = true;
            trackBar2.Visible = true;
            textBox1.Visible = true;
            textBox2.Visible = true;
            label2.Visible = true;
            label3.Visible = true;

            comboBox2.Visible = true;
            textBox3.Visible = true;
            checkBox1.Visible = true;
            checkBox2.Visible = true;
            button3.Visible = true;
            button2.Visible = true;

            label4.Visible = true;
            label5.Visible = true;
            label6.Visible = true;

            cmbStatus.Visible = false;
            cmbType.Visible = false;
            textBox4.Visible = false;
            dtpDate.Visible = false;

            dataGridView1.Location = new Point(206, 132);
            dataGridView1.Size = new Size(822, 550);
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

        private async void button3_Click_1(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox3.Text, out decimal value))
            {
                MessageBox.Show("Adj meg egy érvényes számot az összeg mezőbe!");
                return;
            }

            List<Product> productsToModify;

            if (checkBox2.Checked)
            {
                productsToModify = filteredProducts;
            }
            else
            {
                productsToModify = dataGridView1.SelectedRows
                    .Cast<DataGridViewRow>()
                    .Select(r => r.DataBoundItem as Product)
                    .Where(p => p != null)
                    .ToList();
            }

            if (!productsToModify.Any())
            {
                MessageBox.Show("Nincs kiválasztott termék!");
                return;
            }

            foreach (var product in productsToModify)
            {
                decimal newPrice;

                if (comboBox2.SelectedItem?.ToString() == "%")
                {
                    newPrice = product.SitePrice * (1 + value / 100);
                }
                else
                {
                    newPrice = product.SitePrice + value;
                }

                if (checkBox1.Checked)
                {
                    newPrice = Math.Round(newPrice / 1000m, 0) * 1000m;
                }

                product.NewPrice = newPrice;
            }

            dataGridView1.Refresh();

            var confirm = MessageBox.Show(
                $"{productsToModify.Count} termék ára módosulni fog. Biztos folytatod?",
                "Megerősítés",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes)
                return;

            int success = 0;

            foreach (var product in productsToModify)
            {
                bool ok = await UpdateProductPriceAsync(product);

                if (ok)
                    success++;
            }

            MessageBox.Show($"{success} termék ára sikeresen frissítve.");
        }
        private async Task<bool> UpdateProductPriceAsync(Product product)
        {
            if (product.NewPrice == null)
                return false;

            string getEndpoint = $"products/{product.Bvin}?key={API_KEY}";

            var getResponse = await httpClient.GetAsync(getEndpoint);
            var getJson = await getResponse.Content.ReadAsStringAsync();

            if (!getResponse.IsSuccessStatusCode)
            {
                MessageBox.Show($"Nem sikerült lekérni:\n{product.Sku}\n\n{getJson}");
                return false;
            }

            var fullJson = JObject.Parse(getJson);
            var contentObject = fullJson["Content"] as JObject;

            if (contentObject == null)
            {
                MessageBox.Show($"Nincs Content objektum:\n{product.Sku}");
                return false;
            }

            contentObject["SitePrice"] = product.NewPrice.Value;

            string postEndpoint = $"products?key={API_KEY}";
            string postJson = contentObject.ToString();

            var content = new StringContent(postJson, Encoding.UTF8, "application/json");

            var postResponse = await httpClient.PostAsync(postEndpoint, content);
            var postResponseText = await postResponse.Content.ReadAsStringAsync();

            if (!postResponse.IsSuccessStatusCode)
            {
                MessageBox.Show($"Hiba a frissítéskor:\n{product.Sku}\n\n{postResponse.StatusCode}\n\n{postResponseText}");
                return false;
            }

            product.SitePrice = product.NewPrice.Value;
            return true;
        }

        private const string BOOKINGS_URL =
                "http://20.107.173.235/API/OnlyRide.Dnn.ServiceBooking/BookingApi/GetBookingsByServiceType?moduleId=454&format=json";
        private async Task<List<ServiceBooking>> LoadBookingsAsync()
        {
            var response = await httpClient.GetAsync(BOOKINGS_URL);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Foglalás API hiba:\n" + json);
                return new List<ServiceBooking>();
            }

            var serviceTypes = JsonConvert.DeserializeObject<List<ServiceTypeBookingResponse>>(json);

            var bookings = serviceTypes
                .SelectMany(service =>
                    (service.Bookings ?? new List<ServiceBooking>())
                    .Select(booking =>
                    {
                        booking.ServiceTypeName = service.ServiceTypeName;
                        return booking;
                    }))
                .ToList();

            return bookings;
        }

        private void SetupBookingsGrid()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Clear();

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "BookingId",
                HeaderText = "Foglalás ID",
                Width = 100
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ServiceTypeName",
                HeaderText = "Szerviz típusa",
                Width = 220
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Status",
                HeaderText = "Státusz",
                Width = 120
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CreatedOnDate",
                HeaderText = "Dátum",
                Width = 150,
                DefaultCellStyle = { Format = "yyyy.MM.dd HH:mm" }
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ActualPrice",
                HeaderText = "Ár",
                Width = 100,
                DefaultCellStyle = { Format = "N0" }
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ActualMinutes",
                HeaderText = "Perc",
                Width = 80
            });

            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CustomNote",
                HeaderText = "Megjegyzés",
                Width = 250
            });
        }
    }
}

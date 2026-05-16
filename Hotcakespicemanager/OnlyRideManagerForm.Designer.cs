namespace Hotcakespicemanager
{
    partial class OnlyRideManagerForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OnlyRideManagerForm));
            label2 = new Label();
            label3 = new Label();
            trackBar1 = new TrackBar();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            button1 = new Button();
            button2 = new Button();
            dataGridView1 = new DataGridView();
            button3 = new Button();
            comboBox2 = new ComboBox();
            textBox3 = new TextBox();
            label4 = new Label();
            label6 = new Label();
            trackBar2 = new TrackBar();
            listBox1 = new ListBox();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            panel1 = new Panel();
            panel2 = new Panel();
            label5 = new Label();
            pictureBox1 = new PictureBox();
            groupBox1 = new GroupBox();
            panel3 = new Panel();
            button4 = new Button();
            dtpDate = new DateTimePicker();
            cmbType = new ComboBox();
            cmbStatus = new ComboBox();
            textBox4 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar2).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(446, 81);
            label2.Name = "label2";
            label2.Size = new Size(41, 15);
            label2.TabIndex = 1;
            label2.Text = "Min ár";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(503, 81);
            label3.Name = "label3";
            label3.Size = new Size(42, 15);
            label3.TabIndex = 2;
            label3.Text = "Max ár";
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(560, 81);
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(104, 45);
            trackBar1.TabIndex = 3;
            trackBar1.Scroll += trackBar1_Scroll;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(446, 103);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(51, 23);
            textBox1.TabIndex = 6;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(503, 103);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(51, 23);
            textBox2.TabIndex = 7;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.BackColor = SystemColors.Control;
            button1.FlatStyle = FlatStyle.System;
            button1.Location = new Point(972, 81);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 8;
            button1.Text = "Termékek";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.BackColor = SystemColors.Control;
            button2.FlatStyle = FlatStyle.System;
            button2.Location = new Point(692, 81);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 9;
            button2.Text = "Szűrés";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.GridColor = Color.LightGray;
            dataGridView1.Location = new Point(206, 132);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(697, 441);
            dataGridView1.TabIndex = 10;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button3.Location = new Point(975, 550);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 11;
            button3.Text = "Alkalmaz";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click_1;
            // 
            // comboBox2
            // 
            comboBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(911, 165);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(139, 23);
            comboBox2.TabIndex = 12;
            // 
            // textBox3
            // 
            textBox3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox3.Location = new Point(911, 214);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(139, 23);
            textBox3.TabIndex = 14;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(959, 196);
            label4.Name = "label4";
            label4.Size = new Size(44, 15);
            label4.TabIndex = 15;
            label4.Text = "Összeg";
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new Point(943, 147);
            label6.Name = "label6";
            label6.Size = new Size(60, 15);
            label6.TabIndex = 17;
            label6.Text = "%/normál";
            // 
            // trackBar2
            // 
            trackBar2.Location = new Point(336, 81);
            trackBar2.Name = "trackBar2";
            trackBar2.Size = new Size(104, 45);
            trackBar2.TabIndex = 18;
            trackBar2.Scroll += trackBar2_Scroll;
            // 
            // listBox1
            // 
            listBox1.BackColor = Color.White;
            listBox1.BorderStyle = BorderStyle.None;
            listBox1.Dock = DockStyle.Fill;
            listBox1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 238);
            listBox1.ForeColor = Color.Black;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 17;
            listBox1.Location = new Point(3, 19);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(178, 404);
            listBox1.TabIndex = 19;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // checkBox1
            // 
            checkBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(909, 255);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(114, 19);
            checkBox1.TabIndex = 20;
            checkBox1.Text = "1000-re kerekítés";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(909, 280);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(138, 19);
            checkBox2.TabIndex = 21;
            checkBox2.Text = "minden szűrt termék ";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(35, 35, 35);
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(pictureBox1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1059, 65);
            panel1.TabIndex = 22;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel2.BackColor = Color.LimeGreen;
            panel2.Location = new Point(0, 58);
            panel2.Name = "panel2";
            panel2.Size = new Size(1059, 17);
            panel2.TabIndex = 24;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 26.25F, FontStyle.Bold, GraphicsUnit.Point, 238);
            label5.ForeColor = Color.White;
            label5.Location = new Point(381, 0);
            label5.Name = "label5";
            label5.Size = new Size(430, 47);
            label5.TabIndex = 1;
            label5.Text = "OnlyRide Admin Manager";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(-41, -16);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(252, 91);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(listBox1);
            groupBox1.Location = new Point(0, 123);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(184, 426);
            groupBox1.TabIndex = 23;
            groupBox1.TabStop = false;
            groupBox1.Text = "Kategóriák";
            // 
            // panel3
            // 
            panel3.BackColor = Color.FromArgb(35, 35, 35);
            panel3.Location = new Point(190, 132);
            panel3.Name = "panel3";
            panel3.Size = new Size(10, 416);
            panel3.TabIndex = 24;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button4.Location = new Point(882, 81);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 25;
            button4.Text = "Foglalások";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // dtpDate
            // 
            dtpDate.Format = DateTimePickerFormat.Short;
            dtpDate.Location = new Point(460, 554);
            dtpDate.Name = "dtpDate";
            dtpDate.ShowCheckBox = true;
            dtpDate.Size = new Size(127, 23);
            dtpDate.TabIndex = 28;
            dtpDate.Visible = false;
            // 
            // cmbType
            // 
            cmbType.FormattingEnabled = true;
            cmbType.Location = new Point(313, 554);
            cmbType.Name = "cmbType";
            cmbType.Size = new Size(127, 23);
            cmbType.TabIndex = 29;
            cmbType.Visible = false;
            // 
            // cmbStatus
            // 
            cmbStatus.FormattingEnabled = true;
            cmbStatus.Location = new Point(174, 554);
            cmbStatus.Name = "cmbStatus";
            cmbStatus.Size = new Size(127, 23);
            cmbStatus.TabIndex = 30;
            cmbStatus.Visible = false;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(26, 555);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(127, 23);
            textBox4.TabIndex = 30;
            textBox4.Visible = false;
            // 
            // OnlyRideManagerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1059, 585);
            Controls.Add(textBox4);
            Controls.Add(cmbType);
            Controls.Add(cmbStatus);
            Controls.Add(dtpDate);
            Controls.Add(button4);
            Controls.Add(panel3);
            Controls.Add(groupBox1);
            Controls.Add(panel1);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(trackBar2);
            Controls.Add(label6);
            Controls.Add(label4);
            Controls.Add(textBox3);
            Controls.Add(comboBox2);
            Controls.Add(button3);
            Controls.Add(dataGridView1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(trackBar1);
            Controls.Add(label3);
            Controls.Add(label2);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "OnlyRideManagerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "OnlyRide Admin Manager";
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar2).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label2;
        private Label label3;
        private TrackBar trackBar1;
        private TextBox textBox1;
        private TextBox textBox2;
        private Button button1;
        private Button button2;
        private DataGridView dataGridView1;
        private Button button3;
        private ComboBox comboBox2;
        private TextBox textBox3;
        private Label label4;
        private Label label6;
        private TrackBar trackBar2;
        private ListBox listBox1;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private Panel panel1;
        private PictureBox pictureBox1;
        private Label label5;
        private GroupBox groupBox1;
        private Panel panel2;
        private Panel panel3;
        private Button button4;
        private DateTimePicker dtpDate;
        private ComboBox cmbType;
        private ComboBox cmbStatus;
        private TextBox textBox4;
    }
}

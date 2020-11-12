namespace Транспорт2017
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cor_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cor_button = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.trafic_button = new System.Windows.Forms.Button();
            this.model_button = new System.Windows.Forms.Button();
            this.trafic_textBox = new System.Windows.Forms.TextBox();
            this.model_textBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.hours_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.wait_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.fullReport_checkBox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.have_radioButton = new System.Windows.Forms.RadioButton();
            this.all_radioButton = new System.Windows.Forms.RadioButton();
            this.save_button = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.pWait_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.pasReport_checkBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hours_numericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.wait_numericUpDown)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pWait_numericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // cor_textBox
            // 
            this.cor_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cor_textBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cor_textBox.Location = new System.Drawing.Point(12, 99);
            this.cor_textBox.Multiline = true;
            this.cor_textBox.Name = "cor_textBox";
            this.cor_textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.cor_textBox.Size = new System.Drawing.Size(443, 32);
            this.cor_textBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(9, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Файл с корреспонденциями остановок";
            // 
            // cor_button
            // 
            this.cor_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cor_button.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cor_button.Location = new System.Drawing.Point(461, 108);
            this.cor_button.Name = "cor_button";
            this.cor_button.Size = new System.Drawing.Size(26, 23);
            this.cor_button.TabIndex = 2;
            this.cor_button.Text = "...";
            this.cor_button.UseVisualStyleBackColor = true;
            this.cor_button.Click += new System.EventHandler(this._button_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.trafic_button);
            this.groupBox1.Controls.Add(this.model_button);
            this.groupBox1.Controls.Add(this.cor_button);
            this.groupBox1.Controls.Add(this.trafic_textBox);
            this.groupBox1.Controls.Add(this.model_textBox);
            this.groupBox1.Controls.Add(this.cor_textBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.ForeColor = System.Drawing.Color.Blue;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(496, 201);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Параметры исходных данных";
            // 
            // trafic_button
            // 
            this.trafic_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.trafic_button.ForeColor = System.Drawing.SystemColors.ControlText;
            this.trafic_button.Location = new System.Drawing.Point(461, 169);
            this.trafic_button.Name = "trafic_button";
            this.trafic_button.Size = new System.Drawing.Size(26, 23);
            this.trafic_button.TabIndex = 2;
            this.trafic_button.Text = "...";
            this.trafic_button.UseVisualStyleBackColor = true;
            this.trafic_button.Click += new System.EventHandler(this._button_Click);
            // 
            // model_button
            // 
            this.model_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.model_button.ForeColor = System.Drawing.SystemColors.ControlText;
            this.model_button.Location = new System.Drawing.Point(461, 49);
            this.model_button.Name = "model_button";
            this.model_button.Size = new System.Drawing.Size(26, 23);
            this.model_button.TabIndex = 2;
            this.model_button.Text = "...";
            this.model_button.UseVisualStyleBackColor = true;
            this.model_button.Click += new System.EventHandler(this._button_Click);
            // 
            // trafic_textBox
            // 
            this.trafic_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trafic_textBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.trafic_textBox.Location = new System.Drawing.Point(12, 160);
            this.trafic_textBox.Multiline = true;
            this.trafic_textBox.Name = "trafic_textBox";
            this.trafic_textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.trafic_textBox.Size = new System.Drawing.Size(443, 32);
            this.trafic_textBox.TabIndex = 0;
            // 
            // model_textBox
            // 
            this.model_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.model_textBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.model_textBox.Location = new System.Drawing.Point(12, 40);
            this.model_textBox.Multiline = true;
            this.model_textBox.Name = "model_textBox";
            this.model_textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.model_textBox.Size = new System.Drawing.Size(443, 32);
            this.model_textBox.TabIndex = 0;
            this.model_textBox.Text = "1\r\n2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(9, 144);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(164, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Файл с трафиком пассажиров";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label5.Location = new System.Drawing.Point(9, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(222, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Файл со списком маршрутов и остановок";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Файлы Excel (*.xlsx, *.xlsm)|*.xlsx;*.xlsm|Все файлы|*.*";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.Location = new System.Drawing.Point(9, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(154, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Число часов моделирования";
            // 
            // hours_numericUpDown
            // 
            this.hours_numericUpDown.ForeColor = System.Drawing.SystemColors.ControlText;
            this.hours_numericUpDown.Location = new System.Drawing.Point(169, 26);
            this.hours_numericUpDown.Name = "hours_numericUpDown";
            this.hours_numericUpDown.Size = new System.Drawing.Size(50, 20);
            this.hours_numericUpDown.TabIndex = 4;
            this.hours_numericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.hours_numericUpDown.Value = new decimal(new int[] {
            17,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.Location = new System.Drawing.Point(9, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(321, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Макс. время ожидания пассажира до ухода с остановки, мин";
            // 
            // wait_numericUpDown
            // 
            this.wait_numericUpDown.ForeColor = System.Drawing.SystemColors.ControlText;
            this.wait_numericUpDown.Location = new System.Drawing.Point(336, 52);
            this.wait_numericUpDown.Name = "wait_numericUpDown";
            this.wait_numericUpDown.Size = new System.Drawing.Size(50, 20);
            this.wait_numericUpDown.TabIndex = 4;
            this.wait_numericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.wait_numericUpDown.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // fullReport_checkBox
            // 
            this.fullReport_checkBox.AutoSize = true;
            this.fullReport_checkBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.fullReport_checkBox.Location = new System.Drawing.Point(12, 103);
            this.fullReport_checkBox.Name = "fullReport_checkBox";
            this.fullReport_checkBox.Size = new System.Drawing.Size(478, 17);
            this.fullReport_checkBox.TabIndex = 5;
            this.fullReport_checkBox.Text = "Генерация полного отчета (с листами \"авто_завершен\", \"авто_в_пути\", \"авто_таблица" +
    "\")";
            this.fullReport_checkBox.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.numericUpDown1);
            this.groupBox2.ForeColor = System.Drawing.Color.Blue;
            this.groupBox2.Location = new System.Drawing.Point(521, 29);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(274, 53);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Параметры для генерации трафика пассажиров";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label6.Location = new System.Drawing.Point(9, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(142, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Начальный час с данными";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.numericUpDown1.Location = new System.Drawing.Point(169, 25);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(50, 20);
            this.numericUpDown1.TabIndex = 4;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown1.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.have_radioButton);
            this.groupBox3.Controls.Add(this.all_radioButton);
            this.groupBox3.ForeColor = System.Drawing.Color.Blue;
            this.groupBox3.Location = new System.Drawing.Point(521, 107);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(274, 106);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Параметры для формирования матрицы достижимости";
            // 
            // have_radioButton
            // 
            this.have_radioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.have_radioButton.Location = new System.Drawing.Point(12, 62);
            this.have_radioButton.Name = "have_radioButton";
            this.have_radioButton.Size = new System.Drawing.Size(256, 36);
            this.have_radioButton.TabIndex = 5;
            this.have_radioButton.TabStop = true;
            this.have_radioButton.Text = "использовать только те маршруты, которые имеют рейсы";
            this.have_radioButton.UseVisualStyleBackColor = true;
            // 
            // all_radioButton
            // 
            this.all_radioButton.AutoSize = true;
            this.all_radioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.all_radioButton.Location = new System.Drawing.Point(12, 39);
            this.all_radioButton.Name = "all_radioButton";
            this.all_radioButton.Size = new System.Drawing.Size(172, 17);
            this.all_radioButton.TabIndex = 5;
            this.all_radioButton.TabStop = true;
            this.all_radioButton.Text = "использовать все маршруты";
            this.all_radioButton.UseVisualStyleBackColor = true;
            // 
            // save_button
            // 
            this.save_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.save_button.AutoSize = true;
            this.save_button.Image = global::Транспорт2017.Properties.Resources.FloppyDisk;
            this.save_button.Location = new System.Drawing.Point(634, 324);
            this.save_button.Name = "save_button";
            this.save_button.Size = new System.Drawing.Size(161, 46);
            this.save_button.TabIndex = 2;
            this.save_button.Text = "Сохранить настройки";
            this.save_button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.save_button.UseVisualStyleBackColor = true;
            this.save_button.Click += new System.EventHandler(this.save_button_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.hours_numericUpDown);
            this.groupBox4.Controls.Add(this.pasReport_checkBox);
            this.groupBox4.Controls.Add(this.fullReport_checkBox);
            this.groupBox4.Controls.Add(this.pWait_numericUpDown);
            this.groupBox4.Controls.Add(this.wait_numericUpDown);
            this.groupBox4.ForeColor = System.Drawing.Color.Blue;
            this.groupBox4.Location = new System.Drawing.Point(12, 219);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(496, 151);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Параметры моделирования ТС";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label7.Location = new System.Drawing.Point(9, 79);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(399, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Доля пассажиров общественного транспорта, предпочитающего маршрутки";
            // 
            // pWait_numericUpDown
            // 
            this.pWait_numericUpDown.DecimalPlaces = 2;
            this.pWait_numericUpDown.ForeColor = System.Drawing.SystemColors.ControlText;
            this.pWait_numericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.pWait_numericUpDown.Location = new System.Drawing.Point(414, 77);
            this.pWait_numericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.pWait_numericUpDown.Name = "pWait_numericUpDown";
            this.pWait_numericUpDown.Size = new System.Drawing.Size(50, 20);
            this.pWait_numericUpDown.TabIndex = 4;
            this.pWait_numericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.pWait_numericUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            // 
            // pasReport_checkBox
            // 
            this.pasReport_checkBox.AutoSize = true;
            this.pasReport_checkBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.pasReport_checkBox.Location = new System.Drawing.Point(12, 126);
            this.pasReport_checkBox.Name = "pasReport_checkBox";
            this.pasReport_checkBox.Size = new System.Drawing.Size(379, 17);
            this.pasReport_checkBox.TabIndex = 5;
            this.pasReport_checkBox.Text = "Генерация данных по каждому пассажиру в отчет отчета (лист \"пас\")";
            this.pasReport_checkBox.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 382);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.save_button);
            this.Controls.Add(this.groupBox1);
            this.Name = "SettingsForm";
            this.Text = "Настройки модели";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hours_numericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.wait_numericUpDown)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pWait_numericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox cor_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cor_button;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button trafic_button;
        private System.Windows.Forms.TextBox trafic_textBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown hours_numericUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown wait_numericUpDown;
        private System.Windows.Forms.Button save_button;
        private System.Windows.Forms.Button model_button;
        private System.Windows.Forms.TextBox model_textBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox fullReport_checkBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton have_radioButton;
        private System.Windows.Forms.RadioButton all_radioButton;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown pWait_numericUpDown;
        private System.Windows.Forms.CheckBox pasReport_checkBox;
    }
}
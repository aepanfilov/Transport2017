namespace Транспорт2017
{
    partial class MainForm
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
            this.TabPage5 = new System.Windows.Forms.TabPage();
            this.openRes_button = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.allSolve_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.settings_toolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.matrCorr_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trafPas_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.structMarsh_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TabPage5.SuspendLayout();
            this.TabControl1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabPage5
            // 
            this.TabPage5.Controls.Add(this.openRes_button);
            this.TabPage5.Controls.Add(this.listBox1);
            this.TabPage5.Location = new System.Drawing.Point(4, 24);
            this.TabPage5.Name = "TabPage5";
            this.TabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage5.Size = new System.Drawing.Size(815, 379);
            this.TabPage5.TabIndex = 4;
            this.TabPage5.Text = "Общие результаты";
            this.TabPage5.UseVisualStyleBackColor = true;
            // 
            // openRes_button
            // 
            this.openRes_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.openRes_button.AutoSize = true;
            this.openRes_button.BackColor = System.Drawing.Color.PaleGreen;
            this.openRes_button.Image = global::Транспорт2017.Properties.Resources.excel;
            this.openRes_button.Location = new System.Drawing.Point(639, 22);
            this.openRes_button.Name = "openRes_button";
            this.openRes_button.Size = new System.Drawing.Size(150, 63);
            this.openRes_button.TabIndex = 7;
            this.openRes_button.Text = "Открыть отчет в Excel";
            this.openRes_button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.openRes_button.UseVisualStyleBackColor = false;
            this.openRes_button.Click += new System.EventHandler(this.OpenRes_button_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(6, 6);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(609, 364);
            this.listBox1.TabIndex = 0;
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.TabPage5);
            this.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl1.Location = new System.Drawing.Point(0, 54);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(823, 407);
            this.TabControl1.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton2,
            this.toolStripSeparator1,
            this.allSolve_toolStripButton,
            this.settings_toolStripButton,
            this.toolStripSeparator3,
            this.toolStripSplitButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(823, 54);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = global::Транспорт2017.Properties.Resources.Users;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(139, 51);
            this.toolStripButton2.Text = "Генерация пассажиров";
            this.toolStripButton2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton2.ToolTipText = "Генерация пассажиропотока";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // allSolve_toolStripButton
            // 
            this.allSolve_toolStripButton.Image = global::Транспорт2017.Properties.Resources.Wizard1;
            this.allSolve_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.allSolve_toolStripButton.Name = "allSolve_toolStripButton";
            this.allSolve_toolStripButton.Size = new System.Drawing.Size(97, 51);
            this.allSolve_toolStripButton.Text = "Полный расчет";
            this.allSolve_toolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.allSolve_toolStripButton.Click += new System.EventHandler(this.AllSolve_toolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 54);
            // 
            // settings_toolStripButton
            // 
            this.settings_toolStripButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.settings_toolStripButton.Image = global::Транспорт2017.Properties.Resources.tools;
            this.settings_toolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.settings_toolStripButton.Name = "settings_toolStripButton";
            this.settings_toolStripButton.Size = new System.Drawing.Size(71, 51);
            this.settings_toolStripButton.Text = "Настройки";
            this.settings_toolStripButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.settings_toolStripButton.Click += new System.EventHandler(this.Settings_toolStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 54);
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.matrCorr_ToolStripMenuItem,
            this.trafPas_ToolStripMenuItem,
            this.structMarsh_ToolStripMenuItem});
            this.toolStripSplitButton1.Image = global::Транспорт2017.Properties.Resources.EntityDataModel_NewEntityModelService;
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.White;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(97, 51);
            this.toolStripSplitButton1.Text = "Доп. функции";
            this.toolStripSplitButton1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // matrCorr_ToolStripMenuItem
            // 
            this.matrCorr_ToolStripMenuItem.Image = global::Транспорт2017.Properties.Resources.base_map;
            this.matrCorr_ToolStripMenuItem.Name = "matrCorr_ToolStripMenuItem";
            this.matrCorr_ToolStripMenuItem.Size = new System.Drawing.Size(318, 38);
            this.matrCorr_ToolStripMenuItem.Text = "Формирование матрицы достижимостей";
            this.matrCorr_ToolStripMenuItem.Click += new System.EventHandler(this.MatrCorr_ToolStripMenuItem_Click);
            // 
            // trafPas_ToolStripMenuItem
            // 
            this.trafPas_ToolStripMenuItem.Image = global::Транспорт2017.Properties.Resources.Users;
            this.trafPas_ToolStripMenuItem.Name = "trafPas_ToolStripMenuItem";
            this.trafPas_ToolStripMenuItem.Size = new System.Drawing.Size(318, 38);
            this.trafPas_ToolStripMenuItem.Text = "Подготовка трафика пассажиров";
            this.trafPas_ToolStripMenuItem.Click += new System.EventHandler(this.TrafPas_ToolStripMenuItem_Click);
            // 
            // structMarsh_ToolStripMenuItem
            // 
            this.structMarsh_ToolStripMenuItem.Image = global::Транспорт2017.Properties.Resources.AutoList;
            this.structMarsh_ToolStripMenuItem.Name = "structMarsh_ToolStripMenuItem";
            this.structMarsh_ToolStripMenuItem.Size = new System.Drawing.Size(318, 38);
            this.structMarsh_ToolStripMenuItem.Text = "Определить структуру всех маршрутов";
            this.structMarsh_ToolStripMenuItem.Click += new System.EventHandler(this.StructMarsh_ToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(823, 461);
            this.Controls.Add(this.TabControl1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "MainForm";
            this.Text = "Модель транспортной сети";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.TabPage5.ResumeLayout(false);
            this.TabPage5.PerformLayout();
            this.TabControl1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TabPage TabPage5;
        internal System.Windows.Forms.TabControl TabControl1;
        private System.Windows.Forms.ToolStripButton settings_toolStripButton;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ListBox listBox1;
        internal System.Windows.Forms.Button openRes_button;
        private System.Windows.Forms.ToolStripButton allSolve_toolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripDropDownButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem matrCorr_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trafPas_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem structMarsh_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
    }
}
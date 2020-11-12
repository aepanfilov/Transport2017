namespace Транспорт2017.UI_Forms
{
    partial class StopForm
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
            this.OstDataGridView = new System.Windows.Forms.DataGridView();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.OstDataGridView)).BeginInit();
            this.GroupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // OstDataGridView
            // 
            this.OstDataGridView.AllowUserToAddRows = false;
            this.OstDataGridView.AllowUserToDeleteRows = false;
            this.OstDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.OstDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.OstDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OstDataGridView.Location = new System.Drawing.Point(3, 16);
            this.OstDataGridView.Name = "OstDataGridView";
            this.OstDataGridView.ReadOnly = true;
            this.OstDataGridView.Size = new System.Drawing.Size(824, 456);
            this.OstDataGridView.TabIndex = 14;
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.OstDataGridView);
            this.GroupBox3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.GroupBox3.Location = new System.Drawing.Point(0, 40);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(830, 475);
            this.GroupBox3.TabIndex = 17;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Среднее время ожидания пассажиров по времени и остановкам назначания";
            // 
            // StopForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 515);
            this.Controls.Add(this.GroupBox3);
            this.Name = "StopForm";
            this.Text = "StopForm";
            ((System.ComponentModel.ISupportInitialize)(this.OstDataGridView)).EndInit();
            this.GroupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.DataGridView OstDataGridView;
        internal System.Windows.Forms.GroupBox GroupBox3;
    }
}
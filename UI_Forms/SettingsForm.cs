using System;
using System.Windows.Forms;

namespace Транспорт2017
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            cor_textBox.Text = SettingsModel.FileNameCorresp;
            model_textBox.Text = SettingsModel.FileNameModel;
            trafic_textBox.Text = SettingsModel.FileNameTrafic;
            hours_numericUpDown.Value = SettingsModel.КолЧасовМоделирования;
            wait_numericUpDown.Value = SettingsModel.МаксВремяОжидания;
            pWait_numericUpDown.Value = (decimal)(1 - SettingsModel.ВероятностьПродолженияПоездки);
            fullReport_checkBox.Checked = SettingsModel.ПолныйОтчет;
            pasReport_checkBox.Checked = SettingsModel.ПасажировВОтчет;
            numericUpDown1.Value = SettingsModel.НачЧасДляТрафика;
            all_radioButton.Checked = SettingsModel.ПоВсемМаршрутам;
            have_radioButton.Checked = !all_radioButton.Checked;
        }
        private void _button_Click(object sender, EventArgs e)
        {
            if (sender == cor_button)
                RequestFileName("Выберите файл с корреспонденциями остановок", cor_textBox);
            else if (sender == model_button)
                RequestFileName("Выберите файл со списком остановок и маршрутов", model_textBox);
            else //if (sender == trafic_button)
                RequestFileName("Выберите файл с трафиком пассажиров", trafic_textBox);
        }

        private void RequestFileName(string title, TextBox txtBox)
        {
            openFileDialog1.Title = title;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                txtBox.Text = openFileDialog1.FileName;
        }

        private void save_button_Click(object sender, EventArgs e)
        {
            SettingsModel.FileNameCorresp = cor_textBox.Text;
            SettingsModel.FileNameModel = model_textBox.Text;
            SettingsModel.FileNameTrafic = trafic_textBox.Text;
            SettingsModel.КолЧасовМоделирования = (int)hours_numericUpDown.Value;
            SettingsModel.МаксВремяОжидания = (int)wait_numericUpDown.Value;
            SettingsModel.ВероятностьПродолженияПоездки = (double)(1 - pWait_numericUpDown.Value);
            SettingsModel.ПолныйОтчет = fullReport_checkBox.Checked;
            SettingsModel.ПасажировВОтчет = pasReport_checkBox.Checked;
            SettingsModel.НачЧасДляТрафика = (int)numericUpDown1.Value;
            SettingsModel.ПоВсемМаршрутам = all_radioButton.Checked;
            SettingsModel.Save();
            Close();
        }


    }
}

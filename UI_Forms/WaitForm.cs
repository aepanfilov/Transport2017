using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Транспорт2017
{
    public partial class WaitForm : Form
    {
        public event EventHandler CancelProcess;
        private int curProgress;
        public bool IsNumericProgressBar //установка режима прогрессбара - с цифрами или без цифр (неопределнное число итераций)
        {
            get { return progressBar1.Style == ProgressBarStyle.Continuous; }
            set
            {
                if (value)
                {
                    progressBar1.Style = ProgressBarStyle.Continuous;
                    procent_label.Visible = true;
                }
                else
                {
                    progressBar1.Style = ProgressBarStyle.Marquee;
                    procent_label.Visible = false;
                }
            }
        }

        public WaitForm()
        {
            InitializeComponent();
            curProgress = 0;
        }

        private void SetProcentLabel(int arg)
        {
            if (arg <= progressBar1.Maximum)
            {
                int proc = (int)(arg * 100.0 / progressBar1.Maximum);
                if (curProgress != proc)
                {
                    progressBar1.Value = arg;
                    procent_label.Text = string.Format("{0}% ({1:N0} / {2:N0})", proc, arg, progressBar1.Maximum);
                    curProgress = proc;
                }
            }
        }

        public void SetMaxValue(int arg)
        {
            progressBar1.Maximum = arg;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SetProcentLabel(e.ProgressPercentage);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (CancelProcess != null)
                CancelProcess(null, null);
        }

        public void SetTextLabel(string str)
        {
            label1.Text = str;
        }
    }
}
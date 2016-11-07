using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JangoWindowsFiles
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }
        private string file_format = "";
        private void button1_Click(object sender, EventArgs e)
        {
            LoadForm(CommonFileFormat.pdf_file_format);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadForm(CommonFileFormat.doc_file_format);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LoadForm(CommonFileFormat.excel_file_format);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadForm(CommonFileFormat.ppt_file_format);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoadForm(CommonFileFormat.zip_file_format);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LoadForm(CommonFileFormat.img_file_format);
        }

        private void LoadForm(string file_format)
        {
            var form = new Form2(file_format);
            form.Show();
        }
    }
}

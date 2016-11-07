using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JangoWindowsFiles
{
    public partial class UC_PictureBoxWithLbCk : UserControl
    {
        public UC_PictureBoxWithLbCk()
        {
            InitializeComponent();
        }
        public void Bind()
        {
            this.pictureBox1.Image = this.Img;
            this.label1.Text = this.lbTxt;
            this.checkBox1.Checked = this.IsChecked;
        }

        public int ID { get; set; }
        public Image Img { get; set; }

        public string lbTxt { get; set; }

        public bool IsChecked { get; set; }
    }
}

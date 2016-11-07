using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;

namespace JangoWindowsFiles
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button1_Click(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var drives = Environment.GetLogicalDrives();

            var drive_infos = DriveInfo.GetDrives();
            var index = 0;
            foreach (var item in drive_infos)
            {
                var avaliable_mb = (double)item.AvailableFreeSpace / 1024 / 1024;
                var total_mb = (double)item.TotalSize / 1024 / 1024;
                //listBox1.Items.Add(new { 磁盘 = item.Name, 可用空间 = item.AvailableFreeSpace / 1024 / 1024 + "MB", 磁盘总大小 = item.TotalSize / 1024 / 1024 + "MB" });
                listBox1.Items.Add(string.Format("磁盘{0},总大小{1},可用大小{2}", item.Name, Math.Round(avaliable_mb / 1024, 2) + "GB", Math.Round(total_mb / 1024, 2) + "GB"));
                //listBox1.Items.Add(new { 磁盘 = item.Name, 可用空间= item.AvailableFreeSpace / 1024 / 1024 + "MB", 磁盘总大小 = item.TotalSize / 1024 / 1024+"MB" });

                TabPage tp = new TabPage() { AutoScroll = true };
                tp.TabIndex = index;
                tp.Name = item.Name;
                tp.Text = item.Name.Replace(@":\", "") + "盘";
                index++;
                tabControl1.TabPages.Add(tp);
                //tabControl1.SelectedTab = tp;

            }

        }

        private void TabControl1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
        }

        private Label _tab_loadding_lab;
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var msgResult = MessageBox.Show("是否加载该磁盘重复文件？", "系统提示", MessageBoxButtons.YesNo);
            if (msgResult == DialogResult.No)
            {
                return;
            }
            if (msgResult == DialogResult.Yes)
            {
                var tabControl = (TabControl)sender;
                var selectedTab_index = tabControl.SelectedIndex;
                var selectedName = tabControl.SelectedTab.Name;
                _tab_loadding_lab = new Label() { Text = "Loadding...", Size = new Size(600, 50) };
                tabControl1.TabPages[selectedTab_index].Controls.Add(_tab_loadding_lab);

                var dir = new DirectoryInfo(selectedName);

                _tab_loadding_lab.Text = "loadding files now .";

                get_all_files(dir, selectedTab_index);

                write_repeatfilepath();
            }
        }

        private List<FileItem> current_dic = new List<FileItem>();
        private string sys_file_exts = ".sys,.Msi,.0,.1,.2,.3,._4,._5,lsn,.ns,.lock,.log";
        //private List<int> hashCodes = new List<int>();
        private void get_all_files(DirectoryInfo dir, int selected_tab_index)
        {
            //    var txt1 = new RichTextBox() { Text = "\r\t", Size = new Size(1000, 300), ScrollBars = RichTextBoxScrollBars.Both };
            //    tabControl1.TabPages[selected_tab_index].Controls.Remove(_tab_loadding_lab);
            //    tabControl1.TabPages[selected_tab_index].Controls.Add(txt1);
            if (dir.Extension.Length == 0)
            {
                var current_dir_files = dir.GetFiles();
                foreach (var current_dir_file in current_dir_files)
                {
                    if (!sys_file_exts.Contains(current_dir_file.Extension))
                    {
                        try
                        {

                            var stream = current_dir_file.OpenText();
                            var hashCode = stream.ReadToEnd().GetHashCode();

                            //txt1.Text += current_dir_file.FullName + "        hashCode: " + hashCode + "\r\t";

                            //hashCodes.Add(hashCode);
                            stream.Dispose();

                            current_dic.Add(new FileItem()
                            {
                                HashCode = hashCode,
                                Name = current_dir_file.Name,
                                FullName = current_dir_file.FullName,
                                Extension = current_dir_file.Extension,
                                CreateTime = current_dir_file.CreationTime,
                                UpdateTime = current_dir_file.LastWriteTime
                            });
                            //if (hashCodes.Contains(hashCode))
                            //{

                            //}
                        }
                        catch (Exception e)
                        {
                            continue;
                            //throw;
                        }
                    }

                }

                var current_dirs = dir.GetDirectories();
                foreach (var current_dir in current_dirs)
                {
                    if (current_dir.Attributes == FileAttributes.Directory)
                    {
                        get_all_files(current_dir, selected_tab_index);
                    }
                }


            }


        }

        private void write_repeatfilepath()
        {
            var path = "d:\\repeatfiles_paths.txt";
            var file = new FileInfo(path);
            if (!file.Exists)
            {
                file.Create();
            }
            using (var sr = file.AppendText())
            {
                sr.Write("\r\t\n\n");
                if (current_dic.Any())
                {
                    var re_dic = current_dic.GroupBy(x => x.HashCode);
                    if (re_dic.Any())
                    {
                        foreach (var item in re_dic)
                        {

                            var re_files_dic = item.ToList();
                            if (re_files_dic.Count > 1)
                            {
                                foreach (var re_file in re_files_dic)
                                {
                                    sr.Write("hashCode:" + re_file.HashCode + "    Name:" + re_file.Name + "  " + re_file.FullName + "\n\n\r\t");
                                }
                            }

                        }
                    }
                }
            }

        }
    }

    class FileItem
    {
        public int HashCode { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Extension { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}

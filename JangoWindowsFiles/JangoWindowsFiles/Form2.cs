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

namespace JangoWindowsFiles
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public Form2(string format)
        {
            InitializeComponent();
            this.label1.Text += format + "类型文件 " + " \n\n";
            loadFiles(format);
        }

        private void loadFiles(string aformat)
        {
            var drive_infos = DriveInfo.GetDrives();
            //var index = 0;
            //String str;
            //String nl = Environment.NewLine;
            //String query = "My system drive is %SystemDrive% and my system root is %SystemRoot%";
            //str = Environment.ExpandEnvironmentVariables(query);

            var sys_drive = Environment.ExpandEnvironmentVariables("%SystemDrive%") + "\\";

            foreach (var item in drive_infos)
            {
                if (item.Name != sys_drive)
                {
                    var dir = new DirectoryInfo(item.Name);
                    get_all_files(dir, aformat);
                }
            }
            get_repeatfilepath();
        }


        private List<FileItem> current_dic = new List<FileItem>();
        private void get_all_files(DirectoryInfo dir, string allow_file_exts)
        {
            if (dir.Extension.Length == 0)
            {
                var current_dir_files = dir.GetFiles();
                foreach (var current_dir_file in current_dir_files)
                {
                    if (!string.IsNullOrWhiteSpace(current_dir_file.Extension) && allow_file_exts.Contains(current_dir_file.Extension.ToLower()))
                    {
                        try
                        {

                            var stream = current_dir_file.OpenText();
                            var hashCode = stream.ReadToEnd().GetHashCode();
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
                        get_all_files(current_dir, allow_file_exts);
                    }
                }
            }
        }

        private void get_repeatfilepath()
        {
            var str = new StringBuilder();
            str.Append("\r\t\n\n");
            if (current_dic.Any())
            {
                int margin = 50;
                int cu_margin = 50 + 50;
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
                                str.AppendFormat("\n\nhashCode:{0},Name;{1},path:{2}", re_file.HashCode, re_file.Name, re_file.FullName);//  ("hashCode:" + re_file.HashCode + "    Name:" + re_file.Name + "  " + re_file.FullName + "\n\n\r\t");

                                //var uc_control = new UC_PictureBoxWithLbCk();
                                //uc_control.Img = Image.FromFile(re_file.FullName);
                                //uc_control.lbTxt = re_file.Name;

                                //listView1.Controls.Add(uc_control);
                                cu_margin += margin + 230;
                                PictureBox pic = new PictureBox() { Location = new Point(cu_margin, margin + 230) };
                                pic.Image = Image.FromFile(re_file.FullName);
                                pic.Show();

                                this.listView1.Controls.Add(pic);
                            }
                            str.Append("\r\t\r\t");
                        }

                    }
                }
            }
            var txt = str.ToString();
            this.richTextBox1.Text = txt;

        }



    }

    public class CommonFileFormat
    {
        public static string pdf_file_format = ".pdf";
        public static string doc_file_format = ".doc,.docx";
        public static string excel_file_format = ".xls,.xlsx";
        public static string ppt_file_format = ".ppt,.pptx";
        public static string zip_file_format = ".zip,.rar,.tar,.7z";
        public static string img_file_format = ".png,.jpg,.bpm,.jpeg";
        public static Dictionary<FileFormat, string> dic = new Dictionary<FileFormat, string>() { };

        static CommonFileFormat()
        {
            dic.Add(FileFormat.PDF, pdf_file_format);
            dic.Add(FileFormat.DOC, doc_file_format);
            dic.Add(FileFormat.EXCEL, excel_file_format);
            dic.Add(FileFormat.PPT, ppt_file_format);
            dic.Add(FileFormat.RAR, zip_file_format);
            dic.Add(FileFormat.IMG, img_file_format);
        }

        public string this[FileFormat key]
        {
            get { return dic.ContainsKey(key) ? dic[key] : null; }
            set { dic[key] = value; }
        }

    }
    public enum FileFormat
    {
        PDF,
        IMG,
        DOC,
        EXCEL,
        PPT,
        RAR
    }
}

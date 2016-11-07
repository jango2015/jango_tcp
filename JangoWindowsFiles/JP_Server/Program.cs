using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace JP_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //var _IPEndPoint =  new IPEndPoint()
            TcpListener listener = new TcpListener(IPAddress.Any, 9999);
            //listener.AcceptSocket();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("begin listening port 9999");
            listener.Start();

            while (true)
            {

                const int bufferSize = 256;
                //接受客户端的连接，利用client保存连接的客户端
                TcpClient client = listener.AcceptTcpClient();


                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("client connected ");

                Console.WriteLine("********************IP地址为:" + client.Client.RemoteEndPoint.ToString() + "的客户端，已经远程接入该服务器（服务器地址:" + client.Client.LocalEndPoint.ToString() + "）****************************");

                //获取客户端的流stream
                NetworkStream clientStream = client.GetStream();
                byte[] buffer = new byte[bufferSize];
                int readBytes = 0;
                //将客户端流读入到buffer中
                readBytes = clientStream.Read(buffer, 0, bufferSize);
                //将从客户端流读取的数据保存到字符串request中
                string request = Encoding.ASCII.GetString(buffer).Substring(0, readBytes);
                //如果客户端的命令以LIST开头，
                if (request.StartsWith("LIST"))
                {
                    // LIST request - return list
                    //利用类PictureHelper的函数GetFileListBytes，获取图片文件列表
                    byte[] responseBuffer = PictureHelper.GetFileListBytes();
                    //byte[] responseBuffer = System.Text.Encoding.Default.GetBytes("hahahahahhahahhah");
                    //将服务器获取的图片文件列表写入到clientStream中
                    clientStream.Write(responseBuffer, 0, responseBuffer.Length);
                }
                //如果客户端的请求命令以FILE开头，即获取单个图片文件
                else if (request.StartsWith("FILE"))
                {
                    // FILE request - return file
                    // get the filename
                    //获取请求的文件名字
                    string[] requestMessage = request.Split(':');
                    string filename = requestMessage[1];
                    //利用File.ReadAllBytes函数将文件里面的文件filename读入到字节数组data中，                
                    byte[] data = File.ReadAllBytes(Path.Combine(@"C:\Users\czg\Pictures", filename));
                    // Send the picture to the client.
                    //将data中的文件内容写入到客户端clientStream中，传回客户端
                    clientStream.Write(data, 0, data.Length);
                }
                //关闭客户端流
                clientStream.Close();
            }
        }

        //静态类PictureHelper，
        public static class PictureHelper
        {
            //提供文件夹中的文件列表
            public static string[] GetFileList()
            {
                string[] files = Directory.GetFiles(@"C:\Users\czg\Pictures");
                //去掉文件夹路径，只保留文件名
                // Remove the directory path from the filename.
                for (int i = 0; i < files.Length; i++)
                {
                    files[i] = Path.GetFileName(files[i]);
                }
                return files;
            }
            //将文件filename的内容读到字节数组中
            public static byte[] GetPictureBytes(string filename)
            {
                FileInfo fileInfo = new FileInfo(filename);
                byte[] buffer = new byte[fileInfo.Length];
                using (FileStream stream = fileInfo.OpenRead())
                {
                    stream.Read(buffer, 0, buffer.Length);
                }
                return buffer;
            }

            public static byte[] GetFileListBytes()
            {
                // LIST request - return list
                string[] files = PictureHelper.GetFileList();
                StringBuilder responseMessage = new StringBuilder();
                foreach (string s in files)
                {
                    responseMessage.Append(s);
                    responseMessage.Append(":");
                }
                byte[] responseBuffer = Encoding.ASCII.GetBytes(
                responseMessage.ToString());
                return responseBuffer;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JP_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            const int bufferSize = 4096;
            // Connect to the server.
            //生成一个TcpClinet
            TcpClient client = new TcpClient();
            //生成服务器的IPHostEntry,因为我的服务器和客户端在同一台计算机上，所以此处用localhost，实际应用中，此处的localhost应用用服务器的ip地址代替，
            IPHostEntry host = Dns.GetHostEntry("localhost");
            // 连接到服务器的8888端口
            
            //client.Connect(host.AddressList[0], 9999);
            client.Connect(host.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork), 9999);
            // Send a request to the server.
            //创建一个NetworkStream,  
            NetworkStream clientStream = client.GetStream();
            //request是"LIST"           
            string request = "LIST";
            //将request放入到字节数组requestBuffer中
            byte[] requestBuffer = Encoding.ASCII.GetBytes(request);
            //将请求写入到NetworkStream中，发送到服务器端
            clientStream.Write(requestBuffer, 0, requestBuffer.Length);
            //获取服务器端的回应
            // Read the response from the server.
            byte[] responseBuffer = new byte[bufferSize];
            //生成一个内存流memstream
            MemoryStream memStream = new MemoryStream();
            int bytesRead = 0;
            do
            {
                //将网络流中的数据按256字节一组的顺序写入到memStream中
                bytesRead = clientStream.Read(responseBuffer, 0, bufferSize);
                memStream.Write(responseBuffer, 0, bytesRead);
            } while (bytesRead > 0);
            clientStream.Close();
            client.Close();
            //从内存流读取数据
            byte[] buffer = memStream.GetBuffer();
            string response = Encoding.ASCII.GetString(buffer);
            //将服务器返回的文件列表分解开，文件名保存到字符串数组fileName中           
            //string[] fileNames = response.Split(':');

            Console.WriteLine(response);
            Console.WriteLine(@"\r\t\");
            Console.ReadLine();
        }
    }
}

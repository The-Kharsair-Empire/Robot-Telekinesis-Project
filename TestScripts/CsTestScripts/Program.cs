using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;
using System.Web;

namespace CsTestScripts
{
    class Program
    {

        private static int port = 21;
        private static string server = "localhost";

        private static TcpClient client;
        private static NetworkStream stream;
        private static StreamReader inChannel;
        private static StreamWriter outChannel;
        static void Main(string[] args)
        {
            connect(server, port);

            Console.WriteLine("connected");
            string res = sendAndRecv("fuck you server");
            Console.WriteLine(res);
            client.Close();
        }

        private static void connect(string host, int port)
        {
            client = new TcpClient(host, port);
            stream = client.GetStream();
            inChannel = new StreamReader(client.GetStream());
            outChannel = new StreamWriter(client.GetStream());
        }

        private static string sendAndRecv(string msg){

            // Byte[] data = Encoding.ASCII.GetBytes(msg);
            // stream.Write(data);
            string codeword = HttpUtility.UrlEncode(msg, Encoding.UTF8);
            outChannel.WriteLine(codeword);
            outChannel.Flush();

            // data = new byte[256];
            // int bytes = stream.Read(data, 0, data.Length);
            // string res = Encoding.ASCII.GetString(data, 0, bytes);
            string res = inChannel.ReadLine();
            res = HttpUtility.UrlDecode(res, Encoding.UTF8);
            return res;

        }
    }
}

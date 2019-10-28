using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace RawImageShimmer
{
    class ReadFileToShim
    {
        const string FilePath = "1024_16bit_Output.raw";
        const string HostInterfaceIP = "0.0.0.0";
        const int TransmitDataHostPortDefault = StreamDataListener.DetectorDataPortDefault + 565;
        const string DetectorDataIpDefault = "192.168.184.160";
        static byte[] HeaderBuf = new byte[1024];
        public static async Task<long> CopyRawImageFileToShim(string FilePath,
            string DetectorIpAddr = DetectorDataIpDefault,
            int port = TransmitDataHostPortDefault,
            CancellationToken ct = default(CancellationToken))
        {
            try
            {
                using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse("0.0.0.0");
                IPEndPoint local = new IPEndPoint(ip, 0);
                var detClient = new TcpClient(local);
                await detClient.ConnectAsync(DetectorIpAddr, port);
                NetworkStream detDataStream = detClient.GetStream();
                Console.WriteLine("connected.");
                using FileStream file = new FileStream(FilePath, FileMode.Open, System.IO.FileAccess.Read);
                Console.WriteLine($"copying >{FilePath} : {file.Length}< ");
                await file.CopyToAsync(detDataStream, ct);
                await detDataStream.FlushAsync(ct);
                detDataStream.Close();
                return file.Length;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending image data from the file >{FilePath}< ");
                Console.WriteLine($" {e} ");
            };
            return 0;
        }
    }
}

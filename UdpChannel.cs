using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;

namespace RawImageShimmer
{
    class UdpChannel
    {
        static Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);
        static SemaphoreSlim socketLock = new SemaphoreSlim(1, 1);
        static IPEndPoint ConsoleEndPoint = new IPEndPoint(IPAddress.Broadcast, 0x4153 + 48);
        Channel<string> msgChan;
        //public static async Task<int> Run(string IpAddress, int port)
        //{
        //    if (string.IsNullOrEmpty(msg)) return -1;
        //    int msgLength = msg.Length;
        //    var buffer = new ArraySegment<byte>(ArrayPool<byte>.Shared.Rent(msgLength), 0, msgLength);
        //    try
        //    {
        //        Encoding.ASCII.GetBytes(msg, 0, msgLength, buffer.Array, 0);
        //        if (!await socketLock.WaitAsync(100)) { return -1; }
        //        {
        //            try
        //            {
        //                return await socket.SendToAsync(buffer, SocketFlags.None, ConsoleEndPoint);
        //            }
        //            finally { socketLock.Release(); }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //Task.Delay(100);
        //    }
        //    finally
        //    {
        //        ArrayPool<byte>.Shared.Return(buffer.Array);
        //    }
        //    return -1;
        //}

        //int test()
        //{
        //    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        //    {
        //        Task connectTsk = SocketTaskExtensions.ConnectAsync(socket, detInfo.detIp, detInfo.detPort);
        //        connectTsk.Wait();
        //        version = TlpPacket.ReadPciRegisterAsync(
        //            socket, 1, 0x5000_106C).Result;
        //        //if (version < 1) { return (0, 0, 0); }
        //        xShift = TlpPacket.ReadPciRegisterAsync(
        //            socket, 1, 0x5000_1070).Result;
        //        yShift = TlpPacket.ReadPciRegisterAsync(
        //            socket, 1, 0x5000_1074).Result;
        //    }
        //    return (dwVersion: version, xShift: xShift, yShift: yShift);
        //    return 0;
        //}
    }
}

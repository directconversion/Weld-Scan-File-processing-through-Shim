using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

namespace RawImageShimmer
{
    class Program
    {
        //We need some Channels !
        const int UdpPort = 0x4153;
        public static readonly IPEndPoint LocalIpEndPointLoopback = new IPEndPoint(IPAddress.Loopback, UdpPort);
        void funcList()
        {
            int level = 0;
            int mode = 0;
            int xShift = 0;
            int yShift = 0;
            string fileName = "";
            var txt0 = IpcEnvelope.SendCmdAndAwaitReply("set_filter_level", new string[] { $"{level}" });
            var txt2 = IpcEnvelope.SendCmdAndAwaitReply("set_dw_mode", new string[] { $"{mode}" });
            string txt1 = IpcEnvelope.SendCmdAndAwaitReply("set_dw_shift", new string[] { $"{xShift}", $"{yShift}" });
            string txt3 = IpcEnvelope.SendCmdAndAwaitReply("load_raw_gain_file", new string[] { fileName });
            string reply0 = IpcEnvelope.SendCmdAndAwaitReply("get_raw_last_filename", new string[0]);
            IpcEnvelope.Cmd2JsonText("release_raw_file", new string[] { });
            var txt = IpcEnvelope.SendCmdAndAwaitReply("release_raw_file", new string[] { });
            var reptxt = IpcEnvelope.WaitForJsonReply();
            //string fn = GetLastRawFilename();
            //string reply1 = LoadGainFromRaw(fn);
            //SendDwMode();
        }
        static string CopyFile(string fn)
        {
            var defaultPath = @"C:/Xcounter/FlatField.txt";
            DirectoryInfo info = new DirectoryInfo(defaultPath);
            //Fails if mmf still open
            File.Copy(fn, defaultPath, overwrite: true);
            return defaultPath;
        }

        public const string GainWaitName = "XCounter.ThorShim.Gain.1.0";
        public static string DefaultGainFileName = @"C:/Xcounter/FlatField.txt";
        public static EventWaitHandle wh =
            new EventWaitHandle(false, EventResetMode.AutoReset, GainWaitName);

        //private string LoadGainFromRaw(string fn)
        //{
        //    if (string.IsNullOrWhiteSpace(fn))
        //    { return "no Filename"; }
        //    if (!File.Exists(fn))
        //    { return $"File does not exist >{fn}<"; }
        //    try
        //    {
        //        Console.WriteLine($"Processing {fn}");
        //        var fnOut = fn + ".ref.out.txt";

        //        var floodInfo = ImgMtxOps.ReadFromFile(fn);
        //        //DW hack
        //        //if (floodInfo.Width == 1024)
        //        //{
        //        //    //floodInfo.Width = 2048;
        //        //    floodInfo.Height = floodInfo.Height / 2;
        //        //}
        //        var (width, height, rawCalib) = floodInfo.ToTuple();

        //        //Crop
        //        //int y0 = 0;
        //        int rgnHeight = Math.Min(8192, height);
        //        //ALEX
        //        var result = ImgMtxOps.MakeNewCal((width, rgnHeight, rawCalib));// floodInfo);
        //        var lines = result.Select(f => f.ToString("F4", CultureInfo.InvariantCulture)).ToList();
        //        File.WriteAllLines(fnOut, lines);

        //        //ImgMtxOps.MakeNewCalFromFile(fn, fnOut);
        //        var dest = CopyFile(fnOut);
        //        return $"Copied {fnOut} to {dest} ";
        //    }
        //    catch { return $"Unable to process file >{fn}<"; }
        //}
        //public static string SendJsonForReply(JObject json)
        //{
        //    string msg = json.ToString();
        //    byte[] DGram = Encoding.ASCII.GetBytes(msg);
        //    using (UdpClient udpClient = new UdpClient())
        //    {
        //        udpClient.ExclusiveAddressUse = false;
        //        udpClient.Client.ReceiveTimeout = 1000;
        //        //Dont do this !
        //        udpClient.Connect(LocalIpEndPointLoopback);
        //        udpClient.Send(DGram, DGram.Length);//, LocalIpEndPointLoopback);//, LocalIpEndPointLoopback);
        //        //udpClient.Send(DGram, DGram.Length, LocalIpEndPointLoopback);//, LocalIpEndPointLoopback);
        //        IPEndPoint host = default;
        //        var reply = "";
        //        try
        //        {
        //            udpClient.EnableBroadcast = true;
        //            var data = udpClient.Receive(ref host);
        //            reply = Encoding.ASCII.GetString(data);
        //        }
        //        catch (SocketException e) when (e.SocketErrorCode == SocketError.TimedOut)
        //        {
        //            reply = "Sent ... No Reply";
        //        }
        //        catch (Exception e)
        //        {
        //            reply = e.Message;
        //        }
        //        return reply;
        //    }
        //}
        private void button3_Click(object sender, EventArgs e)
        {
            string msg = "Blah blah \nbad json";
            byte[] DGram = Encoding.ASCII.GetBytes(msg);
            using (UdpClient udpClient = new UdpClient())
            {
                udpClient.Connect(IpcEnvelope.LocalIpEndPointLoopback);
                udpClient.Send(DGram, DGram.Length);//, LocalIpEndPointLoopback);
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            wh.Set();
        }

        //private void TextBox1_DoubleClick(object sender, EventArgs e)
        //{
        //    var fPick = new OpenFileDialog();
        //    fPick.InitialDirectory = "C:\\XCounter\\RawImages";
        //    fPick.Title = "Select Gain File";
        //    fPick.Filter = "Raw image (*.raw)|*.raw";
        //    fPick.Multiselect = false;
        //    // details view needs window handle
        //    if (fPick.ShowDialog() == DialogResult.OK)
        //    {
        //        textBox1.Text = fPick.FileName;
        //    }
        //}
        //static FormattableString 
        static string FocusShift = @"{
  ""DConVersion"": 3,
  ""Target"": ""shim"",
  ""Caller"": ""stanley-aquisition"",
  ""Command"": ""set_dw_shift"",
  ""ReceiptRequested"": false,
  ""CallerCommandIndex"": 0,
  ""Args"": [
    ""x_shift"",
    ""y_shift""
  ]
}";
        static string OnlyDeliverJson = @"{
  ""DConVersion"": 3,
  ""Target"": ""shim"",
  ""Caller"": ""stanley-aquisition"",
  ""Command"": ""set_filter_level"",
  ""ReceiptRequested"": false,
  ""CallerCommandIndex"": 0,
  ""Args"": [
    ""0""
  ],
  ""Valid"": true,
  ""CommandOk"": true,
  ""result"": ""filter_level = 0""
}";
        static string GainOnJson = @"{
  ""DConVersion"": 3,
  ""Target"": ""shim"",
  ""Caller"": ""stanley-aquisition"",
  ""Command"": ""set_filter_level"",
  ""ReceiptRequested"": false,
  ""CallerCommandIndex"": 0,
  ""Args"": [
    ""1""
  ],
  ""Valid"": true,
  ""CommandOk"": true,
  ""result"": ""filter_level = 1""
}";
        static string TileEdgeCorOnJson = @"{
  ""DConVersion"": 3,
  ""Target"": ""shim"",
  ""Caller"": ""stanley-aquisition"",
  ""Command"": ""set_filter_level"",
  ""ReceiptRequested"": false,
  ""CallerCommandIndex"": 0,
  ""Args"": [
    ""2""
  ],
  ""Valid"": true,
  ""CommandOk"": true,
  ""result"": ""filter_level = 2""
}";
        static string BltFilterCorOnJson = @"{
  ""DConVersion"": 3,
  ""Target"": ""shim"",
  ""Caller"": ""stanley-aquisition"",
  ""Command"": ""set_filter_level"",
  ""ReceiptRequested"": false,
  ""CallerCommandIndex"": 0,
  ""Args"": [
    ""3""
  ],
  ""Valid"": true,
  ""CommandOk"": true,
  ""result"": ""filter_level = 3""
}";

        static async Task Main(string[] args)
        {
            var folder = @"C:\Users\Xela\Downloads\moses191006\A\20191003\";
            var gainfolder = @"C:\XCounter\RawImages\";
            var gainFileName = "GainScanFile_03-10-2019_03.48.58.raw";
            var fn = @"191021135414.raw";
            fn = @"JT13-0157_03-10-2019_05.17.14.raw";

            folder = @"C:\Users\Xela\Downloads\moses191006\A\20191004\";
            fn = "JT13-0999_04-10-2019_02.54.11.raw";

            var fileListText =
                @"C:\Users\Xela\Downloads\moses191006\A\20191003\JT13-0155_03-10-2019_03.15.25.raw
C:\Users\Xela\Downloads\moses191006\A\20191003\JT13-0156_03-10-2019_04.32.04.raw
C:\Users\Xela\Downloads\moses191006\A\20191003\JT13-0156_03-10-2019_04.32.07.raw
C:\Users\Xela\Downloads\moses191006\A\20191003\JT13-0157_03-10-2019_05.00.04.raw
C:\Users\Xela\Downloads\moses191006\A\20191003\JT13-0157_03-10-2019_05.00.07.raw
C:\Users\Xela\Downloads\moses191006\A\20191003\JT13-0157_03-10-2019_05.17.11.raw
C:\Users\Xela\Downloads\moses191006\A\20191003\JT13-0157_03-10-2019_05.17.14.raw
C:\Users\Xela\Downloads\moses191006\A\20191004\JT13-0999_04-10-2019_02.54.11.raw";

            var GainPath = gainfolder + gainFileName;

            //fileListText = @"C:\XCounter\Data\BigStitch\190503092931.raw";
            ////@"C:\XCounter\Data\Gains\190502155431.raw" 
            ////,"FixUp"
            ////@"C:\tmp\191021135414.raw";
            //GainPath = @"C:\XCounter\Data\Gains\190502155431.raw";// C:\XCounter\Data\Gains\190502155431.raw";

            //fileListText = @"C:\Users\Xela\Downloads\moses191006\A\20191003\JT13-0157_03-10-2019_05.17.14.raw";
            //GainPath = @"C:\XCounter\RawImages\GainScanFile_03-10-2019_03.48.58.raw";
            //GainPath = @"C:\Users\Xela\Downloads\191023105442.raw";

            //Grotty looking
            fileListText = @"C:\Users\Xela\Downloads\jason 191023\1-AM-L-0004_23-10-2019_01.58.23.raw";
            GainPath = @"C:\Users\Xela\Downloads\jason 191023\GainScanFile_21-10-2019_01.10.23.raw";

            fileListText = @"C:\Users\Xela\Downloads\jason 191023\1-AML-0001_23-10-2019_11.10.11.raw";//dw
            GainPath = @"C:\Users\Xela\Downloads\191023105442.raw";

            fileListText = @"H:\butting_1911\butting raw files\191105031958.raw";// 191105032428.raw";// 191105031640.raw";//dw
            GainPath = @"H:\butting_1911\butting raw files\191105030424g.raw";// 191105030031g.raw";

            //fileListText = @"C:\tmp\Jason\shim191106\1-A-ML-0001_06-11-2019_09.35.13.raw";//dw
            //GainPath = @"C:\tmp\Jason\shim191106\GainScanFile_06-11-2019_09.17.36.raw";

            fileListText = @"C:\Users\Xela\Downloads\wetransfer-7adb80\PROC-0001_19-11-2019_04.40.14.raw";
            GainPath = @"C:\Users\Xela\Downloads\wetransfer-7adb80\36x762(1).raw";

            fileListText = @"H:\ProjectData\Stanley\wetransfer-3b040c\PROC-0002_21-11-2019_12.19.15.raw
H:\ProjectData\Stanley\wetransfer-3b040c\PROC-0003_21-11-2019_03.46.06.raw
H:\ProjectData\Stanley\wetransfer-3b040c\RTR IMAGES\PROC-0001_19-11-2019_04.40.14.raw
H:\ProjectData\Stanley\wetransfer-3b040c\RTR IMAGES\PROC-0001_20-11-2019_12.46.34.raw";

            //named variable replace
            var fileListTest = new List<(string, string)>() {
                (@"H:\ProjectData\Stanley\wetransfer-3b040c\PROC-0002_21-11-2019_12.19.15.raw",@"H:\ProjectData\Stanley\wetransfer-3b040c\RTR IMAGES\36X762(2)\36X762(2).raw"),
                (@"H:\ProjectData\Stanley\wetransfer-3b040c\PROC-0003_21-11-2019_03.46.06.raw",@"H:\ProjectData\Stanley\wetransfer-3b040c\RTR IMAGES\36X762(2)\36X762(2).raw"),
                (@"H:\ProjectData\Stanley\wetransfer-3b040c\RTR IMAGES\PROC-0001_19-11-2019_04.40.14.raw",@"H:\ProjectData\Stanley\wetransfer-3b040c\RTR IMAGES\36X762(2)\36X762(2).raw"),
                (@"H:\ProjectData\Stanley\wetransfer-3b040c\RTR IMAGES\PROC-0001_20-11-2019_12.46.34.raw",@"H:\ProjectData\Stanley\wetransfer-3b040c\RTR IMAGES\36X762(2)\36X762(2).raw"),
                };


            fileListText = @"H:\ProjectData\Stanley\wetransfer-3b040c\RTR IMAGES\PROC-0001_19-11-2019_04.40.14.raw";   
            fileListText = @"H:\ProjectData\Stanley\wetransfer-3b040c\PROC-0002_21-11-2019_12.19.15.raw";
            GainPath = @"H:\ProjectData\Stanley\wetransfer-3b040c\RTR IMAGES\36X762(2)\36X762(2).raw";
            //GainPath = @"H:\ProjectData\Stanley\wetransfer-3b040c\36X635\36X635.raw";
            //GainPath = @"H:\ProjectData\Stanley\wetransfer-3b040c\RTR IMAGES\36X529B\36X529B.raw";
            //GainPath = @"H:\ProjectData\Stanley\wetransfer-7adb80\36x762(1).raw";

            fileListText = @"C:\tmp\MistrasGianCor.raw";

            fileListText = @"C:\tmp\Jason\1-A-ML-002_03-12-2019_11.47.38.raw";
            GainPath = @"C:\tmp\Jason\24x375_JM_1203_No filter.raw";

            fileListText = @"C:\tmp\Jason\sw\1AML-001_04-12-2019_10.40.31.raw";
            GainPath = @"C:\tmp\Jason\sw\20x250_JBARN.raw";//GainScanFile_04-12-2019_10.35.19.raw";
            //GainPath = @"C:\tmp\Jason\sw\GainScanFile_04-12-2019_10.35.19.raw";

            fileListText = @"C:\tmp\ACC-20-2nd.raw";// ACC12.raw";
            fileListText = @"C:\tmp\ACC-20-2nd.raw";// ACC12.raw";
            fileListText = @"C:\tmp\LineGain.raw";// ACC12.raw";
            fileListText = @"C:\tmp\NoGain.raw";// ACC12.raw";
            GainPath = @"C:\tmp\Flat.raw";

            fileListText = @"C:\tmp\Jason\TileLineExample200417\PHP SP5-00036_17-04-2020_10.17.09.raw";
            GainPath = @"C:\tmp\Jason\TileLineExample200417\GainScanFile_17-04-2020_09.19.33.raw";

            fileListText = @"C:\tmp\ForMoses\20190620\SW-001_20-06-2019_03.12.03.raw";
          GainPath = @"H:\ProjectData\Stanley\Jason\wetransfer-ab9748\GainScanFile_25-06-2019_11.07.34.raw";

            var fileList = fileListText.Split('\n').Select(sx => sx.Trim()).ToList();
            if (args.Count() >= 2)
            {
                fileList.Clear();
                fileList.AddRange(args);
                GainPath = args.Last();
            }

            //rawfolder = @"C:\XCounter\RawImages\";
            //folder = rawfolder;
            //gainFileName = "191017123647.raw";
            //fn = "191017124101.raw";

            Console.WriteLine("Hello World!");
            var ipc = IpcEnvelope.MakeCmd("load_raw_gain_file", new string[] { GainPath });
            string sx = JsonSerializer.Serialize<IpcEnvelope>(ipc);
            string ipcString = IpcEnvelope.Cmd2JsonText("load_raw_gain_file", new string[] { GainPath });


            string txt3 = IpcEnvelope.SendCmdAndAwaitReply("load_raw_gain_file", new string[] { GainPath });
            string txt4 = IpcEnvelope.SendJsonForReply(OnlyDeliverJson);
            //txt4 = IpcEnvelope.SendJsonForReply(GainOnJson);
            txt4 = IpcEnvelope.SendJsonForReply(TileEdgeCorOnJson);
            //txt4 = IpcEnvelope.SendJsonForReply(BltFilterCorOnJson);
            //Console.WriteLine(txt3);
            //Console.WriteLine(ipcString);
            //Console.WriteLine(txt4);
            int x_shift = -1;
            int y_shift = +0;
            var IpcFocus = FocusShift.Replace(nameof(x_shift), x_shift.ToString(CultureInfo.InvariantCulture))
                .Replace(nameof(y_shift), y_shift.ToString(CultureInfo.InvariantCulture));
            string txt5 = IpcEnvelope.SendJsonForReply(IpcFocus);
            Console.WriteLine(txt5);

            Task.Delay(3000).Wait();
            //     wh.Set();
            Task.Delay(1000).Wait();

            int nFiles = fileList.Count();
            // nFiles = 2;
            for (int i = 0; i < nFiles; i++)
            {
                //string reply = IpcEnvelope.SendCmdAndAwaitReply("load_raw_gain_file", new string[] { rawfolder + gainFileName });
                //Task.Delay(1000).Wait();
                string inputPath = fileList[i];
                string fnBase = Path.GetFileNameWithoutExtension(inputPath);
                string outputPath = $@"C:/tmp/a{i}.raw";
                var sf = Task.Run(() => StreamDataListener.RunImageDataTcp($@"C:/tmp/{fnBase}.cor.raw", StreamDataListener.DetectorDataPortDefault));
                var t = Task.Run(() => ReadFileToShim.CopyRawImageFileToShim(inputPath));
                // @"C:\XCounter\RawImages\1024_16bit_Output.raw");
                Task.WaitAll(sf, t);
            }
        }
    }

    public static class IpcEnvelopeExtMeth
    {
        public static unsafe Span<Td> ReCast<Ts, Td>(this Span<Ts> src)
            where Ts : struct where Td : struct
            => MemoryMarshal.Cast<Ts, Td>(src);

        public static unsafe ReadOnlySpan<Td> ReCast<Ts, Td>(this ReadOnlySpan<Ts> src)
            where Ts : struct where Td : struct
            => MemoryMarshal.Cast<Ts, Td>(src);

        public static unsafe Span<Td> ReCast<Ts, Td>(this Ts[] src)
            where Ts : struct where Td : struct
            => MemoryMarshal.Cast<Ts, Td>(src);

        public static string Crlf(this string sx)
        {
            if (sx.EndsWith("\n")) { return sx; }
            return sx + Environment.NewLine;
        }
    }
    public class IpcEnvelope
    {
        const int UdpPort = 0x4153;
        public static readonly IPEndPoint LocalIpEndPointLoopback = new IPEndPoint(IPAddress.Loopback, UdpPort);

        public int DConVersion { get; set; } = 3;
        public string Target { get; set; }
        public string Caller { get; set; }
        public string Command { get; set; }
        public bool ReceiptRequested { get; set; }
        public int CallerCommandIndex { get; set; }
        public string[] Args { get; private set; }
        public string json { get; set; }
        public string response { get; set; }
        public bool Valid { get; set; }

        public IpcEnvelope()
        {
            Target = "";
            Caller = "";
            Command = "";
            Args = new string[0];
        }
        public static IpcEnvelope MakeCmd(string cmd, string[] args)
        {
            IpcEnvelope ipc = new IpcEnvelope()
            {
                DConVersion = 3,
                Target = "shim",
                Caller = "stanley-aquisition",
                ReceiptRequested = false,
                CallerCommandIndex = 0,
            };
            ipc.Command = cmd.ToLower();
            ipc.Args = args.ToArray();
            return ipc;
        }
        public static void SendJson(string json)
        {
            string msg = json.ToString();
            byte[] DGram = Encoding.ASCII.GetBytes(msg);
            using (UdpClient udpClient = new UdpClient())
            {
                udpClient.ExclusiveAddressUse = false;
                udpClient.Connect(LocalIpEndPointLoopback);
                udpClient.Send(DGram, DGram.Length);//, LocalIpEndPointLoopback);
            }
        }

        public static string WaitForJsonReply()
        {
            try
            {
                using (UdpClient udpClient = new UdpClient())
                {
                    udpClient.ExclusiveAddressUse = false;
                    udpClient.Client.ReceiveTimeout = 500;
                    udpClient.Connect(LocalIpEndPointLoopback);
                    IPEndPoint host = default;
                    var data = udpClient.Receive(ref host);
                    var reply = Encoding.ASCII.GetString(data);
                    return reply;
                }
            }
            catch (Exception e)
            {
                e = e;
                return "No Reply\n";
            }
        }

        public static string SendJsonForReply(string json)
        {
            string msg = json.ToString();
            byte[] DGram = Encoding.ASCII.GetBytes(msg);
            using (UdpClient udpClient = new UdpClient())
            {
                udpClient.ExclusiveAddressUse = false;
                udpClient.Client.ReceiveTimeout = 8 * 1000;
                //Dont do this !
                udpClient.Connect(LocalIpEndPointLoopback);
                udpClient.Send(DGram, DGram.Length);//, LocalIpEndPointLoopback);//, LocalIpEndPointLoopback);
                                                    //udpClient.Send(DGram, DGram.Length, LocalIpEndPointLoopback);//, LocalIpEndPointLoopback);
                IPEndPoint host = default;
                var reply = "";
                try
                {
                    udpClient.EnableBroadcast = true;
                    var data = udpClient.Receive(ref host);
                    reply = Encoding.ASCII.GetString(data);
                }
                catch (SocketException e) when (e.SocketErrorCode == SocketError.TimedOut)
                {
                    reply = "Sent ... No Reply";
                }
                catch (Exception e)
                {
                    reply = e.Message;
                }
                return reply;
            }
        }

        //public JObject ToJson()
        //{
        //    JObject jso = new JObject();
        //    dynamic album = jso;
        //    album.DConVersion = DConVersion;
        //    album.Target = Target;
        //    album.Caller = Caller;
        //    album.Command = Command;
        //    album.ReceiptRequested = ReceiptRequested;
        //    album.CallerCommandIndex = CallerCommandIndex;
        //    album.Args = new JArray(Args);
        //    return jso;
        //}

        public static string SendIpc(IpcEnvelope ipc)
        {
            string json = "FAIL to make json";
            try
            {
                //JObject jso = ipc.ToJson();
                //json = jso.ToString();
                json = JsonSerializer.Serialize<IpcEnvelope>(ipc);
                SendJson(json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return json;
        }

        public static string SendIpcForReply(IpcEnvelope ipc)
        {
            try
            {
                //JObject jso = ipc.ToJson();
                //    jso.ToString();
                string json = JsonSerializer.Serialize<IpcEnvelope>(ipc);
                var rep = SendJsonForReply(json);
                return rep;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return "";
        }

        public static String SendCmdAndAwaitReply(string cmd, string[] args)
        {
            IpcEnvelope ipc = IpcEnvelope.MakeCmd(cmd, args);
            string rep = SendIpcForReply(ipc);
            return rep;
        }
        public static string SendCmd(string cmd, string[] args)
        {
            IpcEnvelope ipc = IpcEnvelope.MakeCmd(cmd, args);
            return SendIpc(ipc);
        }
        public static string Cmd2JsonText(string cmd, string[] args)
        {
            IpcEnvelope ipc = IpcEnvelope.MakeCmd(cmd, args);
            string json = JsonSerializer.Serialize<IpcEnvelope>(ipc);
            //JObject jso = ipc.ToJson();
            //string json = jso.ToString();
            return json;
        }
    }
    public interface IBasicImgHdr
    {
        int Width { get; set; }
        int Height { get; set; }
    }

    public interface IBasicImage : IBasicImgHdr
    {
        float[] Pix { get; set; }
        (int width, int height, float[] data) ToTuple();
    }

    public class BasicImage : IBasicImage
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float[] Pix { get; set; }
        public (int width, int height, float[] data) ToTuple()
            => (Width, Height, Pix);

        public static IBasicImage ToBasicImage(int awidth, int aheight, float[] data) =>
            new BasicImage(awidth, aheight, data);
        public static IBasicImage ToBasicImage((int width, int height, float[] data) tup) =>
            ToBasicImage(tup.width, tup.height, tup.data);

        public BasicImage() { }
        public BasicImage(int awidth, int aheight, float[] data)
        { Width = awidth; Height = aheight; Pix = data; }
    }
}

//public static IBasicImage ReadFromFile(string fn)
//{
//    var isTiff =
//        fn.EndsWith(".tif", true, CultureInfo.InvariantCulture)
//        || fn.EndsWith(".tiff", true, CultureInfo.InvariantCulture);
//    var result = isTiff ? ImageFile.ReadTiff(fn) : BasicImage.ToBasicImage(RawImageFileStatic.ReadRawOld(fn));
//    return result;
//}
//public static IBasicImage ReadFromFile(string fn)
//{
//    var isTiff =
//        fn.EndsWith(".tif", true, CultureInfo.InvariantCulture)
//        || fn.EndsWith(".tiff", true, CultureInfo.InvariantCulture);
//    var timg = RawImageFileStatic.ReadRawOld(fn);
//    var result = isTiff ? ImageFile.ReadTiff(fn) : BasicImage.ToBasicImage(timg);
//    return result;
//}

//    public static class RawImageFileStatic
//{
//public static IRawHeader ReadHeader(Stream f)
//{
//    SCAN_HEADER_V2 hdr = default;
//    var bytes = new byte[hdr.Size()];
//    var nb = f.Read(bytes, 0, bytes.Length);
//    Deb.ug.Assert(nb == hdr.Size(), "nb == hdr.Size()");
//    var result = ReadHeader(bytes, out hdr);
//    Int64 fsize = f.Length - 1024;
//    int h = (Int32)(fsize / hdr.RawImgWidthBytes);

//    // int h = hdr.RawFramesFromBytes(fsize) * hdr.LinesPerFrame;
//    result.Height = h;
//    return result;
//}

//public static RawHeader ReadHeader(byte[] bytes, out SCAN_HEADER_V2 hdr)
//{
//    const int hdrSize = 1024;
//    //SCAN_HEADER_V2 hdr = default;
//    Deb.ug.Assert(bytes.Length == hdrSize, "bytes.Length == hdrSize");
//    hdr = SCAN_HEADER_V2.CopyFromBytes(bytes);
//    hdr.Ready();
//    int h = 0;
//    var result = RawHeader.Create(hdr.RawImgWidthPixels, h,
//        hdr.LinesPerFrame, hdr.FrameHeaderBytes);
//    return result;
//}

//public static RawHeader ReadHeader(byte[] bytes)
//{
//    const int hdrSize = 1024;
//    SCAN_HEADER_V2 hdr = default;
//    Deb.ug.Assert(bytes.Length == hdr.Size(), "bytes.Length == hdr.Size()");
//    hdr = SCAN_HEADER_V2.CopyFromBytes(bytes);
//    hdr.Ready();
//    int h = 0;
//    var result = RawHeader.Create(hdr.RawImgWidthPixels, h,
//        hdr.LinesPerFrame, hdr.FrameHeaderBytes);
//    return result;
//}

//public static IRawHeader ReadHeaderOld(Stream f)
//{
//    const int hdrSize = 1024;
//    SCAN_HEADER_V2 hdr = default;
//    var bytes = new byte[hdr.Size()];
//    Int64 fsize = f.Length;
//    var nb = f.Read(bytes, 0, hdr.Size());
//    Deb.ug.Assert(nb == hdrSize, "nb == hdrSize");
//    hdr = SCAN_HEADER_V2.CopyFromBytes(bytes);
//    hdr.Ready();
//    int h = hdr.RawFramesFromBytes(fsize) * hdr.LinesPerFrame;
//    var result = RawHeader.Create(hdr.RawImgWidthPixels, h,
//        hdr.LinesPerFrame, hdr.FrameHeaderBytes);
//    return result;
//}
////(hdr.ImgWidthPixels, hdr.FramesFromBytes(fsize) * hdr.LinesPerFrame, hdr.LinesPerFrame, hdr.FrameHeaderBytes);

//public static IRawHeader ReadHeader(string fileName)
//{
//    IRawHeader result;
//    using (var f = File.OpenRead(fileName))
//    {
//        result = ReadHeader(f);
//    }
//    return result;
//}


//public static (int width, int height, Single[] pix)
//    ReadRaw(int w, int h, string fileName, int hdrSize = 0) //ReadRaw(string fileName)
//{
//    //const int hdrSize = skipHdrSize;
//    var hdr = RawHeader.Create(w, h, hdrSize);
//    return ReadRaw(hdr, fileName);
//}

//public static (int width, int height, Single[] pix) ReadRaw(IRawHeader hdr, string fileName) //ReadRaw(string fileName)
//{
//    //const int hdrSize = skipHdrSize;
//    var dataLength = hdr.Width * hdr.Height;
//    int bytesPerLine = hdr.Width * 2;
//    int LinesPerFrame = hdr.LinesPerFrame;
//    byte[] bytes = new byte[bytesPerLine];
//    //            var pix = new float[dataLength]; //ALEX ARRAY POOL RENT and RETURN
//    var pix = ArrayPool<float>.Shared.Rent(dataLength); //ALEX ARRAY POOL RENT and RETURN
//    using (var f = File.OpenRead(fileName))
//    {
//        for (int ny = 0; ny < hdr.Height; ny++)
//        {
//            //var floatPixelLine = new Span<float>(pix, ny * hdr.Width, hdr.Width);
//            var floatPixelLine = pix.AsSpan(ny * hdr.Width, hdr.Width);
//            int skip = hdr.HeaderSize + (ny / hdr.LinesPerFrame) * hdr.FrameHeaderSize;
//            int pixPos = ny * bytesPerLine;
//            f.Seek(skip + pixPos, SeekOrigin.Begin);
//            f.Read(bytes, 0, bytesPerLine);
//            var spU16 = bytes.ReCast<byte, UInt16>();
//            spU16.ShortsToFloats(floatPixelLine);
//        }
//    }
//    return (hdr.Width, hdr.Height, pix);
//    //hdr.Width	1024	int

//}

//public static (int width, int height, Single[] pix) ReadRaw(string fileName, int w, int h)
//{
//    const int hdrSize = 1024;
//    var hdr = ReadHeader(fileName);
//    //(hdr as RawHeader).Width = 2048;
//    //(hdr as RawHeader).Height = 8192;
//    var dataLength = hdr.Width * hdr.Height;
//    int bytesPerLine = hdr.Width * 2;
//    int LinesPerFrame = hdr.LinesPerFrame;
//    byte[] bytes = new byte[bytesPerLine];
//    //            var pix = new float[dataLength]; //ALEX ARRAY POOL RENT and RETURN
//    var pix = ArrayPool<float>.Shared.Rent(dataLength); //ALEX ARRAY POOL RENT and RETURN
//    using (var f = File.OpenRead(fileName))
//    {
//        for (int ny = 0; ny < hdr.Height; ny++)
//        {
//            //var floatPixelLine = new Span<float>(pix, ny * hdr.Width, hdr.Width);
//            var floatPixelLine = pix.AsSpan(ny * hdr.Width, hdr.Width);
//            int skip = hdrSize + (ny / hdr.LinesPerFrame) * hdr.FrameHeaderSize;
//            int pixPos = ny * bytesPerLine;
//            f.Seek(skip + pixPos, SeekOrigin.Begin);
//            f.Read(bytes, 0, bytesPerLine);
//            var spU16 = bytes.ReCast<byte, UInt16>();
//            spU16.ShortsToFloats(floatPixelLine);
//        }
//    }
//    return (hdr.Width, hdr.Height, pix);
//}
//public static (int width, int height, Single[] pix) ReadRawOld(string fileName)
//{
//    const int hdrSize = 1024;
//    var hdr = ReadHeader(fileName);
//    //(hdr as RawHeader).Width = 2048;
//    //(hdr as RawHeader).Height = 8192;
//    var dataLength = hdr.Width * hdr.Height;
//    int bytesPerLine = hdr.Width * 2;
//    int LinesPerFrame = hdr.LinesPerFrame;
//    byte[] bytes = new byte[bytesPerLine];
//    //            var pix = new float[dataLength]; //ALEX ARRAY POOL RENT and RETURN
//    var pix = ArrayPool<float>.Shared.Rent(dataLength); //ALEX ARRAY POOL RENT and RETURN
//    using (var f = File.OpenRead(fileName))
//    {
//        for (int ny = 0; ny < hdr.Height; ny++)
//        {
//            //var floatPixelLine = new Span<float>(pix, ny * hdr.Width, hdr.Width);
//            var floatPixelLine = pix.AsSpan(ny * hdr.Width, hdr.Width);
//            int skip = hdrSize + (ny / hdr.LinesPerFrame) * hdr.FrameHeaderSize;
//            int pixPos = ny * bytesPerLine;
//            f.Seek(skip + pixPos, SeekOrigin.Begin);
//            f.Read(bytes, 0, bytesPerLine);
//            var spU16 = bytes.ReCast<byte, UInt16>();
//            spU16.ShortsToFloats(floatPixelLine);
//        }
//    }
//    return (hdr.Width, hdr.Height, pix);
//}
//00001 /*******************************************************************
//00002   RTN SPLINE: Fits cubic smoothing spline to time series
//00003 
//00004   Derived from IMSL routines by Edward R Cook, Tree Ring Laboratory,
//00005   Lamont-Doherty Earth Observatory, Palisades, New York, USA
//00006 
//00007   Four routines combined into one by
//00008   Richard L Holmes, University of Arizona, Tucson, Arizona, USA
//00009   Modified copyright (C) 10 AUG 1998
//00010 
//00011 ********************************************************************/
//00012 
//00013 #include <stdio.h>
//00014 #include <stdlib.h>
//00015 #include <math.h>
//00016 
//00017 #define PI 3.1415926535897935
//00018 
//00019 
//00020 
//00021 /* DYNAMICALLY ALLOCATE A 2-D ARRAY */
//00022 /* Assumption:  nrows*ncols*element_size, rounded up to a multiple   */
//00023 /* of sizeof(long double), must fit in a long type.  If not, then    */
//00024 /* the "i += ..." step could overflow.                               */
//00025 
//00026 void** MATRIX(int nrows, int ncols, int first_row, int first_col, int element_size)
//00027 {
//00028     void** p;
//00029     int alignment;
//00030     long i;
//00031     
//00032     if(nrows< 1 || ncols< 1) return(NULL);
//00033     i = nrows*sizeof(void*);
//00034     /* align the addr of the data to be a multiple of sizeof(long double) */
//00035     alignment = i % sizeof(long double);
//00036     if(alignment != 0) alignment = sizeof(long double) - alignment;
//00037     i += nrows* ncols* element_size+alignment;
//00038     if((p = (void**) malloc((size_t) i)) != NULL)
//00039     {
//00040         /* compute the address of matrix[first_row][0] */
//00041         p[0] = (char*) (p+nrows)+alignment-first_col* element_size;
//00042         for(i = 1; i<nrows; i++)
//00043             /* compute the address of matrix[first_row+i][0] */
//00044             p[i] = (char*) (p[i - 1])+ncols* element_size;
//00045         /* compute the address of matrix[0][0] */
//00046         p -= first_row;
//00047     }
//00048     return(p);
//00049 }
//00050 
//00051 
//00052 
//00053 /* This function is called from SPLINE when :  */
//00054 /* 1. Series is too short to compute spline    */
//00055 /* 2. Matrix A is not positive definite        */
//00056 
//00057 void errorAction(int N, double* Y, float* ZF)
//00058 {
//00059     int k;
//00060     double ZN;
//00061 
//00062     ZN = 0.0;
//00063     for(k = 1; k <= N; k++)
//00064         ZN = ZN + Y[k];
//00065 
//00066     if (N > 0)
//00067     {
//00068         ZN = ZN/(float) N;
//00069         for(k = 1; k <= N; k++)
//00070             ZF[k - 1] = ZN;
//00071     }
//00072 
//00073     return;
//00074 }
//00075 
//00076 
//00077 
//00078 /* Function  SPLINE: Fits cubic smoothing spline to time series               */
//00079 /* Arguments:                                                                 */
//00080 /*                                                                            */
//00081 /* N:   Number of values in time series                                       */
//00082 /* Z:   Time series array to be modeled with spline                           */
//00083 /* ZF:  Computed cubic spline function array fit to time series               */
//00084 /* ZSP: Length (rigidity) of spline to be used to model series                */
//00085 /* ZPV: Portion of variance at wavelength ZSP contained in spline (0<ZPV<1)   */
//00086 /*                                                                            */
//00087 /* Arguments Z, ZF, ZSP and ZPV are single precision;                         */
//00088 /* computation is done entirely in double-precision arithmetic                */
//00089 
//00090 void SPLINE(int N, float* Z, float* ZF, float ZSP, float ZPV)
//00091 {
//00092     int i, j, k, l, m;
//00093     int NC, NC1, NCP1, IMNCP1, I1, I2, JM1, IW, KL, N1, K1;
//00094     double RSP, VPV, PD, RN, D1, D2, SUM;
//00095     double** A, * F, * Y, C1[5], C2[4];
//00096 
//00097     C1[0] =  0.0;
//00098     C1[1] =  1.0;
//00099     C1[2] = -4.0;
//00100     C1[3] =  6.0;
//00101     C1[4] = -2.0;
//00102     
//00103     C2[0] =  0.0;
//00104     C2[1] =  0.0;
//00105     C2[2] =  0.33333333333333;
//00106     C2[3] =  1.33333333333333;
//00107     
//00108     /* Allocate arrays to store intermeediate results */
//00109     A = (double**) MATRIX(N+1, 5, 0, 0, sizeof(double));
//00110     F = (double*) malloc((N+1)*sizeof(double));
//00111     Y = (double*) malloc((N+1)*sizeof(double));
//00112     if (A == NULL || F == NULL || Y == NULL)
//00113     {
//00114         printf("\nSPLINE >> Unable to allocate memory\n");
//00115         return;
//00116     }
//00117     
//00118     /* Check whether series is too short to compute spline */
//00119     if (N< 4)
//00120     {
//00121         errorAction(N, Y, ZF);
//00122         return;
//00123     }
//00124 
//00125     /* Copy time series into double precision array */
//00126     for(j = 1; j <= N; j++)
//00127         Y[j] = (double) Z[j - 1];
//00128         
//00129     /* Compute Lagrange multiplier, which defines frequency response of spline */
//00130     RSP = (double) ZSP;
//00131     VPV = (double) ZPV;
//00132     PD = ((1.0/(1.0-VPV)-1.0)*6.0* pow((cos(PI*2.0/RSP)-1.0),2.0))/(cos(PI*2.0/RSP)+2.0);
//00133     for(i = 1; i <= N-2; i++)
//00134         for(j = 1; j <= 3; j++)
//00135         {
//00136             A[i][j] = C1[j] + PD* C2[j];
//00137             A[i][4] = Y[i] + C1[4] * Y[i + 1] + Y[i + 2];
//00138         }
//00139 
//00140     A[1][1] = C2[1];
//00141     A[1][2] = C2[1];
//00142     A[2][1] = C2[1];
//00143     NC = 2;
//00144 
//00145     /* Begin LUDAPB */
//00146     RN = 1.0/((double)(N-2) * 16.0);
//00147     D1 = 1.0;
//00148     D2 = 0.0;
//00149     NCP1 = NC + 1;
//00150 
//00151     /* Initialize zero elements */
//00152     if (NC != 0)
//00153     {
//00154         for(i = 1; i <= NC; i++)
//00155             for(j = i; j <= NC; j++)
//00156             {
//00157                 k = NCP1 - j;
//00158                 A[i][k] = 0.0;
//00159             }
//00160     }
//00161 
//00162     /* i: row index of element being computed */
//00163     /* j: column index of element being computed */
//00164     /* l: row index of previously computed vector being used to compute inner product */
//00165     /* m: column index */
//00166     for(i = 1; i <= N-2; i++)
//00167     {
//00168         IMNCP1 = i - NCP1;
//00169         I1 = (1 < 1 - IMNCP1? 1 - IMNCP1: 1);
//00170         for(j = I1; j <= NCP1; j++)
//00171         {
//00172             l = IMNCP1 + j;
//00173             I2 = NCP1 - j;
//00174             SUM = A[i][j];
//00175             JM1 = j - 1;
//00176 
//00177             if (JM1 > 0)
//00178             {
//00179                 for(k = 1; k <= JM1; k++)
//00180                 {
//00181                     m = I2 + k;
//00182                     SUM = SUM - (A[i][k]* A[l][m]);
//00183                 }
//00184             }
//00185 
//00186             /* Matrix not positive definite */
//00187             if (j == NCP1)
//00188             {
//00189                 if (A[i][j]+SUM* RN <= A[i][j])
//00190                 {
//00191                     printf("\nSPLINE >> Matrix not positive definite\n");
//00192                     errorAction(N, Y, ZF);
//00193                     return;             
//00194                 }
//00195                 
//00196                 A[i][j] = 1.0/sqrt(SUM);
//00197                 
//00198                 /* Update determinant */
//00199                 D1 = D1* SUM;
//00200                 while (fabs(D1) > 1.0)
//00201                 {
//00202                     D1 = D1*0.0625;
//00203                     D2 = D2+4.0;
//00204                 }
//00205                 
//00206                 while (fabs(D1) <= 0.0625)
//00207                 {
//00208                     D1 = D1*16.0;
//00209                     D2 = D2-4.0;
//00210                 }
//00211                 continue;
//00212             }
//00213             A[i][j] = SUM* A[l][NCP1];                           
//00214         }
//00215     }
//00216     /* End LUDAPB */
//00217 
//00218     /* Begin LUELPB */
//00219     /* Solution LY = B */
//00220     NC1 = NC + 1;
//00221     IW = 0;
//00222     l = 0;
//00223 
//00224     for(i = 1; i <= N-2; i++)
//00225     {
//00226         SUM = A[i][4];
//00227         if (NC > 0) {
//00228             if (IW != 0) {
//00229                 l = l + 1;
//00230                 if (l > NC)
//00231                 {
//00232                     l = NC;
//00233                 }
//00234                 k = NC1 - l;
//00235                 KL = i - l;
//00236 
//00237                 for(j = k; j <= NC; j++)
//00238                 {
//00239                     SUM = SUM - (A[KL][4]* A[i][j]);
//00240                     KL = KL + 1;
//00241                 }
//00242             } else if (SUM != 0.0)
//00243             {
//00244                 IW = 1;
//00245             }
//00246         }
//00247         A[i][4] = SUM* A[i][NC1];
//00248     }
//00249 
//00250     /* Solution UX = Y */
//00251     A[N - 2][4] = A[N - 2][4]* A[N - 2][NC1];
//00252     N1 = N-2+1;
//00253 
//00254     for(i = 2; i <= N-2; i++)
//00255     {
//00256         k = N1 - i;
//00257         SUM = A[k][4];
//00258         if (NC > 0)
//00259         {
//00260             KL = k +1;
//00261             K1 = (N-2 < k+NC? N-2: k+NC);
//00262             l = 1;
//00263             for(j = KL; j <= K1; j++)
//00264             {
//00265                 SUM = SUM - A[j][4]* A[j][NC1-l];
//00266                 l = l + 1;
//00267             }
//00268         }
//00269         A[k][4] = SUM* A[k][NC1];
//00270     }
//00271     /* End LUELPB */
//00272 
//00273     /* Calculate Spline Curve */
//00274     for(i = 3; i <= N-2; i++)
//00275         F[i] = A[i - 2][4]+C1[4]* A[i - 1][4]+A[i][4];
//00276 
//00277     F[1] = A[1][4];
//00278     F[2] = C1[4]* A[1][4]+A[2][4];
//00279     F[N - 1] = A[N - 2 - 1][4]+C1[4]* A[N - 2][4];
//00280     F[N] = A[N - 2][4];
//00281 
//00282     for(i = 1; i <= N; i++)
//00283         ZF[i - 1] = Y[i] - F[i];
//00284 
//00285     return;
//00286 }
//00287 

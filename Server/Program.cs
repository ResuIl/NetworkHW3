using System.Net.Sockets;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

UdpClient server = new UdpClient(45678);
var remoteEP = new IPEndPoint(IPAddress.Any, 0);

while (true)
{
    var result = await server.ReceiveAsync();
    new Task(async () =>
    {
        remoteEP = result.RemoteEndPoint;
        while (true)
        {
            var imgByte = ImageToByte(TakeScreenShot());
            var chunks = imgByte.Chunk(ushort.MaxValue - 29);
            foreach (var chunk in chunks)
                await server.SendAsync(chunk, chunk.Length, remoteEP);
        }

    }).Start();
}

byte[] ImageToByte(Image img)
{
    using (MemoryStream ms = new MemoryStream())
    {
        img.Save(ms, ImageFormat.Jpeg);
        return ms.ToArray();
    }
}

Image TakeScreenShot()
{
    Rectangle bounds = Screen.PrimaryScreen.Bounds;
    Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
    using (Graphics graphics = Graphics.FromImage(bitmap))
    {
        graphics.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size);
    }
    return bitmap;
}
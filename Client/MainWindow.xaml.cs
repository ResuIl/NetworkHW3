using System.Net.Sockets;
using System.Net;
using System.Windows;
using System.Collections.Generic;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Client;

public partial class MainWindow : Window
{
    UdpClient client;
    IPEndPoint remoteEP;
    bool isPlaying = false;
    public MainWindow()
    {
        InitializeComponent();
        client = new UdpClient();
        remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 45678);
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        if (!isPlaying)
        {
            isPlaying = true;
            var buffer = new byte[ushort.MaxValue - 29];
            await client.SendAsync(buffer, buffer.Length, remoteEP);
            var list = new List<byte>();
            var maxlen = buffer.Length;
            var len = 0;
            while (true)
            {
                do
                {
                    try
                    {
                        var result = await client.ReceiveAsync();
                        buffer = result.Buffer;
                        len = buffer.Length;
                        list.AddRange(buffer);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                } while (len == maxlen);
                var image = LoadImage(list.ToArray());
                if (image != null)
                    Player.Source = image;

                list.Clear();
            }
        } 
    }
    private static BitmapImage? LoadImage(byte[] imageData)
    {
        var image = new BitmapImage();
        image.BeginInit();
        image.StreamSource = new MemoryStream(imageData);
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.EndInit();
        return image;
    }
}

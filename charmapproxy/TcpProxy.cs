using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace cat.srigau.charmapproxy
{
  class TcpProxy : IProxy
  {
    public async Task Start(string remoteServerIp, ushort remoteServerPort, ushort localPort, string localIp, byte[] byteMapClient2Setver, byte[] byteMapSetver2Client)
    {
      //var clients = new ConcurrentDictionary<IPEndPoint, TcpClient>();

      IPAddress localIpAddress = string.IsNullOrEmpty(localIp) ? IPAddress.IPv6Any : IPAddress.Parse(localIp);
      var server = new System.Net.Sockets.TcpListener(new IPEndPoint(localIpAddress, localPort));
      server.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
      server.Start();

      Console.WriteLine($"TCP proxy started {localPort} -> {remoteServerIp}|{remoteServerPort}");
      while (true)
      {

        try
        {
          var remoteClient = await server.AcceptTcpClientAsync();
          remoteClient.NoDelay = true;
          var ips = await Dns.GetHostAddressesAsync(remoteServerIp);

          new TcpClient(remoteClient, new IPEndPoint(ips.First(), remoteServerPort), byteMapClient2Setver, byteMapSetver2Client);


        }
        catch (Exception ex)
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine(ex);
          Console.ResetColor();
        }

      }
    }
  }

  class TcpClient
  {
    private System.Net.Sockets.TcpClient _remoteClient;
    private IPEndPoint _clientEndpoint;
    private IPEndPoint _remoteServer;
    private byte[] byteMapClient2Setver;
    private byte[] byteMapSetver2Client;
    public TcpClient(System.Net.Sockets.TcpClient remoteClient, IPEndPoint remoteServer, byte[] byteMapClient2SetverPar, byte[] byteMapSetver2ClientPar)
    {
      byteMapClient2Setver = new byte[byteMapClient2SetverPar.Length];  byteMapClient2SetverPar.CopyTo(byteMapClient2Setver,0);
      byteMapSetver2Client = new byte[byteMapSetver2ClientPar.Length];  byteMapSetver2ClientPar.CopyTo(byteMapSetver2Client,0);
      if ( byteMapClient2Setver.Length != 256) {
        byteMapClient2Setver = new byte[265];
        for (int i = 0; i < 256; i++)
        {
          byteMapClient2Setver[i] = (byte)i;
        }
      }
      if (byteMapSetver2Client.Length != 256)
      {
        byteMapSetver2Client = new byte[265];
        for (int i = 0; i < 256; i++)
        {
          byteMapSetver2Client[i] = (byte)i;
        }
      }
      _remoteClient = remoteClient;
      _remoteServer = remoteServer;
      client.NoDelay = true;
      _clientEndpoint = (IPEndPoint)_remoteClient.Client.RemoteEndPoint;
      Console.WriteLine($"Established {_clientEndpoint} => {remoteServer}");
      Run();
    }


    public System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();



    private void Run()
    {

      Task.Run(async () =>
      {
        try
        {
          CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
          CancellationToken cancellationToken = cancellationTokenSource.Token;
          using (_remoteClient)
          using (client)
          {
            await client.ConnectAsync(_remoteServer.Address, _remoteServer.Port);
            var serverStream = client.GetStream();
            var remoteStream = _remoteClient.GetStream();
            CancellationTokenSource cancellationTokenSource2 = new CancellationTokenSource();
            CancellationToken cancellationToken2 = cancellationTokenSource.Token;

            // await Task.WhenAny(remoteStream.CopyToAsync(serverStream), serverStream.CopyToAsync(remoteStream));
            await Task.WhenAny(StreamCopy.CopyToAsync3(remoteStream,serverStream,1536, byteMapClient2Setver, cancellationToken), StreamCopy.CopyToAsync3(serverStream,remoteStream,1536, byteMapSetver2Client, cancellationToken2) );

          }
        }
        catch (Exception) { }
        finally
        {
          Console.WriteLine($"Closed {_clientEndpoint} => {_remoteServer}");
          _remoteClient = null;
        }
      });
    }


  }
}

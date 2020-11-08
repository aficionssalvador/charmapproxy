using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace cat.srigau.charmapproxy
{
  class Program
  {
    static private byte[] byteMapClient2Setver;
    static private byte[] byteMapSetver2Client;
    static void Main(string[] args)
    {
      try
      {
        // llegir configuracio
        var configJson = System.IO.File.ReadAllText("config.json");
        // json a objete
        var configs = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ProxyConfig>>(configJson);
        
        byteMapClient2Setver = new byte[265];
        for (int i = 0; i < 256; i++)
        {
          byteMapClient2Setver[i] = (byte)i;
        }
        byteMapSetver2Client = new byte[265];
        for (int i = 0; i < 256; i++)
        {
          byteMapSetver2Client[i] = (byte)i;
        }

        Task.WhenAll(configs.Select(c =>
        {
          for (int  i=0; i < c.Value.client2server.Length; i++)
          {
            byteMapClient2Setver[c.Value.client2server[i].s] = (byte)c.Value.client2server[i].s;
          }
          for (int i = 0; i < c.Value.server2client.Length; i++)
          {
            byteMapSetver2Client[c.Value.server2client[i].s] = (byte)c.Value.server2client[i].s;
          }
          if (c.Value.protocol == "tcp")
          {
            try
            {
              var proxy = new TcpProxy();
              return proxy.Start(c.Value.forwardIp, c.Value.forwardPort, c.Value.localPort, c.Value.localIp, byteMapClient2Setver, byteMapSetver2Client);
            }
            catch (Exception ex)
            {
              Console.WriteLine($"Failed to start {c.Key} : {ex.Message}");
              throw ex;
            }
          }
          else
          {
            return Task.FromException(new InvalidOperationException($"procotol not supported {c.Value.protocol}"));
          }
        })).Wait();




      }
      catch (Exception ex)
      {
        Console.WriteLine($"An error occured : {ex}");
      }
    }
  }

  public class ProxyConfig
  {
    public string protocol { get; set; }
    public ushort localPort { get; set; }
    public string localIp { get; set; }
    public string forwardIp { get; set; }
    public ushort forwardPort { get; set; }
    public Map1Byte[] client2server { get; set; }
    public Map1Byte[] server2client { get; set; }
  }

  public class Map1Byte
  {
  public int s;
  public int d;
  }
  interface IProxy
  {
    Task Start(string remoteServerIp, ushort remoteServerPort, ushort localPort, string localIp , byte[] byteMapClient2Setver, byte[] byteMapSetver2Client);
  }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace cat.srigau.charmapproxy
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        // llegir configuracio
        var configJson = System.IO.File.ReadAllText("config.json");
        // json a objete
        var configs = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, ProxyConfig>>(configJson);


        Task.WhenAll(configs.Select(c =>
        {
          /*
          for(byte b = 0; b <=255; b++)
          {
            c.Value.client2server[b] = b;
          }
          if (c.Value.client2server.Count > 0)
          {
            foreach

          }*/
          if (c.Value.protocol == "tcp")
          {
            try
            {
              var proxy = new TcpProxy();
              return proxy.Start(c.Value.forwardIp, c.Value.forwardPort, c.Value.localPort, c.Value.localIp);
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
    //public List<{Byte, Byte }> client2server { get; set; }
    //public List<Byte> server2client { get; set; }
  }
  interface IProxy
  {
    Task Start(string remoteServerIp, ushort remoteServerPort, ushort localPort, string localIp = null);
  }
}
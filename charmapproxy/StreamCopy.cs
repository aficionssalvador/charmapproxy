using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace cat.srigau.charmapproxy
{
  static class StreamCopy
  {
    const int _DefaultBufferSize = 1536;
    const byte IAC = 255;
    //static bool b3;
    //static bool b4;

    public static async Task CopyToAsync3(
      this Stream source,
      Stream destination,
      int bufferSize,
      byte[] mybytemap,
      CancellationToken cancellationToken)
    {
      if (0 == bufferSize)
        bufferSize = _DefaultBufferSize;
      var buffer = new byte[bufferSize];
      var buffermaped = new byte[bufferSize];
      var totalBytesCopied = 0L;
      var bytesRead = -1;
      bool IACMode = false;
      while (0 != bytesRead && !cancellationToken.IsCancellationRequested)
      //while (0 != bytesRead)
      {
        bytesRead = await source.ReadAsync(buffer, 0, buffer.Length);
        //bytesRead = source.Read(buffer, 0, buffer.Length);
        if (0 == bytesRead || cancellationToken.IsCancellationRequested)
          break;
        //if (0 == bytesRead)
        //  break;
        //IACMode = b3;
        for (int i = 0; i < bytesRead; i++)
        {
          if (buffer[i] == IAC)
          {
            IACMode = !(IACMode); //b3 = IACMode;
          }
          if (IACMode)
          {
            buffermaped[i] = buffer[i];
          }
          else
          {
            buffermaped[i] = mybytemap[(int)buffer[i]];
          }
        }
        await destination.WriteAsync(buffermaped, 0, bytesRead);
        //destination.Write(buffer, 0, bytesRead);
        totalBytesCopied += bytesRead;
      }
      cancellationToken.ThrowIfCancellationRequested();
    }

    public static async Task CopyToAsync4(
      this Stream source,
      Stream destination,
      int bufferSize,
      byte[] mybytemap,
      CancellationToken cancellationToken)
    {
      if (0 == bufferSize)
        bufferSize = _DefaultBufferSize;
      var buffer = new byte[bufferSize];
      var buffermaped = new byte[bufferSize];
      var totalBytesCopied = 0L;
      var bytesRead = -1;
      bool IACMode = false;
      while (0 != bytesRead && !cancellationToken.IsCancellationRequested)
      //while (0 != bytesRead)
      {
        bytesRead = await source.ReadAsync(buffer, 0, buffer.Length);
        //bytesRead = source.Read(buffer, 0, buffer.Length);
        if (0 == bytesRead || cancellationToken.IsCancellationRequested)
          break;
        //if (0 == bytesRead)
        //  break;
        //IACMode = b4;
        for (int i = 0; i < bytesRead; i++)
        {
          if (buffer[i] == IAC)
          {
            IACMode = !(IACMode); //b4 = IACMode;
          }
          if (IACMode)
          {
            buffermaped[i] = buffer[i];
          }
          else
          {
            buffermaped[i] = mybytemap[(int)buffer[i]];
          }
        }
        await destination.WriteAsync(buffermaped, 0, bytesRead);
        //destination.Write(buffer, 0, bytesRead);
        totalBytesCopied += bytesRead;
      }
      cancellationToken.ThrowIfCancellationRequested();
    }
  }

}

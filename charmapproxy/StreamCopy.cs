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
      while (0 != bytesRead && !cancellationToken.IsCancellationRequested)
      //while (0 != bytesRead)
      {
        bytesRead = await source.ReadAsync(buffer, 0, buffer.Length);
        //bytesRead = source.Read(buffer, 0, buffer.Length);
        if (0 == bytesRead || cancellationToken.IsCancellationRequested)
          break;
        //if (0 == bytesRead)
        //  break;
        for(int i = 0; i < bytesRead; i++)
          buffermaped[i] = mybytemap[(int)buffer[i]];
        await destination.WriteAsync(buffermaped, 0, bytesRead);
        //destination.Write(buffer, 0, bytesRead);
        totalBytesCopied += bytesRead;
      }
      cancellationToken.ThrowIfCancellationRequested();
    }
  }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TotalCommander.Common
{
    public static class CopyExtension
    {
        public static async Task CopyToAsync(this Stream source, Stream destination, CancellationToken cancellationToken, IProgress<StreamCopyProgress> progress)
        {
            int i = 0;
            long bytesRead = 0;
            long totalBytesRead = 0;
            var buffers = new[] { new byte[0x1000], new byte[0x1000] };

            Task writeTask = null;
            while (true)
            {
                var readTask = source.ReadAsync(buffers[i], 0, buffers[i].Length);

                if (writeTask != null)
                {
                    await Task.WhenAll(readTask, writeTask).ConfigureAwait(false);

                    cancellationToken.ThrowIfCancellationRequested();
                    progress.Report(new StreamCopyProgress(totalBytesRead * 100/source.Length));
                }

                bytesRead = await readTask.ConfigureAwait(false);
                totalBytesRead += bytesRead;

                if (bytesRead == 0) break;

                writeTask = destination.WriteAsync(buffers[i], 0, (int)bytesRead);
                i ^= 1;
            }
        }
    }

    public class StreamCopyProgress
    {
        public StreamCopyProgress()
        {
            Timestamp = DateTime.UtcNow;
        }

        public StreamCopyProgress(long currentBytes)
            : this()
        {
            CurrentBytes = currentBytes;
        }

        public StreamCopyProgress(StreamCopyProgress copy)
            : this()
        {
            CurrentBytes = copy.CurrentBytes;
            UserState = copy.UserState;
            Timestamp = copy.Timestamp;
        }

        public long CurrentBytes { get; set; }
        public object UserState { get; set; }
        public DateTime Timestamp { get; set; }
    }
}

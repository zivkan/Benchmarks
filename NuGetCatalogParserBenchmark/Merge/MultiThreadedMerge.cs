using Newtonsoft.Json;
using NuGetCatalogParserBenchmark.NuGetModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace NuGetCatalogParserBenchmark.Merge
{
    public class MultiThreadedMerge
    {
        private readonly string _cachePath;
        private readonly int _maxConcurrent;
        private readonly JsonSerializer _jsonSerializer;

        public MultiThreadedMerge(string cachePath, int maxConcurrent)
        {
            _cachePath = cachePath;
            _maxConcurrent = maxConcurrent;
            _jsonSerializer = new JsonSerializer();
        }

        public Dictionary<string, List<string>> Go()
        {
            var index = Utility.Read<CatalogIndex>(Path.Combine(_cachePath, "index.json"), _jsonSerializer);
            int maxConcurrent = Math.Min(_maxConcurrent, index.Items.Count);
            var queues = new List<List<string>>(maxConcurrent);
            for (int processor = 0; processor < maxConcurrent; processor++)
            {
                queues.Add(new List<string>(index.Items.Count / maxConcurrent + 1));
            }

            for (int page = 0, processor = 0; page < index.Items.Count; page++)
            {
                queues[processor].Add(index.Items[page].Id);
                processor = (processor + 1) % maxConcurrent;
            }

            var cancel = new CancellationTokenSource();
            var work = new List<WorkItem>(maxConcurrent);
            var handles = new List<WaitHandle>(maxConcurrent);
            for (int t = 0; t < maxConcurrent; t++)
            {
                var workItem = new WorkItem
                {
                    Queue = queues[t],
                    Finished = new ManualResetEvent(false),
                    Thread = new Thread(ProcessQueue),
                    Cancel = cancel.Token
                };
                workItem.Thread.Start(workItem);
                work.Add(workItem);
                handles.Add(workItem.Finished);
            }

            Dictionary<string, Dictionary<string, (DateTime, bool)>> intermediate = null;
            while (work.Count > 0)
            {
                var finishedIndex = WaitHandle.WaitAny(handles.ToArray());
                var finished = work[finishedIndex];
                Debug.Assert(!finished.Thread.IsAlive);
                work.RemoveAt(finishedIndex);
                handles.RemoveAt(finishedIndex);

                if (finished.Exception != null)
                {
                    cancel.Cancel();
                    throw new Exception("Thread finished with error", finished.Exception);
                }

                if (intermediate == null)
                {
                    intermediate = finished.Result;
                }
                else
                {
                    Utility.Merge(intermediate, finished.Result);
                }
            }

            var result = Utility.ConvertIntermediateResultToFinal(intermediate);
            return result;
        }

        private void ProcessQueue(object obj)
        {
            var workItem = (WorkItem)obj;
            try
            {
                var intermediate = new Dictionary<string, Dictionary<string, (DateTime timeStamp, bool deleted)>>();

                foreach (var pageUri in workItem.Queue)
                {
                    if (workItem.Cancel.IsCancellationRequested)
                    {
                        break;
                    }

                    var uri = new Uri(pageUri);
                    var fileName = Path.Combine(_cachePath, uri.Segments[uri.Segments.Length - 1]);
                    var page = Utility.Read<CatalogPage>(fileName, _jsonSerializer);
                    var src = Utility.ConvertCatalogPageItemsToIntermediate(page.Items);
                    Utility.Merge(intermediate, src);
                }

                workItem.Result = intermediate;
            }
            catch (Exception e)
            {
                workItem.Exception = e;
            }
            finally
            {
                workItem.Finished.Set();
            }
        }

        private class WorkItem
        {
            public List<string> Queue { get; set; }
            public ManualResetEvent Finished { get; set; }
            public Thread Thread { get; set; }
            public CancellationToken Cancel { get; set; }

            public Dictionary<string, Dictionary<string, (DateTime timeSpan, bool deleted)>> Result { get; set; }
            public Exception Exception { get; set; }
        }
    }
}

//REF: https://michaelscodingspot.com/c-job-queues/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BITPay.DBL
{
    public class LogUtil
    {
        private static Queue<LogJob> _jobs = new Queue<LogJob>();
        private static bool _delegateQueuedOrRunning = false;

        static LogUtil()
        {

        }

        public static void Infor(string? logFile, string functioName, string message)
        {
            WriteLog(logFile, functioName, new Exception(message), false);
        }

        public static void Error(string? logFile, string? functioName, Exception ex)
        {
            WriteLog(logFile, functioName, ex);
        }

        public static void Error(string? logFile, string? functioName, string? message)
        {
            WriteLog(logFile, functioName, new Exception(message));
        }

        private static void WriteLog(string? logFile, string? functioName, Exception ex, bool isError = true)
        {
            lock (_jobs)
            {
                _jobs.Enqueue(new LogJob(logFile!, functioName!, ex, isError));
                if (!_delegateQueuedOrRunning)
                {
                    _delegateQueuedOrRunning = true;
                    ThreadPool.UnsafeQueueUserWorkItem(ProcessQueuedItems!, null);
                }
            }            
        }

        private static void ProcessQueuedItems(object ignored)
        {
            while (true)
            {
                LogJob item;
                lock (_jobs)
                {
                    if (_jobs.Count == 0)
                    {
                        _delegateQueuedOrRunning = false;
                        break;
                    }

                    item = _jobs.Dequeue();
                }

                try
                {
                    //do job
                    ProcessLogJob(item);
                }
                catch
                {
                    ThreadPool.UnsafeQueueUserWorkItem(ProcessQueuedItems!, null);
                    throw;
                }
            }
        }

        private static void ProcessLogJob(LogJob job)
        {
            try
            {
                //--- Create folder if it does not exists
                var fi = new FileInfo(job.LogFile);
                if (!fi.Directory.Exists)
                    fi.Directory.Create();

                //--- Delete log if it more than 500Kb
                if (fi.Exists)
                {
                    if ((fi.Length / 1000) > 100)
                        fi.Delete();
                }

                //--- Create stream writter
                StreamWriter stream = new StreamWriter(job.LogFile, true);
                stream.WriteLine(string.Format("{0}|{1:dd-MMM-yyyy HH:mm:ss}|{2}|{3}",
                    job.IsError ? "ERROR" : "INFOR",
                    DateTime.Now,
                    job.ModuleName,
                    job.IsError ? job.Exception.ToString() : job.Exception.Message));
                stream.Close();
            }
            catch (Exception) { }
        }

        public class LogJob
        {
            public string LogFile { get; set; }
            public string ModuleName { get; set; }
            public Exception Exception { get; set; }
            public bool IsError { get; set; }

            public LogJob(string logFile, string moduleName, Exception exception, bool isError)
            {
                LogFile = logFile;
                ModuleName = moduleName;
                Exception = exception;
                IsError = isError;
            }
        }
    }
}

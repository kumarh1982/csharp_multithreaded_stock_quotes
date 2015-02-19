using System;				
using System.Net;           //for using WebClient
using System.Diagnostics;   //for using StopWatch
using System.Threading;     //for ManualResetEvent

namespace MultithreadedStockQuotes
{
    class StockQueryEngineQueryTask
    {
        private ManualResetEvent m_done_flag;
        private Stopwatch m_watch;

        public StockQueryEngineQueryTask(ManualResetEvent done_flag)
        {
            m_done_flag = done_flag;
            m_watch = new Stopwatch();
        }

        public void ThreadPoolCallback(Object threadContext)
        {
            StockQueryEngineTaskInfo thread_info = (StockQueryEngineTaskInfo)threadContext;

            m_watch.Start();
            thread_info.Quote = QuerySymbol(thread_info.Symbol, thread_info.BaseUrl, thread_info.UrlFunction);
            m_watch.Stop();
            thread_info.ExecutionTime = m_watch.ElapsedMilliseconds;
            m_done_flag.Set();
        }

        public static string QuerySymbol(string symbol, string baseUrl, string function)
        {
            string csvData;

            using (WebClient web = new WebClient())
            {
                string url = baseUrl + symbol + function;
                csvData = web.DownloadString(url);
                string[] args = csvData.Split(',');
                return args[2];
            }
        }
    }
}
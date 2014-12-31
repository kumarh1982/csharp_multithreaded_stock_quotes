using System;
using System.Net;           //for using WebClient
using System.Threading;     //for using ThreadPool
using System.Diagnostics;   //for using StopWatch

namespace MultithreadedStockQuotes
{
    class StockQueryEngineTaskInfo
    {
        private int m_thread_index;
        private string m_symbol;
        private string m_quote;
        private long m_execution_time;

        public StockQueryEngineTaskInfo(int thread_index, string symbol)
        {
            m_thread_index = thread_index;
            m_symbol = symbol;
            m_execution_time = 0;
        }
        public int ThreadIndex
        {
            get { return m_thread_index; }
            set { m_thread_index = value; }
        }
        public string Symbol
        {
            get { return m_symbol; }
            set { m_symbol = value; }
        }

        public string Quote
        {
            get { return m_quote; }
            set { m_quote = value; }
        }

        public long ExecutionTime
        {
            get { return m_execution_time; }
            set { m_execution_time = value; }
        }
    }

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
            thread_info.Quote = QuerySymbol(thread_info.Symbol);
            m_watch.Stop();
            thread_info.ExecutionTime = m_watch.ElapsedMilliseconds;
            m_done_flag.Set();
        }

        public static string QuerySymbol(string symbol)
        {
            string csvData;

            using (WebClient web = new WebClient())
            {
                string url = "http://finance.yahoo.com/d/quotes.csv?s=" + symbol + "&f=snbaopl1";
                csvData = web.DownloadString(url);
                string[] args = csvData.Split(',');
                return args[2];
            }
        }
    }
    
    class StockQueryEngine
    {
        private System.Collections.Generic.List<string> m_symbols;
        private bool m_symbols_loaded;

        private long m_latest_execution_time;
        private Stopwatch m_watch;
        
        private ManualResetEvent[] m_task_done_flags;
        private StockQueryEngineQueryTask[] m_tasks;
        private StockQueryEngineTaskInfo[] m_task_infos;

        public StockQueryEngine()
        {
            m_symbols = new System.Collections.Generic.List<string>();
            m_symbols_loaded = false;
            m_latest_execution_time = 0;
            m_watch = new Stopwatch();
        }

        public void SetMaxNumberOfThreads (int n)
        {
            if( n < System.Environment.ProcessorCount)
            {
                n = System.Environment.ProcessorCount;
            }

            ThreadPool.SetMaxThreads(n, n);
        }

        public int GetMaxNumberOfThreads()
        {
            int worker_threads=0;
            int completion_ports=0;
            ThreadPool.GetMaxThreads(out worker_threads, out completion_ports);
            return worker_threads;
        }

        public long ExecutionTime
        {
            get { return m_latest_execution_time; }
        }

        public bool LoadSymbolsFromFile(string filename)
        {
            using (System.IO.StreamReader file = new System.IO.StreamReader(filename))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    m_symbols.Add(line);
                }
            }

            m_symbols_loaded = true;
            return true;
        }

        public StockQueryEngineTaskInfo[] Execute()
        {
            if( m_symbols_loaded == false )
            {
                return null;
            }

            m_watch.Start();

            int count_symbols = m_symbols.Count;

            m_task_done_flags = new ManualResetEvent[count_symbols];
            m_tasks = new StockQueryEngineQueryTask[count_symbols];
            m_task_infos = new StockQueryEngineTaskInfo[count_symbols];

            for (int i = 0; i < count_symbols; i++ )
            {
                m_task_done_flags[i] = new ManualResetEvent(false);
                m_tasks[i] = new StockQueryEngineQueryTask(m_task_done_flags[i]);

                m_task_infos[i] = new StockQueryEngineTaskInfo(i, m_symbols[i]);
                ThreadPool.QueueUserWorkItem( new WaitCallback(m_tasks[i].ThreadPoolCallback), m_task_infos[i] );
            }

            WaitHandle.WaitAll(m_task_done_flags);

            m_watch.Stop();
            m_latest_execution_time = m_watch.ElapsedMilliseconds;

            return m_task_infos;
        }
    }
}
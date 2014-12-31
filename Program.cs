using System;
using System.Text;

namespace MultithreadedStockQuotes
{
    class Program
    {
        static void Main(string[] args)
        {
            string input_file = args[1];

            StockQueryEngine engine = new StockQueryEngine();

            engine.SetMaxNumberOfThreads(System.Environment.ProcessorCount);
            engine.LoadSymbolsFromFile(input_file);

            StockQueryEngineTaskInfo[] results = engine.Execute();

            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("Entire execution took " + engine.ExecutionTime + " miliseconds");
            Console.WriteLine("");
            Console.WriteLine("Number of threads fired : " + engine.GetMaxNumberOfThreads());
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("");

            foreach(StockQueryEngineTaskInfo info in results)
            {
                Console.WriteLine(info.ThreadIndex + " : " + info.Symbol + " " + info.Quote + " , in " + info.ExecutionTime + " miliseconds");
            }

            Console.WriteLine("");
            Console.WriteLine("----------------------------------------------------------------------------");
        }
    }
}
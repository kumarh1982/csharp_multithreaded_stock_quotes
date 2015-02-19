using System;
using System.Text;

namespace MultithreadedStockQuotes
{
    class Program
    {
        static void Main(string[] args)
        {
            int num_arguments = args.Length;

            if (num_arguments == 0)
            {
                Console.WriteLine("Please provide a symbols file as argument." + System.Environment.NewLine);
                return;
            }

            string input_file = input_file = args[0];
			
            StockQueryEngine engine = new StockQueryEngine();

            engine.SetMaxNumberOfThreads(System.Environment.ProcessorCount);

            if (engine.LoadSymbolsFromFile(input_file) == false)
            {
               OnEngineError(ref engine);
               return;
            }

            if (StockQueryEngine.CheckForInternetConnection() == true)
            {

                StockQueryEngineTaskInfo[] results = null;
                results = engine.Execute();

                if( results == null )
                {
                    OnEngineError(ref engine);
                    return;
                }

                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("Entire execution took " + engine.ExecutionTime + " miliseconds");
                Console.WriteLine("");
                Console.WriteLine("Number of threads fired : " + engine.GetMaxNumberOfThreads());
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("");

                foreach (StockQueryEngineTaskInfo info in results)
                {
                    Console.WriteLine(info.ThreadIndex + " : " + info.Symbol + " " + info.Quote + " , in " + info.ExecutionTime + " miliseconds");
                }

                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------------------");
            }
            else
            {
                Console.WriteLine("No internet connection.");
            }
        }

        static void OnEngineError(ref StockQueryEngine engine)
        {
            Console.WriteLine("Error occured during engine initalisation : " + System.Environment.NewLine);
            Console.WriteLine(engine.GetLastError());
        }
		
    }
}
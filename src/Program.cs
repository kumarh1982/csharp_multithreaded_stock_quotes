using System;
using System.Text;

namespace MultithreadedStockQuotes
{
    class Program
    {
        static int Main(string[] args)
        {
            int num_arguments = args.Length;

            if (num_arguments == 0)
            {
                Console.WriteLine("Please provide a symbols file as argument." + System.Environment.NewLine);
                return 1;
            }

            string input_file = args[0];
			
            StockQueryEngine engine = new StockQueryEngine();

            engine.SetMaxNumberOfThreads(System.Environment.ProcessorCount);

            if (engine.LoadSymbolsFromFile(input_file) == false)
            {
               OnEngineError(ref engine);
               return 2;
            }

            if (StockQueryEngine.CheckForInternetConnection() == true)
            {

                StockQueryEngineTaskInfo[] results = null;
                if ( engine.Execute() )
                {
                    results = engine.Join();
                }
                else
                {
                    OnEngineError(ref engine);
                    return 3;
                }

                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("Entire execution took " + engine.ExecutionTime + " miliseconds");
                Console.WriteLine("");
                Console.WriteLine("Number of threads fired : " + engine.GetMaxNumberOfThreads());
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("");

                foreach (StockQueryEngineTaskInfo info in results)
                {
                    Console.Write(info.ThreadIndex + " : " + info.Symbol + " " + info.Bid + " , in " + info.ExecutionTime + " miliseconds");
                    Console.Write(Environment.NewLine);
                    Console.Write(Environment.NewLine);
                }

                Console.WriteLine("");
                Console.WriteLine("----------------------------------------------------------------------------");

                if (num_arguments == 2)
                {
                    engine.OutputAsCSV(args[1]);
                }

                return 0;
            }
            else
            {
                Console.WriteLine("No internet connection.");
                return 4;
            }
        }

        static void OnEngineError(ref StockQueryEngine engine)
        {
            Console.WriteLine("Error occured during engine initalisation : " + System.Environment.NewLine);
            Console.WriteLine(engine.GetLastError());
        }
		
    }
}
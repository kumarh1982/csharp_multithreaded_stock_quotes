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

            string input_file = "";

            //    The reason for the check below is Mono doesn`t pass program
            //    name as the first argument.
			if( StockQueryEngine.IsRunningUnderLinux() )
			{
				input_file = args[0]; // Linux Systems
			}
			else
			{
				input_file = args[1]; // Windows Systems
			}
			
            StockQueryEngine engine = new StockQueryEngine();

            engine.SetMaxNumberOfThreads(System.Environment.ProcessorCount);

            if (engine.LoadSymbolsFromFile(input_file) == false)
            {
                Console.WriteLine("Error occured during engine initalisation : " + System.Environment.NewLine);
                Console.WriteLine(engine.GetLastError());
                return;
            }

            if (StockQueryEngine.CheckForInternetConnection() == true)
            {

                StockQueryEngineTaskInfo[] results = engine.Execute();

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
		
    }
}
Multithreaded stock quotes

Licence : All samples are "Public Domain" code 
http://en.wikipedia.org/wiki/Public_domain_software

===========================================================================

This is a multithreaded stock quotes querying code. It loads symbols 
from a text file and gets the quotes by using Yahoo Finance API
by using System.Threading.ThreadPool.

How to build :

a) Windows systems : You can use Visual Studio 2013 solution file provided. 
Tested with Visual Studio 2013 Express Edition.

	1) Open Visual Studio developer command line
	2) Change your directory to build/windows
	3) Type "msbuild MultithreadedStockQuotes.csproj" and then press enter
	
	Alternatively you can use Visual Studo

b) Linux Systems : You can build against Mono. This has been tested against Debian Wheezy :

	Prerequisites : You need to install Mono for your platform. For Debian systems :
				
				sudo apt-get install mono-complete
		
	To build : 
	
			1) Change your directory to build/linux
			2) Type "make" and press enter
	
				
	How to execute it : After a successful build, the executable is created under 
	root directory. After changing your directory to the executable`s location :
		
				mono multithreaded_stock_quotes.exe symbols.txt
	

Example code :

            StockQueryEngine engine = new StockQueryEngine();

            engine.SetMaxNumberOfThreads(System.Environment.ProcessorCount);
            engine.LoadSymbolsFromFile("symbols.txt");

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

Example output : 

			----------------------------------------------------------------------------
			Entire execution took 2548 miliseconds

			Number of threads fired : 8
			----------------------------------------------------------------------------

			0 : AAPL 110.56 , in 1128 miliseconds
			1 : INTC 36.30 , in 1343 miliseconds
			2 : MSFT 46.45 , in 1513 miliseconds
			3 : GOOGL 530.70 , in 910 miliseconds
			4 : QCOM 74.40 , in 1321 miliseconds
			5 : QQQ 103.35 , in 1119 miliseconds
			6 : BBRY 10.91 , in 896 miliseconds
			7 : SIRI 3.51 , in 1549 miliseconds
			8 : ZNGA 2.66 , in 813 miliseconds
			9 : ARCP 8.97 , in 865 miliseconds
			10 : XIV 31.42 , in 849 miliseconds
			11 : FOXA 38.42 , in 793 miliseconds
			12 : TVIX 2.72 , in 808 miliseconds
			13 : YHOO 50.49 , in 824 miliseconds
			14 : HBAN 10.50 , in 828 miliseconds
			15 : AAL 53.58 , in 823 miliseconds
			16 : FTR 6.65 , in 836 miliseconds

			----------------------------------------------------------------------------
using System.Diagnostics;

namespace FolderSync
{
    internal class Program
    {
        static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            if (args.Length != 2)
            {
                Console.WriteLine("Usage: FolderSync <source> <destination>");
                return 1;
            }

            string sourcePath = args[0];
            string targetPath = args[1];

            Console.WriteLine($"Synchronization {sourcePath} to {targetPath}");

            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                Synchronizator synchronizator = new Synchronizator();
                synchronizator.Synchronize(sourcePath, targetPath);

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 2;
            }
            finally { 
                watch.Stop();
                Console.WriteLine($"Synchronization complete in {watch.Elapsed} sec.");
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Exception:");

            if (e.ExceptionObject is Exception exception)
                Console.WriteLine(exception.Message);
            else
                Console.WriteLine(e.ExceptionObject.ToString());
        }

        private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine("Exception:");
            Console.WriteLine(e.Exception.Message);
            e.SetObserved();
        }
    }
}

using System.Diagnostics;

namespace FolderSync
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: FolderSync <source> <destination>");
                return;
            }

            string sourcePath = args[0];
            string targetPath = args[1];

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            Console.WriteLine($"Syncing {sourcePath} to {targetPath}");

            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                Synchronizator synchronizator = new Synchronizator();
                synchronizator.Synchronize(sourcePath, targetPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally { 
                watch.Stop();
                Console.WriteLine($"Synchronization complete in {watch.Elapsed} sec.");
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Exception:");

            if (e.ExceptionObject is Exception)
                Console.WriteLine(((Exception)e.ExceptionObject).Message);
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

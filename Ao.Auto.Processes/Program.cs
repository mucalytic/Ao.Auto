using System.Reactive.Linq;
using System.Diagnostics;
using System;

namespace Ao.Auto.Processes
{
    public class Program
    {
        private const string GameName = "Anarchy Online";
        
        public static void Main(string[] args)
        {
            using (ProcessSubscription)
            {
                Console.ReadKey();
            }
        }

        private static IDisposable ProcessSubscription =>
            ProcessObservable.Subscribe(process => Console.WriteLine(process.Info()));

        private static IObservable<Process> ProcessObservable =>
            Observable.Timer(TimeSpan.FromMilliseconds(500))
                      .SelectMany(_ => Process.GetProcesses())
                      .Where(p => p.ProcessName     == GameName.WithoutWhitespace())
                      .Where(p => p.CharacterName() != GameName)
                      .Repeat();
    }
}
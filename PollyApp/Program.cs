using System;
using Polly;

namespace PollyApp
{
    static class Program
    {
        private static int _doCount;

        static void Main()
        {
            var policy = Policy
                .Handle<Exception>()
                .Retry(1, (exception, _) =>
                {
                    Console.WriteLine($"polly retry cause of {exception.Message}");
                });

            try
            {
                var result = policy.Execute(DoSomething);
                Console.WriteLine($"polly result {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"polly finally failed with {ex.Message}");
            }

            Console.ReadLine();
        }

        static string DoSomething()
        {
            _doCount++;
            Console.WriteLine($"try {_doCount}");

            // fail on first run
            if (_doCount < 3)
            {
                Console.WriteLine($"try {_doCount} failing");
                throw new Exception($"try {_doCount} exception");
            }
            else
            {
                Console.WriteLine($"try {_doCount} success");
                return "I DID IT";
            }
        }
    }
}

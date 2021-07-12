using System;
using System.Threading.Tasks;
using Polly;

namespace PollyApp
{
    static class Program
    {
        private static int _doCount;
        private static int _failCount = 2;

        static async Task Main()
        {
            var fallbackValue = "FALLBACK";

            var fallback = Policy<string>
                .Handle<Exception>()
                .FallbackAsync(fallbackValue, result =>
                {
                    Console.WriteLine($"polly fallback cause of {result.Exception.Message}");
                    return Task.CompletedTask;
                });

            var retry = Policy<string>
                .Handle<Exception>()
                .RetryAsync(1, (result, _) =>
                {
                    Console.WriteLine($"polly retry cause of {result.Exception.Message}");
                });

            var policy = Policy.WrapAsync(fallback, retry);

            try
            {
                var result = await policy.ExecuteAsync(DoSomething);
                Console.WriteLine($"polly result {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"polly finally failed with {ex.Message}");
            }

            Console.ReadLine();
        }

        static Task<string> DoSomething()
        {
            _doCount++;
            Console.WriteLine($"try {_doCount}");

            // fail on first run
            if (_doCount <= _failCount)
            {
                Console.WriteLine($"try {_doCount} failing");
                throw new Exception($"try {_doCount} exception");
            }
            else
            {
                Console.WriteLine($"try {_doCount} success");
                return Task.FromResult("I DID IT");
            }
        }
    }
}

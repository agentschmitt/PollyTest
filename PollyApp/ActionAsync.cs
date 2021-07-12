using System;
using System.Threading.Tasks;
using Polly;

namespace PollyApp
{
    public class ActionAsync
    {
        private int _doCount;

        public int RetryCount { get; set; } = 1;

        public int FailCount { get; set; } = 2;

        public async Task Do()
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
                .RetryAsync(RetryCount, (result, _) =>
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

        private Task<string> DoSomething()
        {
            _doCount++;
            Console.WriteLine($"try {_doCount}");

            // fail on defined runs
            if (_doCount <= FailCount)
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

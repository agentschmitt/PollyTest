using System;
using Polly;

namespace PollyApp
{
    public class Action
    {
        private int _doCount;

        public int RetryCount { get; set; } = 1;

        public int FailCount { get; set; } = 2;

        public void Do()
        {
            var fallbackValue = "FALLBACK";

            var fallback = Policy<string>
                .Handle<Exception>()
                .Fallback(fallbackValue, (result) =>
                {
                    Console.WriteLine($"polly fallback cause of {result.Exception.Message}");
                });

            var retry = Policy<string>
                .Handle<Exception>()
                .Retry(RetryCount, (result, _) =>
                {
                    Console.WriteLine($"polly retry cause of {result.Exception.Message}");
                });

            var policy = Policy.Wrap(fallback, retry);

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

        private string DoSomething()
        {
            _doCount++;
            Console.WriteLine($"try {_doCount}");

            // fail on defined runs
            if (_doCount < FailCount)
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
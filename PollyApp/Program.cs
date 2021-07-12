using System;
using System.Threading.Tasks;

namespace PollyApp
{
    static class Program
    {
        private static readonly int _retryCount = 1;
        private static readonly int _failCount = 2;
        private static ActionType actionType = ActionType.Async;

        static async Task Main()
        {
            Console.WriteLine($"ActionType {actionType}");

            switch (actionType)
            {
                case ActionType.Sync:
                {
                    var action = new Action { RetryCount = _retryCount, FailCount = _failCount };
                    action.Do();
                    break;
                }

                case ActionType.Async:
                {
                    var action = new ActionAsync { RetryCount = _retryCount, FailCount = _failCount };
                    await action.Do();
                    break;
                }
            }

            Console.ReadLine();
        }
    }

    enum ActionType
    {
        Sync,
        Async
    }
}

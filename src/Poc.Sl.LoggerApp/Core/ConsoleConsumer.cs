using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Poc.Sl.LoggerApp.Core
{
    public class ConsoleConsumer : IObserver<BaseEvent>
    {
        public void OnCompleted()
        {
            // IObservable has finished.
            Debug.WriteLine($"{nameof(ConsoleConsumer)} | {nameof(OnCompleted)}");
        }

        public void OnError(Exception error)
        {
            // write log
            Debug.WriteLine($"{nameof(ConsoleConsumer)} | {nameof(OnError)}");
        }

        void IObserver<BaseEvent>.OnNext(BaseEvent value)
        {
            Console.WriteLine(value.ToString());
        }
    }
}

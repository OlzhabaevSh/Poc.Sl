using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Sl.LoggerApp.Core
{
    internal class Unsubscriber : IDisposable
    {
        private List<IObserver<BaseEvent>> observers;

        public Unsubscriber(List<IObserver<BaseEvent>> _observers)
        {
            this.observers = _observers;
        }

        public void Dispose()
        {
            if (this.observers == null || this.observers.Any())
                return;

            this.observers.Clear();
        }
    }
}

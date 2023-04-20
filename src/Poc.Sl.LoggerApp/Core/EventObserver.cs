using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Sl.LoggerApp.Core
{
    internal class EventObserver : IObservable<BaseEvent>
    {
        private static EventObserver _instance;

        public static EventObserver Instance 
        {
            get 
            {
                if (_instance == null)
                { 
                    _instance = new EventObserver();
                }

                return _instance;
            }
        }

        private readonly List<IObserver<BaseEvent>> observers;

        private EventObserver() 
        {
            this.observers = new List<IObserver<BaseEvent>>();
        }

        public IDisposable Subscribe(IObserver<BaseEvent> observer)
        {
            if (!this.observers.Contains(observer))
                this.observers.Add(observer);

            return new Unsubscriber(this.observers);
        }

        public void AddEvent(BaseEvent baseEvent)
        {
            // notify all observers
            foreach (var observer in this.observers)
            {
                observer.OnNext(baseEvent);
            }
        }
    }
}

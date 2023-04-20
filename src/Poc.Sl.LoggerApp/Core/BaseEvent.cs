using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Poc.Sl.LoggerApp.Core
{
    internal abstract class BaseEvent
    {
        public string EventKey { get; set; }

        public abstract string CategoryName { get; }

        public abstract string EventName { get; }

        /// <summary>
        /// Return as JSON
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this, this.GetType());
        }
    }
}

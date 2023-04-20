using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Sl.LoggerApp.Core
{
    internal static class NotificationService
    {
        public static event EventHandler<BaseEvent> Happend;
    }
}

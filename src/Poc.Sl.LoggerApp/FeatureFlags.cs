using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Sl.LoggerApp
{
    public static class FeatureFlags
    {
        public static bool UseHttpClient => false;
        public static bool UseAspnetCore => true;
    }
}

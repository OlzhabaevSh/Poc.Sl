using Microsoft.Diagnostics.Tracing.AutomatedAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Sl.LoggerApp
{
    internal static class ProcessHelper
    {
        public static string ProcessName { get; } = "Poc.Sl.HttpSenderApp";

        public static int? GetProcessId(string processName)
        {
            var process = GetProcessWithRetry(processName);

            return process != null 
                ? process.Id 
                : null;
        }

        private static System.Diagnostics.Process GetProcessWithRetry(string processName, int retryCount = 0)
        {
            var process = System.Diagnostics.Process
                .GetProcessesByName(processName)
                .FirstOrDefault();
            
            if (process == null && retryCount < 5)
            {   
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine($"Waiting {retryCount}");
                process = GetProcessWithRetry(processName, retryCount++);
            }

            return process;
        }
    }
}

﻿using Microsoft.Diagnostics.NETCore.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poc.Sl.LoggerApp
{
    internal static class EvenPipeProvidersHelper
    {
        public static List<EventPipeProvider> GetProviders()
        {
            return new List<EventPipeProvider>()
            {
                // Required providers:
                // 1. Microsoft-Extensions-Logging
                // 2. System.Threading.Tasks.TplEventSource
                new EventPipeProvider(
                    "Microsoft-Extensions-Logging",
                    EventLevel.LogAlways,
                    long.MaxValue),
                new EventPipeProvider(
                    "System.Threading.Tasks.TplEventSource",
                    EventLevel.LogAlways,
                    0x80),
                // after last check, looks like we don't need this provider
                //new EventPipeProvider(
                //    "System-Net-Http",
                //    EventLevel.LogAlways,
                //    long.MaxValue),
                //new EventPipeProvider(
                //    "Microsoft.AspNetCore.HttpLogging",
                //    EventLevel.LogAlways,
                //    long.MaxValue),
            };
        }
    }
}

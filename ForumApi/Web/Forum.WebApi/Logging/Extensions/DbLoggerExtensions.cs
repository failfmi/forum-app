using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.WebApi.Logging.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace Forum.WebApi.Logging.Extensions
{
    public static class DBLoggerExtensions
    {
        public static ILoggerFactory AddContext(this ILoggerFactory factory, IApplicationBuilder applicationBuilder,
            Func<string, LogLevel, bool> filter = null)
        {
            factory.AddProvider(new DBLoggerProvider(applicationBuilder, filter));

            return factory;
        }

        public static ILoggerFactory AddContext(this ILoggerFactory factory, IApplicationBuilder applicationBuilder, LogLevel min)
        {
            return AddContext(factory, applicationBuilder, (_, logLevel) => logLevel >= min);
        }
    }
}

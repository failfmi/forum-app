using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace Forum.WebApi.Logging.Providers
{
    public class DBLoggerProvider : ILoggerProvider
    {
        private readonly IApplicationBuilder applicationBuilder;
        private readonly Func<string, LogLevel, bool> filter;

        public DBLoggerProvider(IApplicationBuilder applicationBuilder, Func<string, LogLevel, bool> filter)
        {
            this.applicationBuilder = applicationBuilder;
            this.filter = filter;
        }

        public void Dispose()
        {
            
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DBLogger(applicationBuilder, categoryName, filter);
        }
    }
}

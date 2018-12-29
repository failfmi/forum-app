using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.Data;
using Forum.Data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Forum.WebApi.Logging
{
    public class DBLogger : ILogger
    {
        private readonly IApplicationBuilder applicationBuilder;
        private readonly string categoryName;
        private readonly Func<string, LogLevel, bool> filter;

        public DBLogger(IApplicationBuilder applicationBuilder, string categoryName, Func<string, LogLevel, bool> filter)
        {
            this.applicationBuilder = applicationBuilder;
            this.categoryName = categoryName;
            this.filter = filter;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            using (var serviceScope = this.applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ForumContext>();

                context.Logs
                    .Add(new EventLog
                    {
                        EventId = eventId.Id,
                        LogLevel = logLevel.ToString(),
                        CreatedTime = DateTime.UtcNow,
                        Message = state.ToString()
                    });
                context.SaveChanges();
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (filter == null || filter(categoryName, logLevel));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}

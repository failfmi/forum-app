using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Forum.WebApi.Middleware.Extensions
{
    public static class FakeRemoteIpAddressMiddlewareExtension
    {
        public static IApplicationBuilder UseFakeRemoteIpAddressMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FakeRemoteIpAddressMiddleware>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebApi.Utils
{
    public class ReturnMessage : ProblemDetails
    {
        public string Message { get; set; }
    }
}

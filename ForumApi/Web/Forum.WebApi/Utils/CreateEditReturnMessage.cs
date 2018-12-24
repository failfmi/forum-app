using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Forum.WebApi.Utils
{
    public class CreateEditReturnMessage<T> : ProblemDetails
    {
        public string Message { get; set; }

        public T Data { get; set; }
    }
}

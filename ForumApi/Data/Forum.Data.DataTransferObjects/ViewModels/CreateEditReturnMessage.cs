using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.Data.DataTransferObjects.ViewModels
{
    public class CreateEditReturnMessage<T>
    {
        public string Message { get; set; }

        public T Data { get; set; }
    }
}

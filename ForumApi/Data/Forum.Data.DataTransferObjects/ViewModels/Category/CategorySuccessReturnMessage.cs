using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.Data.DataTransferObjects.ViewModels.Category
{
    public class CategorySuccessReturnMessage
    {
        public string Message { get; set; }

        public CategoryViewModel Data { get; set; }
    }
}

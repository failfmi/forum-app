using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.Data.DataTransferObjects.ViewModels.Category
{
    public class CategoryAllReturn
    {
        public ICollection<CategoryViewModel> Data { get; set; }
    }
}

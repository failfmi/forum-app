using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Data.Models
{
    public class BaseModel<T>
    {
        [Key]
        public T Id { get; set; }
    }
}

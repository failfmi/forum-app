using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Forum.Data.DataTransferObjects.InputModels.Post
{
    public class PostInputEditModel : PostInputModel
    {
        [Required]
        public int Id { get; set; }
    }
}

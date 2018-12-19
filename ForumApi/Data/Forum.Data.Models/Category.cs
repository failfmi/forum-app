using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Forum.Data.Models
{
    public class Category : BaseModel<int>
    {
        public string Name { get; set; }
    }
}

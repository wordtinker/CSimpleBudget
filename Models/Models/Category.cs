using System.Collections.Generic;

namespace Models
{
    public class Category
    {
        public string Name { get; set; }
        public Category Parent { get; set; }
        public List<Category> Children { get; set; }

        public Category()
        {
            Children = new List<Category>();
        }
    }
}

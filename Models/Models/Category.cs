using System.Collections.Generic;

namespace Models
{
    public class Category
    {
        // Unique category ID.
        internal int Id { get; set; }
        // Name of the category.
        public string Name { get; set; }
        // Parent category. If the category is top tier, parent is Null.
        public Category Parent { get; set; }
        // Chuld categories. For bottom tier categories this list is empty.
        public List<Category> Children { get; } = new List<Category>();
    }
}

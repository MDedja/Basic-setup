using Entities.Base;

namespace Entities.Category
{
    public class Category : Entity
    {
        public Category(Guid id) : base(id)
        {
        }

        public Category(Guid id, string code, string name) : base(id)
        {
            Code = code;
            Name = name;
        }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
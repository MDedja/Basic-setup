using Entities.Base;

namespace Entities.Category.ReadModels
{
    public class CategoryReadModel : IReadModel
    {
        public CategoryReadModel(Guid id, string code, string name)
        {
            Id = id;
            Code = code;
            Name = name;
        }

        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
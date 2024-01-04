namespace Infrastructure.Persistence.Records.Base;

public interface IRecord
{
    Guid Id { get; set; }
}
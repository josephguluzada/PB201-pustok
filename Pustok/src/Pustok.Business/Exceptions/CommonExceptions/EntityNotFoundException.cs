using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Pustok.Business.Exceptions.CommonExceptions;

public class EntityNotFoundException : Exception
{
    public string PropertyName { get; set; }
    public EntityNotFoundException()
    {
    }

    public EntityNotFoundException(string? message) : base(message)
    {
    }
    public EntityNotFoundException(string propertyName, string? message) : base(message)
    {
        PropertyName = propertyName;
    }
}

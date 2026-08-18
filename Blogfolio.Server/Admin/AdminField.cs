using System.Reflection;

namespace Blogfolio.Server.Admin;

public class AdminField
{
    public required string Name { get; init; }
    public required string Label { get; init; }
    public required Type ClrType { get; init; }
    public  PropertyInfo? Property { get; init; }
    public FieldInfo? Field { get; init; }
    public Type? PrincipalType { get; init; }

    public bool IsKey { get; init; }
    public bool IsReadOnly { get; init; }
    public bool IsHidden { get; init; }
    public bool IsSecret { get; init; }
    
    public bool IsForeignKey => PrincipalType is not null;
    public bool IsEnum => Nullable.GetUnderlyingType(ClrType)?.IsEnum ?? ClrType.IsEnum;
}


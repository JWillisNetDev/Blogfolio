using System.Reflection;

namespace Blogfolio.Server.Admin;

public sealed class AdminField2
{
    public required string Name { get; init; }
    public required string Label { get; init; }
    public required Type ClrType { get; init; }
    public PropertyInfo? Property { get; init; }
    public Type? PrincipalType { get; init; }

    public bool IsKey { get; init; }
    public bool IsReadOnly { get; init; }
    public bool IsHidden { get; init; }
    public bool IsSecret { get; init; }
    
    public bool IsForeignKey => PrincipalType is not null;
    public bool IsEnum => ClrType.IsEnum;
}

public sealed record AdminField(
    string Name, // Generated
    string Label, // User-defined or otherwise generated
    Type ClrType, // Generated.
    PropertyInfo? Property, // Make this non-nullable? Is this because of fields? What cases is this null?
    bool IsKey, // Generated
    bool ReadOnly, // Generated or user-defined
    bool Hidden, // User-defined
    bool IsForeignKey, // Generated
    Type? PrincipalType, // Generated, but tightly coupled to IsForeignKey. Can IsForeignKey be a function of PrincipalType?
    bool IsEnum, // Generated
    bool IsSecret = false); // User-defined

using System.Reflection;

namespace Blogfolio.Server.Admin;

public sealed record AdminField(
    string Name,
    string Label,
    Type ClrType,
    PropertyInfo? Property,
    bool IsKey,
    bool ReadOnly,
    bool Hidden,
    bool IsForeignKey,
    Type? PrincipalType,
    bool IsEnum,
    bool IsSecret = false);

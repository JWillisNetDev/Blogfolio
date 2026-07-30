using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blogfolio.Data.Annotations;

public static class AdminAnnotations
{
    internal const string AdminPanelEnabledKey = "AdminPanel:Enabled";
    internal const string AdminPanelHiddenKey = "AdminPanel:Hidden";
    internal const string AdminPanelSecretKey = "AdminPanel:Secret";
    internal const string AdminPanelReadOnlyKey = "AdminPanel:ReadOnly";

    public static EntityTypeBuilder<T> AdminPanelEnabled<T>(this EntityTypeBuilder<T> etb) where T : class => etb.HasAnnotation(AdminPanelEnabledKey, true);

    public static PropertyBuilder<T> AdminPanelHidden<T>(this PropertyBuilder<T> pb) => pb.HasAnnotation(AdminPanelHiddenKey, true);
    public static PropertyBuilder<T> AdminPanelSecret<T>(this PropertyBuilder<T> pb) => pb.HasAnnotation(AdminPanelSecretKey, true);
    public static PropertyBuilder<T> AdminPanelReadOnly<T>(this PropertyBuilder<T> pb) => pb.HasAnnotation(AdminPanelReadOnlyKey, true);

    public static bool IsAdminPanelEnabledFor(IEntityType typ) => typ[AdminPanelEnabledKey] is true;
    public static bool IsAdminPanelHiddenFor(IProperty prop) => prop[AdminPanelHiddenKey] is true;
    public static bool IsAdminPanelSecretFor(IProperty prop) => prop[AdminPanelSecretKey] is true;
    public static bool IsAdminPanelReadOnlyFor(IProperty prop) => prop[AdminPanelReadOnlyKey] is true;
}
using Microsoft.AspNetCore.Components;

namespace Blogfolio.Server.Admin.Handlers;

public abstract class AdminFormBase<TModel> : ComponentBase
{
    [Parameter, EditorRequired]
    public TModel Model { get; set; } = default!;
    
    [Parameter]
    public EventCallback OnSubmit { get; set; }
}
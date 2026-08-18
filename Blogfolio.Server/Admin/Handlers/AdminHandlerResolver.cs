using Microsoft.AspNetCore.Components;

namespace Blogfolio.Server.Admin.Handlers;



public interface IAdminFormHandlerResolver
{    void Register<THandler, TModel, TForm, TComponent>()
        where THandler : AdminCustomFormHandler<TModel, TForm, TComponent>
        where TForm : notnull, new()
        where TComponent : AdminFormBase<TForm>;

    THandler? ResolveFormHandler<THandler, TModel, TForm, TComponent>()
        where THandler : AdminCustomFormHandler<TModel, TForm, TComponent>
        where TForm: notnull, new()
        where TComponent : AdminFormBase<TForm>;
}

public sealed class AdminFormHandlerResolver(IServiceProvider services) : IAdminFormHandlerResolver, IDisposable
{
    private delegate THandler HandlerFactory<out THandler>() where THandler : IAdminCustomFormHandler;

    private readonly Dictionary<Type, IAdminCustomFormHandler> _customHandlers = [];
    private IEnumerable<IDisposable> DisposableCustomHandlers => _customHandlers.Values.OfType<IDisposable>();
    private readonly Dictionary<Type, Func<IAdminCustomFormHandler>> _customHandlerFactories = [];
    private bool _disposed;

    public void Register<THandler, TModel, TForm, TComponent>()
        where THandler : AdminCustomFormHandler<TModel, TForm, TComponent>
        where TForm : notnull, new()
        where TComponent : AdminFormBase<TForm>
    {
        EnsureNotAlreadyRegistered<TModel>();
        _customHandlerFactories.Add(typeof(TModel), ActivateHandler<THandler>);
    }

    public THandler? ResolveFormHandler<THandler, TModel, TForm, TComponent>()
        where THandler : AdminCustomFormHandler<TModel, TForm, TComponent>
        where TForm: notnull, new()
        where TComponent : AdminFormBase<TForm>
    {
        var typ = typeof(TModel);
        if (_customHandlers.TryGetValue(typ, out var h) && h is THandler hh)
        {
            return hh;
        }

        if (_customHandlerFactories.TryGetValue(typ, out var factory) && factory() is THandler hh2)
        {
            return hh2;
        }

        return null;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        foreach (var disposable in DisposableCustomHandlers)
        {
            disposable?.Dispose();
        }

        _disposed = true;
    }

    private T ActivateHandler<T>() where T : IAdminCustomFormHandler
    {
        return ActivatorUtilities.CreateInstance<T>(services);
    }

    private void EnsureNotAlreadyRegistered<T>()
    {
        var typ = typeof(T);
        if (_customHandlerFactories.ContainsKey(typ) || _customHandlers.ContainsKey(typ))
        {
            throw new InvalidOperationException("Cannot register more than one handler for a single admin model type.");
        }
    }
}
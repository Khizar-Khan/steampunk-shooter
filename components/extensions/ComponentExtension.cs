using System;
using Godot;

namespace SteampunkShooter.components.extensions;

public abstract partial class ComponentExtension : Node
{
    public Component ParentComponent { get; private set; }

    private bool _isEnabled = true;

    [Export]
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled == value)
                return;

            _isEnabled = value;
            if (_isEnabled)
                OnEnabled();
            else
                OnDisabled();
        }
    }

    protected internal void SetParent(Component parentComponent)
    {
        if (ParentComponent != null)
            throw new InvalidOperationException("ParentComponent is already set.");

        ParentComponent = parentComponent ?? throw new ArgumentNullException(nameof(parentComponent));
    }

    // Virtual method for initialisation logic, can be overridden by derived component extensions
    internal virtual void OnInitialise()
    {
        // Default initialisation logic
    }

    // Virtual method for per-frame processing, can be overridden by derived component extensions
    internal virtual void OnProcess(double delta)
    {
        // Default per-frame processing logic
    }

    // Virtual method for per-physics process processing, can be overridden by derived component extensions
    internal virtual void OnPhysicsProcess(double delta)
    {
        // Default per-physics process logic
    }

    // Hook called when the component extension is enabled
    protected virtual void OnEnabled()
    {
        SetProcess(true);
        SetPhysicsProcess(true);
    }

    // Hook called when the component extension is disabled
    protected virtual void OnDisabled()
    {
        SetProcess(false);
        SetPhysicsProcess(false);
    }
}
using System.Collections.Generic;
using Godot;
using SteampunkShooter.components.extensions;

namespace SteampunkShooter.components;

public abstract partial class Component : Node
{
    private bool _isEnabled = true;
    private List<ComponentExtension> _extensions;

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
            {
                OnEnabled();
            }
            else
            {
                OnDisabled();
            }
        }
    }

    public override async void _Ready()
    {
        if (Owner == null)
        {
            GD.PrintErr("Component needs a valid owner. Disabling component.");
            IsEnabled = false;
            return;
        }

        await ToSignal(Owner, "ready");

        Initialise();
        InitialiseExtensions();
    }

    // Process method that only runs if the component is enabled
    public override void _Process(double delta)
    {
        if (!_isEnabled)
            return;

        Process(delta);
        foreach (ComponentExtension extension in _extensions)
        {
            if (extension.IsEnabled)
            {
                extension.Process(delta);
            }
        }
    }

    // Physics Process method that only runs if the component is enabled
    public override void _PhysicsProcess(double delta)
    {
        if (!_isEnabled)
            return;

        PhysicsProcess(delta);
        foreach (ComponentExtension extension in _extensions)
        {
            if (extension.IsEnabled)
            {
                extension.PhysicsProcess(delta);
            }
        }
    }

    // Virtual method for initialisation logic, can be overridden by derived components
    protected virtual void Initialise()
    {
        _extensions = new List<ComponentExtension>();
    }

    // Virtual method for per-frame processing, can be overridden by derived components
    protected virtual void Process(double delta)
    {
        // Default per-frame processing logic
    }

    // Virtual method for per-physics process processing, can be overridden by derived components
    protected virtual void PhysicsProcess(double delta)
    {
        // Default per-physics process logic
    }

    // Hook called when the component is enabled
    protected virtual void OnEnabled()
    {
        // Logic to execute when the component is enabled
    }

    // Hook called when the component is disabled
    protected virtual void OnDisabled()
    {
        // Logic to execute when the component is disabled
    }

    private void InitialiseExtensions()
    {
        _extensions ??= new List<ComponentExtension>();

        foreach (Node node in GetChildren())
        {
            if (node is ComponentExtension componentExtension)
            {
                componentExtension.SetParent(this);
                componentExtension.Initialise();
                _extensions.Add(componentExtension);
            }
        }
    }
}
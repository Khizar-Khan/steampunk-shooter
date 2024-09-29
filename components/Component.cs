using System.Collections.Generic;
using Godot;
using SteampunkShooter.components.extensions;

namespace SteampunkShooter.components;

public abstract partial class Component : Node
{
    private bool _isEnabled = true;
    private readonly List<ComponentExtension> _extensions = new();

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

    public sealed override async void _Ready()
    {
        if (Owner == null)
        {
            GD.PrintErr("Component needs a valid owner. Disabling component.");
            IsEnabled = false;
            return;
        }

        await ToSignal(Owner, "ready");

        OnInitialise();
        InitialiseExtensions();
    }

    // Process method that only runs if the component is enabled
    public sealed override void _Process(double delta)
    {
        if (!_isEnabled)
            return;

        OnProcess(delta);
        ProcessExtensions(delta);
    }

    // Physics Process method that only runs if the component is enabled
    public sealed override void _PhysicsProcess(double delta)
    {
        if (!_isEnabled)
            return;

        OnPhysicsProcess(delta);
        PhysicsProcessExtensions(delta);
    }

    // Virtual method for initialisation logic, can be overridden by derived components
    protected virtual void OnInitialise()
    {
        // Initialisation logic for the component.
    }

    // Virtual method for per-frame processing, can be overridden by derived components
    protected virtual void OnProcess(double delta)
    {
        // Default per-frame processing logic
    }

    // Virtual method for per-physics process processing, can be overridden by derived components
    protected virtual void OnPhysicsProcess(double delta)
    {
        // Default per-physics process logic
    }

    // Hook called when the component is enabled
    protected virtual void OnEnabled()
    {
        SetProcess(true);
        SetPhysicsProcess(true);
    }

    // Hook called when the component is disabled
    protected virtual void OnDisabled()
    {
        SetProcess(false);
        SetPhysicsProcess(false);
    }

    private void InitialiseExtensions()
    {
        foreach (Node node in GetChildren())
        {
            if (node is ComponentExtension componentExtension)
            {
                componentExtension.SetParent(this);
                componentExtension.OnInitialise();
                _extensions.Add(componentExtension);
            }
        }
    }

    private void ProcessExtensions(double delta)
    {
        foreach (ComponentExtension extension in _extensions)
        {
            if (extension.IsEnabled)
            {
                extension.OnProcess(delta);
            }
        }
    }

    private void PhysicsProcessExtensions(double delta)
    {
        foreach (ComponentExtension extension in _extensions)
        {
            if (extension.IsEnabled)
            {
                extension.OnPhysicsProcess(delta);
            }
        }
    }
}
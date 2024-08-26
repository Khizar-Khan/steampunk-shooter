using System;
using Godot;

namespace SteampunkShooter.components;

public abstract partial class Component : Node
{
    [Signal]
    public delegate void EnabledStateChangedEventHandler(bool isEnabled);

    private bool _isEnabled = true;

    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled == value)
                return;

            _isEnabled = value;
            EmitSignal(SignalName.EnabledStateChanged, _isEnabled);

            if (_isEnabled)
                OnEnabled();
            else
                OnDisabled();
        }
    }

    public override void _Ready()
    {
        Initialise();
    }

    // Process method that only runs if the component is enabled
    public override void _Process(double delta)
    {
        RunIfEnabled(ProcessComponent, delta);
    }

    // Physics Process method that only runs if the component is enabled
    public override void _PhysicsProcess(double delta)
    {
        RunIfEnabled(PhysicsProcessComponent, delta);
    }

    // Virtual method for initialisation logic, can be overridden by derived components
    protected virtual void Initialise()
    {
        // Default initialisation logic
    }

    // Virtual method for per-frame processing, can be overridden by derived components
    protected virtual void ProcessComponent(double delta)
    {
        // Default per-frame processing logic
    }

    // Virtual method for per-physics process processing, can be overridden by derived components
    protected virtual void PhysicsProcessComponent(double delta)
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

    private void RunIfEnabled(Action<double> processMethod, double delta)
    {
        if (_isEnabled)
        {
            processMethod(delta);
        }
    }
}
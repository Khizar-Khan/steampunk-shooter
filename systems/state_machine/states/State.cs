using Godot;

namespace SteampunkShooter.systems.state_machine.states;

public abstract partial class State : Node
{
    protected StateMachine StateMachine { get; private set; }

    public virtual void Initialise(StateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }
    
    public virtual void Enter() {}
    public virtual void Exit() {}
    
    public virtual void Update(double delta) {}
    public virtual void PhysicsUpdate(double delta) {}
    public virtual void HandleInput(InputEvent @event) {}
}
using System;

public abstract class StateBase<T> : IState<T> where T : Enum
{
    protected readonly Character character;
    protected readonly GameBus gameBus;

    public virtual T Type { get; }

    public virtual T Update() => Type;

    public StateBase(Character character, GameBus gameBus)
    {
        this.character = character;
        this.gameBus = gameBus;
    }

    public virtual void Enter()
    {

    }
}

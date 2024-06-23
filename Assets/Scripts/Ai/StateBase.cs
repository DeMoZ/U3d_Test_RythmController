using System;

public abstract class StateBase : IState
{
    protected readonly Character character;
    protected readonly GameBus gameBus;

    public StateBase(Character character, GameBus gameBus)
    {
        this.character = character;
        this.gameBus = gameBus;
    }

    public virtual void Enter()
    {

    }

    public virtual Type Update()
    {
        return null;
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class StateBase<T> : IState<T> where T : Enum
{
    protected readonly Character _character;
    protected readonly GameBus _gameBus;

    public virtual T Type { get; }

    public virtual T Update(float deltaTime = 0) => Type;

    public StateBase(Character character, GameBus gameBus)
    {
        _character = character;
        _gameBus = gameBus;
    }

    public virtual async Task EnterAsync(CancellationToken token)
    {
        await Task.Yield();
    }

    public virtual async Task ExitAsync(CancellationToken token)
    {
        await Task.Yield();
    }

    protected bool IsInRange(Vector3 point, float distance)
    {
        return Vector3.Distance(_character.Transform.position, point) <= distance;
    }
}

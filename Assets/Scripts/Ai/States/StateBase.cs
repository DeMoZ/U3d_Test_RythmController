using System;
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

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

    public virtual void Enter()
    {
        Debug.Log($"{GetType()} Enter");
    }

    public virtual void Exit()
    {
        Debug.Log($"{GetType()} Exit");
    }

    protected bool IsInRange(Vector3 point, float distance)
    {
        return Vector3.Distance(_character.Transform.position, point) <= distance;
    }

    protected float GetRandomInRange(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
}
using System;
using System.Collections.Generic;
using DMZ.Events;

/// <summary>
/// Finit State Machine with external update loop
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class FSMUpdateBase<T> : IDisposable where T : Enum
{
    private Action<T> _onStateChangedCallback;

    protected DMZState<IState<T>> _currentState = new();
    protected Dictionary<T, IState<T>> _states;

    public virtual T DefaultStateType { get; }

    protected abstract void Init();
    public void OnEnter()
    {
        _currentState.Unsubscribe(OnStateChanged);
        _currentState.Value = _states[DefaultStateType];
        _currentState.Value.Enter();
    }

    public void OnExit()
    {
        _currentState.Unsubscribe(OnStateChanged);
        _currentState.Value.Exit();
        _currentState.Value = null;
    }

    public IState<T> GetState => _currentState.Value;

    public FSMUpdateBase(Action<T> stateChangedCallback)
    {
        _onStateChangedCallback = stateChangedCallback;
    }

    public void Dispose()
    {
        _currentState?.Dispose();
    }

    private void OnStateChanged(IState<T> state)
    {
        _onStateChangedCallback?.Invoke(state.Type);
    }

    public T Update(float deltaTime)
    {
        var nextStateType = _currentState.Value.Update(deltaTime);

        if (!nextStateType.Equals(_currentState.Value.Type))
        {
            _currentState.Value.Exit();
            _currentState.Value = _states[nextStateType];
            _currentState.Value.Enter();
        }

        return _currentState.Value.Type;
    }
}
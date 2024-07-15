using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DMZ.Events;

/// <summary>
/// Finit State Machine with internal update loop in task
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class FSMTaskBase<T> : IDisposable where T : Enum
{
    private Stopwatch _stopwatch;

    protected DMZState<IState<T>> _currentState = new();
    protected Dictionary<T, IState<T>> _states;
    protected CancellationTokenSource _cancelationTokenSource;

    protected abstract void Init();

    public FSMTaskBase()
    {
        _stopwatch = new Stopwatch();
        _currentState.Subscribe(OnStateChanged);
    }

    public void Dispose()
    {
        _currentState.Unsubscribe(OnStateChanged);
        _cancelationTokenSource?.Cancel();
    }

    protected abstract void OnStateChanged(IState<T> state);

    public void StopStateMachine()
    {
        _cancelationTokenSource?.Cancel();
    }

    private void SwitchToNextState(T nextState, CancellationToken token)
    {
        if (_states.TryGetValue(nextState, out var state))
        {
            _currentState.Value.Exit();

            if (token.IsCancellationRequested)
                return;

            _currentState.Value = state;
            _currentState.Value.Enter();
        }
        else
            throw new ArgumentOutOfRangeException();
    }

    public void RunStateMachine()
    {
        _cancelationTokenSource = new CancellationTokenSource();
        _stopwatch.Start();
        UpdateLoopAsync(_cancelationTokenSource.Token);
    }

    public async Task UpdateAsync(CancellationToken token)
    {
        var deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
        _stopwatch.Restart();

        var nextState = _currentState.Value.Update(deltaTime);

        if (!nextState.Equals(_currentState.Value.Type))
            SwitchToNextState(nextState, token);

        await Task.Delay(1, token);
    }

    private async void UpdateLoopAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (_currentState != null)
                    await UpdateAsync(token);
            }
        }
        catch (TaskCanceledException)
        {

        }
    }
}
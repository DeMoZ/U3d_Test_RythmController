using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DMZ.Events;

public abstract class StateMachineBase<T> : IDisposable where T : Enum
{
    protected DMZState<IState<T>> _currentState = new();
    protected Dictionary<T, IState<T>> _states;
    protected CancellationTokenSource _cancelationTokenSource;

    private Stopwatch _stopwatch;

    protected abstract void Init();

    public StateMachineBase(Action<T> stateChangedCallback)
    {
        _stopwatch = new Stopwatch();
        _currentState.Subscribe(stateType => stateChangedCallback?.Invoke(stateType.Type));
    }

    public void Dispose()
    {
        _cancelationTokenSource?.Cancel();
    }

    public void StopStateMachine()
    {
        _cancelationTokenSource?.Cancel();
    }

    private async Task SwitchToNextState(T nextState, CancellationToken token)
    {
        if (_states.TryGetValue(nextState, out var state))
        {
            await _currentState.Value.ExitAsync(token);

            if (token.IsCancellationRequested)
                return;

            _currentState.Value = state;
            await _currentState.Value.EnterAsync(token);
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

    // todo remove update loop and replase with Update mathod striked from outside with deltatime
    public async Task UpdateAsync(CancellationToken token)
    {
        var deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
        _stopwatch.Restart();

        var nextState = _currentState.Value.Update(deltaTime);

        if (!nextState.Equals(_currentState.Value.Type))
            await SwitchToNextState(nextState, token);

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
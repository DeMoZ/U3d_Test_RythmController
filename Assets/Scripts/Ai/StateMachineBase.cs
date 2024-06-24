using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMZ.Events;

public abstract class StateMachineBase<T> : IDisposable where T : Enum
{
    protected DMZState<IState<T>> _currentState = new();
    protected Dictionary<T, IState<T>> _states;
    protected CancellationTokenSource _cancelationTokenSource;

    protected abstract void Init();

    public StateMachineBase(Action<T> stateChangedCallback)
    {
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
        UpdateLoopAsync(_cancelationTokenSource.Token);
    }

    public async Task UpdateAsync(CancellationToken token)
    {
        var nextState = _currentState.Value.Update();

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
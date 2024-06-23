using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMZ.Events;

// public enum BotStates
// {
//     Idle,
//     Chase,
//     Attack,
//     Return
// }

public abstract class StateMachineBase : IDisposable
{
    protected DMZState<IState> _currentState = new();
    protected Dictionary<Type, IState> _states;
    protected CancellationTokenSource _cancelationTokenSource;

    protected abstract void Init();

    public StateMachineBase(Action<Type> stateChangedCallback)
    {
        _currentState.Subscribe(stateType => stateChangedCallback?.Invoke(stateType.GetType()));
    }

    public void Dispose()
    {
        _cancelationTokenSource?.Cancel();
    }

    public void StopStateMachine()
    {
        _cancelationTokenSource?.Cancel();
    }

    public void SwitchToNextState(Type nextState)
    {
        if (_states.TryGetValue(nextState, out var state))
            _currentState.Value = state;
        else
            throw new ArgumentOutOfRangeException();
    }

    public void RunStateMachine()
    {
        _cancelationTokenSource = new CancellationTokenSource();
        UpdateAsync(_cancelationTokenSource.Token);
    }

    public void Update()
    {
        Type nextState = _currentState.Value?.Update();
        SwitchToNextState(nextState);
    }

    private async void UpdateAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (_currentState != null)
                {
                    Update();
                    await Task.Delay(1, token);
                }
            }
        }
        catch (TaskCanceledException)
        {

        }
    }
}

public class BotBehaviour : StateMachineBase
{
    private GameBus _gameBus;
    private readonly Character _character;

    private Type _defaultState => typeof(IdleState);

    public BotBehaviour(Character character, GameBus gameBus, Action<Type> stateChangedCallback)
        : base(stateChangedCallback)
    {
        _gameBus = gameBus;
        _character = character;
        Init();
    }

    protected override void Init()
    {
        _states = new Dictionary<Type, IState>
        {
            { typeof(IdleState), new IdleState(_character, _gameBus ) },
            { typeof(ChaseState), new ChaseState(_character, _gameBus ) },
            { typeof(AttackState), new AttackState(_character, _gameBus ) },
            { typeof(ReturnState), new ReturnState(_character, _gameBus ) }
        };

        _currentState.Value = _states[_defaultState];
    }
}

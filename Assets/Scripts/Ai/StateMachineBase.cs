using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DMZ.Events;

public enum BotStates
{
    Idle,
    Chase,
    Attack,
    Return
}

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

    public void SwitchToNextState(T nextState)
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
        var nextState = _currentState.Value.Update();
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

public class BotBehaviour : StateMachineBase<BotStates>
{
    private GameBus _gameBus;
    private readonly Character _character;

    private BotStates _defaultState => BotStates.Idle;

    public BotBehaviour(Character character, GameBus gameBus, Action<BotStates> stateChangedCallback)
        : base(stateChangedCallback)
    {
        _gameBus = gameBus;
        _character = character;
        Init();
    }

    protected override void Init()
    {
        _states = new Dictionary<BotStates, IState<BotStates>>
        {
            { BotStates.Idle, new IdleState(_character, _gameBus ) },
            { BotStates.Chase, new ChaseState(_character, _gameBus ) },
            { BotStates.Attack, new AttackState(_character, _gameBus ) },
            { BotStates.Return, new ReturnState(_character, _gameBus ) }
        };

        _currentState.Value = _states[_defaultState];
    }
}

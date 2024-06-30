using System;
using System.Collections.Generic;

public class BotBehaviour : StateMachineBase<BotStates>
{
    private readonly GameBus _gameBus;
    private readonly Character _character;
    private readonly InputModel _inputModel;

    private BotStates _defaultState => BotStates.Idle;

    public BotBehaviour(Character character, GameBus gameBus, InputModel inputModel, Action<BotStates> stateChangedCallback)
        : base(stateChangedCallback)
    {
        _gameBus = gameBus;
        _character = character;
        _inputModel = inputModel;
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

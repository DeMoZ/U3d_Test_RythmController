using System;
using System.Collections.Generic;

public class BotFSM : FSMUpdateBase<States>
{
    private readonly GameBus _gameBus;
    private readonly Character _character;
    private States _defaultState => States.Idle;

    public BotFSM(Character character, GameBus gameBus, Action<States> stateChangedCallback = null)
    {
        _gameBus = gameBus;
        _character = character;

        Init();
    }

    protected override void Init()
    {
        _states = new Dictionary<States, IState<States>>
        {
            { States.Idle, new IdleState(_character, _gameBus ) },
            { States.Chase, new ChaseState(_character, _gameBus ) },
            { States.Attack, new AttackState(_character, _gameBus ) },
            { States.Return, new ReturnState(_character, _gameBus ) }
        };

        _currentState.Value = _states[_defaultState];
        _currentState.Value.Enter();
    }

    /// <summary>
    /// it is just for outside messages
    /// </summary>
    protected override void OnStateChanged(States state)
    {
        _character.ShowLog(0, state.ToString());

        // switch (state)
        // {
        //     case States.Idle:
        //         break;
        //     case States.Chase:
        //         break;
        //     case States.Attack:
        //         break;
        //     case States.Return:
        //         break;
        //     default:
        //         throw new ArgumentOutOfRangeException(nameof(state), state, null);
        // }
    }
}
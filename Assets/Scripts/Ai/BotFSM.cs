using System;
using System.Collections.Generic;

public class BotFSM : FSMUpdateBase<States>
{
    private readonly Character _character;
    private States _defaultState => States.Idle;

    public BotFSM(Character character, Action<States> stateChangedCallback = null)
    {
        _character = character;

        Init();
    }

    protected override void Init()
    {
        _states = new Dictionary<States, IState<States>>
        {
            { States.Idle, new IdleState(_character ) },
            { States.Chase, new ChaseState(_character ) },
            { States.Attack, new AttackState(_character ) },
            { States.Return, new ReturnState(_character ) },
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
        _character.CharacterModel.State = state;
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
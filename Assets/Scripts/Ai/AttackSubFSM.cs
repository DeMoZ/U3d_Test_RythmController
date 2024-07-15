using System;
using System.Collections.Generic;

public class AttackSubFSM : FSMUpdateBase<AttackSubStates>
{
    private readonly GameBus _gameBus;
    private readonly Character _character;

    private AttackSubStates _defaultState => AttackSubStates.Idle;

    public AttackSubFSM(Character character, GameBus gameBus)
    {
        _gameBus = gameBus;
        _character = character;
        Init();
    }

    protected override void Init()
    {
        _states = new Dictionary<AttackSubStates, IState<AttackSubStates>>
        {
            { AttackSubStates.Idle, new AttackIdleSubState(_character, _gameBus ) },
            { AttackSubStates.Hit, new AttackHitSubState(_character, _gameBus ) },
            // { BotAttackSubStates.Block, new BlockState(_character, _gameBus ) },
            // { BotAttackSubStates.Reposition, new ChaseState(_character, _gameBus ) },
        };

        _currentState.Value = _states[_defaultState];
    }

    /// <summary>
    /// it is just for outside messages
    /// </summary>
    protected override void OnStateChanged(AttackSubStates state)
    {
        _character.ShowLog(3, state.ToString());

        // switch (state)
        // {
        //     case AttackSubStates.Idle:
        //         break;
        //     case AttackSubStates.Hit:
        //         break;
        //     case AttackSubStates.Block:
        //         break;
        //     case AttackSubStates.Reposition:
        //         break;
        //     default:
        //         throw new ArgumentOutOfRangeException(nameof(state), state, null);
        // }
    }
}
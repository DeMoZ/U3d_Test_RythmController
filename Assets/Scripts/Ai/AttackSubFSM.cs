using System.Collections.Generic;

public class AttackSubFSM : FSMUpdateBase<AttackSubStates>
{
    private readonly Character _character;

    private AttackSubStates _defaultState => AttackSubStates.Countdown;

    public AttackSubFSM(Character character)
    {
        _character = character;
        Init();
    }

    protected override void Init()
    {
        _states = new Dictionary<AttackSubStates, IState<AttackSubStates>>
        {
            { AttackSubStates.Countdown, new AttackCountdownSubState(_character ) },
            { AttackSubStates.Hit, new AttackHitSubState(_character) },
            // { AttackSubStates.Block, new BlockState(_character) },
            { AttackSubStates.Reposition, new AttackRepositionSubState(_character ) },
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
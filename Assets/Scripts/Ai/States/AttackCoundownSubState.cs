using System.Collections.Generic;
using System.Linq;
using Debug = DMZ.DebugSystem.DMZLogger;

/// <summary>
/// In that state wait for some time between actions
/// </summary>
public class AttackCountdownSubState : StateBase<AttackSubStates>
{
    private class AttackIdleDice
    {
        public readonly AttackSubStates state;
        public readonly float chance;

        public AttackIdleDice(AttackSubStates state, float chance)
        {
            this.state = state;
            this.chance = chance;
        }
    }

    public override AttackSubStates Type { get; } = AttackSubStates.Countdown;

    private float _timer = 0f;

    /// <summary>
    /// decisiton to do something elese, not only counting for next hit - block, reposition, etc.
    /// </summary>
    private bool _isDecisionDone;
    private List<AttackIdleDice> _decisionRanges = new List<AttackIdleDice>{
        new(AttackSubStates.Countdown, 0.3f),
        // new(AttackSubStates.Hit, 0.5f),
        // new(AttackSubStates.Block, 0.8f),
        new(AttackSubStates.Reposition, 1f),
    };

    public AttackCountdownSubState(Character character, GameBus gameBus) : base(character, gameBus)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _isDecisionDone = false;
        _timer = GetRandomInRange(0.01f, 0.4f);
    }


    // todo roman consider what to do if player attack
    // todo roman consider what to do if player attack ME !!!
    // if CombatPhase.Pre - block or fast attack or nothing. Need to add percentage for every action type
    // todo roman add more states to check and implement for "smart bot" if CombatPhase...
    void A() { }


    public override AttackSubStates Update(float deltaTime)
    {
        if (!_isDecisionDone)
        {
            if (_gameBus.Player.CharacterModel.IsInAttackPhase)
            {
                _isDecisionDone = true;
                if (_gameBus.Player.CharacterModel.AttackSequenceState.Value == CombatPhase.Pre)
                {
                    var nextReactiveState = GetReactiveDecision();

                    if (nextReactiveState != AttackSubStates.Countdown)
                    {
                        Debug.Warning($"Behavour reaction. Next {nextReactiveState}");
                        return nextReactiveState;
                    }
                }
            }
        }

        _timer -= deltaTime;

        if (_timer <= 0)
            return AttackSubStates.Hit;

        return Type;
    }

    /// <summary>
    /// Next behavour sate if player attack
    /// </summary>
    /// <returns></returns>
    private AttackSubStates GetReactiveDecision()
    {
        var random = GetRandomInRange(0, 1);
        var sortedRanges = _decisionRanges.OrderBy(r => r.chance).ToList();
        var highestChanceBelowRandom = sortedRanges.FirstOrDefault(r => r.chance >= random);

        return highestChanceBelowRandom.state;
    }
}
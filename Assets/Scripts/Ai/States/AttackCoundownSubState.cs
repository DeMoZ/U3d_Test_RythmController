using System;
using System.Collections.Generic;
using System.Linq;
using Debug = DMZ.DebugSystem.DMZLogger;

/// <summary>
/// In that state wait for some time between actions
/// </summary>
public class AttackCountdownSubState : StateBase<AttackSubStates>
{
    public override AttackSubStates Type { get; } = AttackSubStates.Countdown;

    private float _timer = 0f;

    /// <summary>
    /// decisiton to do something elese, not only counting for next hit - block, reposition, etc.
    /// </summary>
    private bool _shouldExit;
    private AttackSubStates _nextState;

    /// <summary>
    /// Same as default decision ranges but countDown chance will be reduced every repeat on ReEnter
    /// </summary>
    private Dictionary<AttackSubStates, float> _decisionRangesTemp;

    // todo roman implement and initialize from config
    private AttackCountdownDecisionRanges _decisionRanges = new();

    public AttackCountdownSubState(Character character) : base(character)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _decisionRangesTemp = new Dictionary<AttackSubStates, float>(_decisionRanges.Countdown);
        _decisionRangesTemp[AttackSubStates.Countdown] += 0.1f; // to reduce first minus in reenter method;
        ReEnter();
    }

    // todo roman consider what to do if target attack ME !!!
    // todo roman add more states to check and implement for "smart bot" if CombatPhase...
    public override AttackSubStates Update(float deltaTime)
    {
        if (_shouldExit)
            return _nextState;

        var target = _characterModel.Target.Value;
        if (target && _characterModel.Target.Value.TryGetComponent<Character>(out var targetCharacter))
        {
            if (targetCharacter.CharacterModel.IsInAttackPhase)
            {// random chance of blocking or repositioning
                var ranges = targetCharacter.CharacterModel.IsInHardAttack ? _decisionRanges.Hit : _decisionRanges.HardHit;
                var reactionState = GetRandomReaction(ranges);
                if (reactionState != AttackSubStates.Countdown)
                    return reactionState;
            }
        }

        _timer -= deltaTime;

        if (_timer <= 0)
            ReEnter();

        return Type;
    }

    private void ReEnter()
    {
        var coundDownChance = _decisionRangesTemp[AttackSubStates.Countdown];
        _decisionRangesTemp[AttackSubStates.Countdown] = coundDownChance <= 0 ? 0 : coundDownChance - 0.1f; // reduce countdown to avoid passive loop

        _timer = GetRandomInRange(0.001f, 1f);
        _nextState = GetRandomReaction(_decisionRangesTemp);
        _shouldExit = _nextState != AttackSubStates.Countdown;
    }

    /// <summary>
    /// Next behavour sate get randomly from ranges list
    /// </summary>
    private AttackSubStates GetRandomReaction(Dictionary<AttackSubStates, float> ranges)
    {
        var summ = ranges.Sum(x => x.Value);
        var random = GetRandomInRange(0, summ);
        var currentSum = 0f;

        foreach (var pair in ranges)
        {
            currentSum += pair.Value;

            if (random <= currentSum)
                return pair.Key;
        }

        return AttackSubStates.Countdown;
    }

    [Serializable]
    private class AttackCountdownDecisionRanges
    {
        /// <summary>
        /// random behaviour decision on enter state
        /// </summary>
        private Dictionary<AttackSubStates, float> _countdown = new()
        {
            {AttackSubStates.Countdown, 0.3f},
            {AttackSubStates.Hit, 0.6f},
            // (AttackSubStates.Block, 0.4f),
            {AttackSubStates.Reposition, 0.3f},
        };

        /// <summary>
        /// random behaviour decision on player attack
        /// </summary>
        private Dictionary<AttackSubStates, float> _hit = new()
        {
            {AttackSubStates.Countdown, 0.5f},
            {AttackSubStates.Hit, 0.2f},
            // {AttackSubStates.Block, 1f},
            {AttackSubStates.Reposition, 0.5f},
        };

        /// <summary>
        /// random behaviour decision on player hard attack
        /// </summary>
        private Dictionary<AttackSubStates, float> _hardHit = new()
        {
            {AttackSubStates.Countdown, 0.2f},
            {AttackSubStates.Hit, 0.4f},
            // {AttackSubStates.Block, 0.5f},
            {AttackSubStates.Reposition, 1f},
        };

        public Dictionary<AttackSubStates, float> Countdown => _countdown;
        public Dictionary<AttackSubStates, float> Hit => _hit;
        public Dictionary<AttackSubStates, float> HardHit => _hardHit;
    }
}
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Hit and sequencing timer logic
/// </summary>
public class AttackHitSubState : StateBase<AttackSubStates>
{
    private float _sequenceTimer;
    private bool _holdButton;
    private (int, int) _curHitKey;
    private Queue<(int, int)> _queue;
    private readonly InputModel _inputModel;
    private readonly ICombatRepository _combatRepository;
    private int _sequenceStep = 0;

    public override AttackSubStates Type { get; } = AttackSubStates.Hit;

    public AttackHitSubState(Character character) : base(character)
    {
        _inputModel = character.InputModel;
        _combatRepository = character.CombatRepository;
    }

    // 1) get random combo from config
    // 2) if combo is not found - return to countdown
    // 3) if combo is found - get random lenght from combo (can be few hit from queue)
    // 4) run queue
    // 5) return to countdown
    public override void Enter()
    {
        base.Enter();
        _sequenceStep = 0;
        _sequenceTimer = 0;
        _holdButton = false;
        _queue = GetSequence();
    }

    // todo roman this method is hard coded. Need to be refactored when new attack sequences will be implemented
    private Queue<(int, int)> GetSequence()
    {
        var result = new Queue<(int, int)>();
        //List<(int, int)> keys = _character.CombatRepository.GetSequencesKeys();

        // at this time first hit is always (0,0) or (0,1)(for hard hit)
        result.Enqueue(Random.value < 0.5f ? (0, 0) : (0, 1));
        // at this time have only two hits queue
        var canAddNext = Random.value < 0.5f;
        if (canAddNext)
            result.Enqueue(Random.value < 0.5f ? (1, 0) : (1, 1));

        return result;
    }

    public override AttackSubStates Update(float deltaTime)
    {
        var isAttacking = _characterModel.IsInAttackPhase;

        if (!isAttacking)
        {
            var target = _characterModel.Target.Value;
            if (target != null && !IsInRange(target.Transform.position, _characterConfig.MeleAttackRange))
                return AttackSubStates.Countdown;
        }

        return UpdateAttack(deltaTime);
    }

    // todo roman this method is hard coded. Need to be refactored when new attack sequences will be implemented
    private AttackSubStates UpdateAttack(float deltaTime)
    {
        _sequenceTimer -= deltaTime;

        if (_sequenceTimer <= 0f)
        {
            switch (_sequenceStep)
            {
                case 0:
                    if (CanAttackAngle())
                    { // hold button to make hit or hard hit
                        _holdButton = true;
                        _curHitKey = _queue.Dequeue();
                        _sequenceTimer = _curHitKey.Item2 == 0
                        ? _combatRepository.GetPreAttackTime(_curHitKey)
                        : _combatRepository.GetPreAttackTime((_curHitKey.Item1, 0)) + _combatRepository.GetPreAttackTime((_curHitKey.Item1, 1));
                        _inputModel.OnAttack?.Invoke(true, AttackNames.Attack1);
                        _sequenceStep = 1;
                    }
                    else
                    { // clamp timer
                        _sequenceTimer = 0;
                    }
                    break;
                case 1:
                    if (_holdButton)
                    { // perform hit by release button and random await for new hit
                        _holdButton = false;
                        _sequenceTimer = _combatRepository.GetAttackTime(_curHitKey);
                        _inputModel.OnAttack?.Invoke(false, AttackNames.Attack1);
                        _sequenceStep = 2;
                    }
                    break;
                case 2:
                    // attack time is over and check if have more hits in this queue
                    if (_queue.Count > 0)
                    {
                        _sequenceStep = 0;
                    }
                    else
                    {
                        _sequenceTimer = _combatRepository.GetPostAttackTime(_curHitKey);
                        _sequenceStep = 3;
                    }
                    break;
                case 3:
                    return AttackSubStates.Countdown;
            }
        }
        return Type;
    }

    private bool CanAttackAngle()
    {
        var target = _characterModel.Target.Value;
        if (target == null)
            return false;

        var direction = target.Transform.position - _characterModel.Transform.position;
        var forward = _characterModel.Transform.forward;
        direction.y = 0;
        forward.y = 0;
        var angleDegrees = Vector3.Angle(direction, forward);
        return angleDegrees <= _characterConfig.AttackRotationAngle;
    }
}
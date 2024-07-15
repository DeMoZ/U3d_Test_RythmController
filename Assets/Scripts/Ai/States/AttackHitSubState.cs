using UnityEngine;

/// <summary>
/// Hit and sequencing timer logic
/// </summary>
public class AttackHitSubState : StateBase<AttackSubStates>
{
    private float _rotationVelocity;
    private float _sequenceTimer;
    private bool _holdButton;

    private readonly InputModel _inputModel;

    public override AttackSubStates Type { get; } = AttackSubStates.Hit;

    public AttackHitSubState(Character character, GameBus gameBus) : base(character, gameBus)
    {
        _inputModel = character.InputModel;
    }

    // todo implement timer
    public override AttackSubStates Update(float deltaTime)
    {
        var isAttacking = _character.IsInAttackPhase;
        if (!isAttacking && !IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.MeleAttackRange))
            return AttackSubStates.Idle;

        UpdateAttack(deltaTime);

        if (!isAttacking)
        {// rotation
            var direction = _gameBus.Player.Transform.position - _character.Transform.position;
            direction.y = 0;
            var axis = direction.normalized;
            var _targetRotation = Quaternion.LookRotation(axis).eulerAngles.y;
            var rotation = Mathf.SmoothDampAngle(_character.Transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _character.CharacterConfig.RotationSmoothTime);
            _character.Transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        return Type;
    }

    private void UpdateAttack(float deltaTime)
    {
        _sequenceTimer -= deltaTime;

        if (_sequenceTimer <= 0f)
        {
            if (_holdButton)
            {// perform hit by release button and random await for new hit
                _holdButton = false;
                _sequenceTimer = GetRandomTime(0.01f, 3f);
                _inputModel.OnAttack?.Invoke(false);
            }
            else if (CanAttackAngle())
            {// to make new hit, press and hold button for short random time
                _holdButton = true;
                _sequenceTimer = GetRandomTime(0.01f, 2f);
                _inputModel.OnAttack?.Invoke(true);
            }
            else
            {// clamp timer
                _sequenceTimer = 0;
            }
        }
    }

    private bool CanAttackAngle()
    {
        var direction = _gameBus.Player.Transform.position - _character.Transform.position;
        var forward = _character.Transform.forward;
        direction.y = 0;
        forward.y = 0;
        var angleDegrees = Vector3.Angle(direction, forward);
        return angleDegrees <= _character.CharacterConfig.AttackRotationAngle;
    }
}

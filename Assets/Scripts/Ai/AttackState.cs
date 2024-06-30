using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttackState : StateBase<BotStates>
{
    private readonly InputModel _inputModel;
    private readonly CharacterModel _characterModel;

    private float _rotationVelocity;
    private float _sequenceTimer;
    private CombatPhase _currentState;
    private bool _holdButton;

    public override BotStates Type { get; } = BotStates.Attack;

    public AttackState(Character character, GameBus gameBus)
        : base(character, gameBus)
    {
        _inputModel = character.InputModel;
        _characterModel = character.CharacterModel;
    }

    public override async Task EnterAsync(CancellationToken token)
    {
        _rotationVelocity = 0;
        _sequenceTimer = GetRandomTime(0.2f, 1);
        await Task.Yield();
    }

    // todo Implement the attacke angle support
    // _character.CharacterConfig.AttackRotationAngle
    public override BotStates Update(float deltaTime)
    {
        var isAttacking = IsAttacking();
        if (!isAttacking && !IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.MeleAttackRange))
            return BotStates.Chase;

        //if (isAttacking)
        UpdateAttack(deltaTime);

        if (CanAttackAngle())
        {
            //make attack
            // important - if
        }

        // if not perform hit at current time
        if (!isAttacking)
        {

            var direction = _gameBus.Player.Transform.position - _character.Transform.position;
            var inputDirection = direction.normalized;
            var targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            var rotation = Mathf.SmoothDampAngle(_character.Transform.eulerAngles.y, targetRotation, ref _rotationVelocity, _character.CharacterConfig.RotationSmoothTime);
            _character.Transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        return Type;
    }

    private void UpdateAttack(float deltaTime)
    {
        _sequenceTimer -= Time.deltaTime;

        if (_sequenceTimer <= 0f)
        {
            if (_holdButton)
            {// perform hit by release button and random await for new hit
                _holdButton = false;
                _sequenceTimer = GetRandomTime(0.01f, 3f);
                _inputModel.OnAttack?.Invoke(false);
            }
            else
            {// to make new hit, press and hold button for short random time
                _holdButton = true;
                _sequenceTimer = GetRandomTime(0.01f, 2f);
                _inputModel.OnAttack?.Invoke(true);
            }
        }
    }

    private float GetRandomTime(float min, float max)
    {
        return Random.Range(min, max);
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

    private bool IsAttacking()
    {
        return _holdButton || _character.CharacterModel.AttackSequenceState.Value is not CombatPhase.Idle or CombatPhase.None;
    }

    /*
    // Attack sequence using task
    private async void InputAttackAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay((int)(GetRandomTime(3, 6) * 1000), token);
                if (_cancellationTokenSource.IsCancellationRequested)
                    return;

                _inputModel.OnAttack?.Invoke(true);

                await Task.Delay((int)(GetRandomTime(0.01f, 3f) * 1000), token);
                if (token.IsCancellationRequested)
                    return;

                _inputModel.OnAttack?.Invoke(false);
            }
        }
        catch (TaskCanceledException)
        {
        }

        double GetRandomTime(float min, float max)
        {
            // return new Random().NextDouble() * (max - min) + min;
            return 1;
        }
    }*/
}

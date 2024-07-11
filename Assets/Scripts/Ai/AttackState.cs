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
        _holdButton = false;
        _rotationVelocity = 0;
        _sequenceTimer = GetRandomTime(0.2f, 1);
        await Task.Yield();
    }

    // todo Move if oponent is in attack phase
    // todo possible need to implement substate machine
    public override BotStates Update(float deltaTime)
    {
        var isAttacking = _character.IsInAttackPhase();
        if (!isAttacking && !IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.MeleAttackRange))
            return BotStates.Chase;

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

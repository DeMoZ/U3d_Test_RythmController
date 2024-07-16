using UnityEngine;

public class PlayerRotateStrategy : RotateStrategyBase
{
    private Transform _cameraTransform;

    public PlayerRotateStrategy(Camera mainCamera)
    {
        _cameraTransform = mainCamera.transform;
    }

    // todo implement FSM for targeting
    protected override void OnRotate(Vector3 axis, float deltaTime)
    {
        if (!_characterModel.IsInAttackPhase)
        {
            if (TryGetTarget(out var target))
            {
                var inputDirection = target.position - _transform.position;
                inputDirection.y = 0.0f;
                var _targetRotation = Quaternion.LookRotation(inputDirection).eulerAngles.y;
                var rotation = Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _characterConfig.RotationSmoothTime);
                _transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
            else if (axis != Vector3.zero)
            {
                var inputDirection = axis.normalized;
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
                var rotation = Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _characterConfig.RotationSmoothTime);
                _transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }
    }

    private bool TryGetTarget(out Transform target)
    {
        target = null;
        var minDistanceSquared = float.MaxValue;
        var chaseRangeSquared = _characterConfig.ChaseRange * _characterConfig.ChaseRange;

        foreach (var bot in _gameBus.Bots)
        {
            float distanceSquared = (bot.Transform.position - _transform.position).sqrMagnitude;
            if (distanceSquared < minDistanceSquared)
            {
                minDistanceSquared = distanceSquared;
                target = bot.Transform;
            }
        }

        _characterModel.Target = target;
        return target != null && minDistanceSquared <= chaseRangeSquared;
    }
}

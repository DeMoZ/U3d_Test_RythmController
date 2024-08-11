using UnityEngine;

// todo roman implement FSM if more states
public class BotRotateStrategy : RotateStrategyBase
{
    protected override void OnRotate(Vector3 axis, float deltaTime)
    {
        var target = _characterModel.Target.Value;
        if (target != null && IsOnTarget && !_characterModel.IsInAttackPhase)
        {
            var direction = target.Transform.position - _transform.position;
            direction.y = 0;

            var _targetRotation = Quaternion.LookRotation(direction.normalized).eulerAngles.y;
            var rotation = Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _characterConfig.RotationSmoothTime);
            _transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        else if (axis != Vector3.zero)
        {
            var _targetRotation = Quaternion.LookRotation(axis).eulerAngles.y;
            var rotation = Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _characterConfig.RotationSmoothTime);
            _transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
    }

    bool IsOnTarget => _characterModel.State is States.Attack or States.Chase;
}
using UnityEngine;

// todo implement FSM for targeting
// todo remove rotation from bot fsm attack state, probably
public class BotRotateStrategy : RotateStrategyBase
{
    protected override void OnRotate(Vector3 axis, float deltaTime)
    {
        if (axis != Vector3.zero)
        {
            var _targetRotation = Quaternion.LookRotation(axis).eulerAngles.y;
            var rotation = Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _characterConfig.RotationSmoothTime);
            _transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
    }
}

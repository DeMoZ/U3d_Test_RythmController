#define LOGGER_ON
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

public class BotMoveStrategy : MoveStrategyBase
{
    // todo need to limit movement speed
    // need to animate
    protected override void OnMove(Vector3 axis, float deltaTime)
    {
        var configSpeed = _characterConfig.WalkSpeed;
        var targetDirection = axis;
        _velocity += targetDirection.normalized * (configSpeed * _characterConfig.SpeedChangeRate * deltaTime);
        _velocity += -_velocity * (_characterConfig.SpeedChangeRate * deltaTime); // friction/resistance

        var horizontalMove = _velocity * deltaTime;
        var verticalMove = new Vector3(0.0f, _verticalVelocity, 0.0f) * deltaTime;
        _controller.Move(horizontalMove + verticalMove);

        if (axis != Vector3.zero)
        {// rotation
            var _targetRotation = Quaternion.LookRotation(axis).eulerAngles.y;
            var rotation = Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _characterConfig.RotationSmoothTime);
            _transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
    }
}
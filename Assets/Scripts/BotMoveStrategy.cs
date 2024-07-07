#define LOGGER_ON
using System;
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

public class BotMoveStrategy : MoveStrategyBase
{
    protected override void OnMove(Vector3 axis, float deltaTime)
    {
        if (_characterConfig == null)
            return;

        var targetSpeed = _inputModel.IsRunning.Value ? _characterConfig.SprintSpeed : _characterConfig.WalkSpeed;

        if (axis == Vector3.zero)
            targetSpeed = 0.0f;

        var curHorSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        if (curHorSpeed < targetSpeed - _characterConfig.SpeedOffset || curHorSpeed > targetSpeed + _characterConfig.SpeedOffset)
        {
            _speed = Mathf.Lerp(curHorSpeed, targetSpeed, _characterConfig.SpeedChangeRate * deltaTime);
            _speed = (float)Math.Round(_speed, 3);
        }
        else
        {
            _speed = targetSpeed;
        }

        var horizontalMove = axis.normalized * (_speed * deltaTime);
        var verticalMove = new Vector3(0.0f, _verticalVelocity, 0.0f) * deltaTime;
        _controller.Move(horizontalMove + verticalMove);

        _characterModel.MoveSpeed.Value = _speed;

        // todo if target is close, rotate towards target. Now rotates toward movement
        if (axis != Vector3.zero)
        {
            var _targetRotation = Quaternion.LookRotation(axis).eulerAngles.y;
            var rotation = Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _characterConfig.RotationSmoothTime);
            _transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
    }
}

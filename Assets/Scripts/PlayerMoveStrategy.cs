#define LOGGER_ON
using System;
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

public class PlayerMoveStrategy : MoveStrategyBase
{
    private Transform _cameraTransform;
    private Vector3 _velocity;

    public PlayerMoveStrategy(Camera mainCamera)
    {
        _cameraTransform = mainCamera.transform;
    }

    // todo need to limit movement speed
    // need to animate
    protected override void OnMove(Vector3 axis, float deltaTime)
    {
        var configSpeed = _characterConfig.WalkSpeed;
        var targetDirection = Quaternion.Euler(0.0f, _cameraTransform.eulerAngles.y, 0.0f) * axis;
        _velocity += targetDirection.normalized * (configSpeed * _characterConfig.SpeedChangeRate * Time.deltaTime);
        _velocity += -_velocity * (_characterConfig.SpeedChangeRate * Time.deltaTime); // friction/resistance
        _controller.Move(_velocity * Time.deltaTime);

        if (axis != Vector3.zero)
        {// rotation
            var inputDirection = axis.normalized;
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _cameraTransform.eulerAngles.y;
            var rotation = Mathf.SmoothDampAngle(_transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _characterConfig.RotationSmoothTime);
            _transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        _characterModel.MoveSpeed.Value = _speed;
    }
}
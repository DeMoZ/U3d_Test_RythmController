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
        // var configSpeed = _characterConfig.SprintSpeed;
        var targetDirection = axis;
        _velocity += targetDirection.normalized * (configSpeed * _characterConfig.SpeedChangeRate * deltaTime);
        _velocity += -_velocity * (_characterConfig.SpeedChangeRate * deltaTime); // friction/resistance

        var horizontalMove = _velocity * deltaTime;
        var verticalMove = new Vector3(0.0f, _verticalVelocity, 0.0f) * deltaTime;
        _controller.Move(horizontalMove + verticalMove);

        _characterModel.MoveSpeed.Value = _velocity.magnitude;
    }
}
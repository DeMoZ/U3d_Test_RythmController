#define LOGGER_ON
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

public class BotMoveStrategy : MoveStrategyBase
{
    // todo need to limit movement speed
    protected override void OnMove(Vector3 axis, float deltaTime)
    {
        var targetDirection = axis;
        _velocity += targetDirection.normalized * (ConfigSpeed * _characterConfig.SpeedChangeRate * deltaTime);
        _velocity += -_velocity * (_characterConfig.SpeedChangeRate * deltaTime); // friction/resistance

        var horizontalMove = _velocity * deltaTime;
        var verticalMove = new Vector3(0.0f, _verticalVelocity, 0.0f) * deltaTime;
        _controller.Move(horizontalMove + verticalMove);
    }
}
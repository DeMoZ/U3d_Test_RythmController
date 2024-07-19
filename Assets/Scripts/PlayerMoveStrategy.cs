#define LOGGER_ON
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

public class PlayerMoveStrategy : MoveStrategyBase
{
    private Transform _cameraTransform;

    public PlayerMoveStrategy(Transform cameraTransform)
    {
        _cameraTransform = cameraTransform.transform;
    }

    // todo need to limit movement speed
    protected override void OnMove(Vector3 axis, float deltaTime)
    {
       _characterModel.IsRunning = true;  // todo roman remove this speed hack

        axis = _characterModel.IsInAttackPhase ? Vector3.zero : axis;

        var targetDirection = Quaternion.Euler(0.0f, _cameraTransform.eulerAngles.y, 0.0f) * axis;
        _velocity += targetDirection.normalized * (ConfigSpeed * _characterConfig.SpeedChangeRate * deltaTime);
        _velocity += -_velocity * (_characterConfig.SpeedChangeRate * deltaTime); // friction/resistance

        var horizontalMove = _velocity * deltaTime;
        var verticalMove = new Vector3(0.0f, _verticalVelocity, 0.0f) * deltaTime;
        _controller.Move(horizontalMove + verticalMove);
    }
}
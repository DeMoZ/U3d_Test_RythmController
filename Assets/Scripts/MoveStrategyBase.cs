#define LOGGER_ON
using System;
using UnityEngine;

public interface IMoveStrategy : IDisposable
{
    void Init(InputModel inputModel, CharacterModel characterModel, CharacterController characterController,
        CharacterConfig characterConfig);
    void OnUpdate(float deltaTime);
}

public abstract class MoveStrategyBase : IMoveStrategy
{
    protected float _targetRotation;
    protected float _rotationVelocity;
    protected float _speed;
    protected Vector3 _velocity;
    protected float _verticalVelocity = -9.8f;
    protected Transform _transform;
    protected CharacterController _controller;
    protected CharacterConfig _characterConfig;
    protected InputModel _inputModel;
    protected CharacterModel _characterModel;

    // todo roman remove this speed hack
    protected float ConfigSpeed => _characterModel.IsRunning ? _characterConfig.SprintSpeed : _characterConfig.WalkSpeed;

    public void Dispose()
    {
    }

    public void Init(InputModel inputModel, CharacterModel characterModel, CharacterController characterController,
        CharacterConfig characterConfig)
    {
        _inputModel = inputModel;
        _characterModel = characterModel;
        _controller = characterController;
        _characterConfig = characterConfig;

        _transform = _controller.transform;
    }

    public void OnUpdate(float deltaTime)
    {
        if (_inputModel == null)
            return;

        var axis = _inputModel.OnMove.Value;
        OnMove(axis, deltaTime);

        var relativeVelocity = _velocity / ConfigSpeed;
        _characterModel.MoveSpeed.Value = _transform.InverseTransformDirection(relativeVelocity);
    }

    protected abstract void OnMove(Vector3 axis, float deltaTime);
}

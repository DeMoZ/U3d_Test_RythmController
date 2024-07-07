#define LOGGER_ON
using System;
using UnityEngine;

public interface IMoveStrategy : IDisposable
{
    void Init(InputModel inputModel, CharacterModel characterModel, CharacterController characterController,
        CharacterConfig characterConfig);
    void Update(float deltaTime);
}

public abstract class MoveStrategyBase : IMoveStrategy{
    protected float _targetRotation;
    protected float _rotationVelocity;
    protected float _speed;
    protected float _verticalVelocity;
    protected Transform _transform;

    protected CharacterController _controller;
    protected CharacterConfig _characterConfig;
    protected InputModel _inputModel;
    protected CharacterModel _characterModel;

    public void Dispose()
    {
    }

    public void Init(InputModel inputModel, CharacterModel characterModel,CharacterController characterController,
        CharacterConfig characterConfig)
    {
        _inputModel = inputModel;
        _characterModel = characterModel;
        _controller = characterController;
        _characterConfig = characterConfig;

        _transform = _controller.transform;
    }

    public void Update(float deltaTime)
    {
        if (_inputModel == null)
            return;

        var axis = _inputModel.OnMove.Value;
        OnMove(axis, deltaTime);
    }

    protected abstract void OnMove(Vector3 axis, float deltaTime);
}

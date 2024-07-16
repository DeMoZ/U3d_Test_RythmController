using System;
using UnityEngine;

/// <summary>
/// Logic for lock rotation to the target
/// </summary>
public interface IRotationStrategy : IDisposable
{
    void Init(InputModel inputModel, CharacterModel characterModel, CharacterController characterController,
        CharacterConfig characterConfig, GameBus gameBus);
    void OnUpdate(float deltaTime);
}

public abstract class RotateStrategyBase : IRotationStrategy
{
    protected float _targetRotation;
    protected float _rotationVelocity;
    protected Transform _transform;

    protected CharacterController _controller;
    protected CharacterConfig _characterConfig;
    protected GameBus _gameBus;
    protected InputModel _inputModel;
    protected CharacterModel _characterModel;

    // todo remove gamebus from dependency
    public void Init(InputModel inputModel, CharacterModel characterModel, CharacterController characterController,
        CharacterConfig characterConfig, GameBus gameBus)
    {
        _inputModel = inputModel;
        _characterModel = characterModel;
        _controller = characterController;
        _characterConfig = characterConfig;
        _gameBus = gameBus;

        _transform = _controller.transform;
    }

    public void Dispose()
    {
    }

    public void OnUpdate(float deltaTime)
    {
        if (_inputModel == null)
            return;

        var axis = _inputModel.OnMove.Value;
        OnRotate(axis, deltaTime);
    }

    protected abstract void OnRotate(Vector3 axis, float deltaTime);
}
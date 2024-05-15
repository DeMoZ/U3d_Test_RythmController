using System;
using Attack;
using UnityEngine.InputSystem;

public interface IInputController : IDisposable
{
}

/// <summary>
/// Triggers input actions of new input system
/// upon ui buttons events 
/// </summary>
public class InputController : IInputController
{
    private readonly InputBus _inputBus;
    private InputActionAsset _inputAsset;
    private InputAction _attackL;
    private InputAction _attackR;
    private InputAction _attackU;
    private InputAction _attackD;

    public InputController(InputBus inputBus, InputActionAsset inputAsset)
    {
        _inputBus = inputBus;
        _inputAsset = inputAsset;
        _inputAsset.Enable();

        _attackL = _inputAsset.FindAction("AttackL");
        _attackR = _inputAsset.FindAction("AttackR");
        _attackU = _inputAsset.FindAction("AttackU");
        _attackD = _inputAsset.FindAction("AttackD");

        _attackL.performed += OnLeftAttackPerformed;
        _attackR.performed += OnRightAttackPerformed;
        _attackU.performed += OnUpAttackPerformed;
        _attackD.performed += OnDownAttackPerformed;
    }

    private void OnLeftAttackPerformed(InputAction.CallbackContext context) =>
        SendAttackDirection(context, AttackDirection.L);

    private void OnRightAttackPerformed(InputAction.CallbackContext context) =>
        SendAttackDirection(context, AttackDirection.R);

    private void OnUpAttackPerformed(InputAction.CallbackContext context) =>
        SendAttackDirection(context, AttackDirection.U);

    private void OnDownAttackPerformed(InputAction.CallbackContext context) =>
        SendAttackDirection(context, AttackDirection.D);

    public void Dispose()
    {
        _attackL.performed -= OnLeftAttackPerformed;
        _attackR.performed -= OnRightAttackPerformed;
        _attackU.performed -= OnUpAttackPerformed;
        _attackD.performed -= OnDownAttackPerformed;

        _inputAsset.Disable();
    }

    private void SendAttackDirection(InputAction.CallbackContext context, AttackDirection attackDirection)
    {
        _inputBus.AttackClicked?.Invoke(attackDirection);
    }
}
using UnityEngine.InputSystem;
using Debug =  DMZ.DebugSystem.DMZLogger;

/// <summary>
/// Triggers input actions of new input system
/// upon ui buttons events 
/// </summary>
public class Input3x3Controller : IInputController
{
    private readonly Attack3x3Bus _inputBus;
    private InputActionAsset _inputAsset;
    private InputAction _attackL;

    public Input3x3Controller(Attack3x3Bus inputBus, InputActionAsset inputAsset)
    {
        _inputBus = inputBus;
        _inputAsset = inputAsset;
        _inputAsset.Enable();
        
        _attackL = _inputAsset.FindAction("AttackL");
        _attackL.started += OnAttackLStarted; // touch started
        _attackL.canceled += OnAttackLCanceled; // touch ended
    }

    private void OnAttackLCanceled(InputAction.CallbackContext obj)
    {
        _inputBus.OnAttackTouchEnded?.Invoke();
    }

    private void OnAttackLStarted(InputAction.CallbackContext obj)
    {
        _inputBus.OnAttackTouchStarted?.Invoke();
    }

    public void Dispose()
    {
        _attackL.started -= OnAttackLStarted;
        _attackL.canceled -= OnAttackLCanceled;
        _inputAsset.Disable();
    }
}
using UnityEngine.InputSystem;

public class PlayerInputStrategy : IInputStrategy
{
    private readonly InputActionAsset _inputAsset;
    private InputModel _inputModel;
    private InputAction _attackL;

    public PlayerInputStrategy(InputActionAsset inputAsset)
    {
        _inputAsset = inputAsset;
        _attackL = _inputAsset.FindAction("AttackL");
    }

    public void Init(InputModel inputModel)
    {
        _inputModel = inputModel;

        _inputAsset.Enable();
        _attackL.started += OnAttackLStarted; // touch started
        _attackL.canceled += OnAttackLCanceled; // touch ended
    }

    private void OnAttackLStarted(InputAction.CallbackContext obj)
    {
        _inputModel.OnAttackTouchStarted?.Invoke();
    }

    private void OnAttackLCanceled(InputAction.CallbackContext obj)
    {
        _inputModel.OnAttackTouchEnded?.Invoke();
    }

    public void Dispose()
    {
        _attackL.started -= OnAttackLStarted;
        _attackL.canceled -= OnAttackLCanceled;
        _inputAsset.Disable();
    }
}
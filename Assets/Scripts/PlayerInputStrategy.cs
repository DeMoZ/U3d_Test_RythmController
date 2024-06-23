//#define LOGGER_ON
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = DMZ.DebugSystem.DMZLogger;

public class PlayerInputStrategy : IInputStrategy
{
    private readonly InputActionAsset _inputAsset;
    private readonly UiJoyStick _uiJoyStick;

    private InputModel _inputModel;
    private InputAction _attackLAction;
    private InputAction _moveDigitalAction;

    private CancellationTokenSource _moveCancellationTokenSource;

    public PlayerInputStrategy(InputActionAsset inputAsset, UiJoyStick uiJoyStick)
    {
        _inputAsset = inputAsset;
        _uiJoyStick = uiJoyStick;
        _attackLAction = _inputAsset.FindAction("AttackL");
        _moveDigitalAction = _inputAsset.FindAction("MoveDigital");
    }

    public void Init(InputModel inputModel, Character character, GameBus gameBus)
    {
        _inputModel = inputModel;

        _attackLAction.started += OnAttackLActionStarted; // touch started
        _attackLAction.canceled += OnAttackLActionCanceled; // touch ended

        _moveDigitalAction.started += OnMoveDigitalStarted;
        _moveDigitalAction.canceled += OnMoveDigitalStopped;

        _uiJoyStick.OnJoysticOutput += OnJoystickOutput;
    }

    public void Dispose()
    {
        _attackLAction.started -= OnAttackLActionStarted;
        _attackLAction.canceled -= OnAttackLActionCanceled;

        _moveDigitalAction.started -= OnMoveDigitalStarted;
        _moveDigitalAction.canceled -= OnMoveDigitalStopped;

        _uiJoyStick.OnJoysticOutput -= OnJoystickOutput;

        _inputAsset.Disable();
    }

    private void OnAttackLActionStarted(InputAction.CallbackContext obj)
    {
        _inputModel.OnAttack?.Invoke(true);
    }

    private void OnAttackLActionCanceled(InputAction.CallbackContext obj)
    {
        _inputModel.OnAttack?.Invoke(false);
    }

    private void OnMoveDigitalStarted(InputAction.CallbackContext obj)
    {
        _moveCancellationTokenSource?.Cancel();
        _moveCancellationTokenSource = new CancellationTokenSource();
        DigitalInputAsync(_moveCancellationTokenSource.Token);
    }

    private async void DigitalInputAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                var value = _moveDigitalAction.ReadValue<Vector2>();

#if LOGGER_ON
                Debug.Log($"move {value}; mag {value.magnitude}");
#endif
                _inputModel.OnMove.Value = new Vector3(value.x, 0, value.y);
                await Task.Yield();
            }
        }
        catch (TaskCanceledException)
        {

        }
    }

    private void OnMoveDigitalStopped(InputAction.CallbackContext obj)
    {
        if (_moveCancellationTokenSource.IsCancellationRequested)
            return;

#if LOGGER_ON
        Debug.Log($"move {Vector2.zero}");
#endif
        _inputModel.OnMove.Value = Vector3.zero;
        _moveCancellationTokenSource?.Cancel();
    }

    private void OnJoystickOutput(Vector2 value)
    {
        _inputModel.OnMove.Value = new Vector3(value.x, 0, value.y);
    }
}
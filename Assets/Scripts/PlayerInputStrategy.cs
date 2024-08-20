//#define LOGGER_ON
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = DMZ.DebugSystem.DMZLogger;

public class PlayerInputStrategy : IInputStrategy
{
    private const string ACTION_NAME_AttackL = "AttackL";
    private const string ACTION_NAME_MoveDigital = "MoveDigital";

    private const string ACTION_NAME_SBlock0 = "SBlock0";
    private const string ACTION_NAME_WBlock0 = "WBlock0";
    private const string ACTION_NAME_WBlock1 = "WBlock1";
    private const string ACTION_NAME_WBlock2 = "WBlock2";

    private readonly InputActionAsset _inputAsset;
    private readonly UiJoyStick _uiJoyStick;

    private InputModel _inputModel;
    private InputAction _attackLAction;
    private InputAction _moveDigitalAction;

    private InputAction _sBlockAction;
    private InputAction _wBlock0Action;
    private InputAction _wBlock1Action;
    private InputAction _wBlock2Action;

    private CancellationTokenSource _moveCancellationTokenSource;

    public PlayerInputStrategy(InputActionAsset inputAsset, UiJoyStick uiJoyStick)
    {
        _inputAsset = inputAsset;
        _uiJoyStick = uiJoyStick;
        _attackLAction = _inputAsset.FindAction(ACTION_NAME_AttackL);
        _moveDigitalAction = _inputAsset.FindAction(ACTION_NAME_MoveDigital);

        _sBlockAction = _inputAsset.FindAction(ACTION_NAME_SBlock0);
        _wBlock0Action = _inputAsset.FindAction(ACTION_NAME_WBlock0);
        _wBlock1Action = _inputAsset.FindAction(ACTION_NAME_WBlock1);
        _wBlock2Action = _inputAsset.FindAction(ACTION_NAME_WBlock2);
    }

    public void Init(InputModel inputModel, Character character, GameBus gameBus)
    {
        _inputModel = inputModel;

        _attackLAction.started += OnButtonStarted; // touch started
        _attackLAction.canceled += OnButtonCanceled; // touch ended


        _sBlockAction.started += OnButtonStarted;
        _wBlock0Action.started += OnButtonStarted;
        _wBlock1Action.started += OnButtonStarted;
        _wBlock2Action.started += OnButtonStarted;

        _sBlockAction.canceled += OnButtonCanceled;
        _wBlock0Action.canceled += OnButtonCanceled;
        _wBlock1Action.canceled += OnButtonCanceled;
        _wBlock2Action.canceled += OnButtonCanceled;


        _moveDigitalAction.started += OnMoveDigitalStarted;
        _moveDigitalAction.canceled += OnMoveDigitalStopped;

        _uiJoyStick.OnJoysticOutput += OnJoystickOutput;
    }

    public void Dispose()
    {
        _attackLAction.started -= OnButtonStarted;
        _attackLAction.canceled -= OnButtonCanceled;

        _sBlockAction.started -= OnButtonStarted;
        _wBlock0Action.started -= OnButtonStarted;
        _wBlock1Action.started -= OnButtonStarted;
        _wBlock2Action.started -= OnButtonStarted;

        _sBlockAction.canceled -= OnButtonCanceled;
        _wBlock0Action.canceled -= OnButtonCanceled;
        _wBlock1Action.canceled -= OnButtonCanceled;
        _wBlock2Action.canceled -= OnButtonCanceled;

        _moveDigitalAction.started -= OnMoveDigitalStarted;
        _moveDigitalAction.canceled -= OnMoveDigitalStopped;

        _uiJoyStick.OnJoysticOutput -= OnJoystickOutput;

        _inputAsset.Disable();
    }

    private void OnButtonStarted(InputAction.CallbackContext obj)
    {
        Debug.Log($"Start Action {obj.action.name}");
        OnButton(obj, true);
    }

    private void OnButtonCanceled(InputAction.CallbackContext obj)
    {
        Debug.Log($"Cancelled Action {obj.action.name}");
        OnButton(obj, false);
    }

    private void OnButton(InputAction.CallbackContext obj, bool started)
    {
        switch (obj.action.name)
        {
            case ACTION_NAME_AttackL:
                _inputModel.OnAttack?.Invoke(started);
                break;
            case ACTION_NAME_SBlock0:
                _inputModel.OnBlock?.Invoke(started, BlockNames.SBlock0);
                break;
            case ACTION_NAME_WBlock0:
                _inputModel.OnBlock?.Invoke(started, BlockNames.WBlock0);
                break;
            case ACTION_NAME_WBlock1:
                _inputModel.OnBlock?.Invoke(started, BlockNames.WBlock1);
                break;
            case ACTION_NAME_WBlock2:
                _inputModel.OnBlock?.Invoke(started, BlockNames.WBlock2);
                break;
            default:
                Debug.LogError($"Unknown action {obj.action.name}");
                break;
        }
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

    // todo refactor this. Need to remove empty method and delete from interface
    // this caused by the bot update requirements for behaviour FSM
    public void OnUpdate(float deltaTime)
    {
    }
}
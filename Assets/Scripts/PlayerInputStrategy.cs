//#define LOGGER_ON

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = DMZ.DebugSystem.DMZLogger;

public class PlayerInputStrategy : IInputStrategy
{
    private const string ACTION_NAME_MoveDigital = "MoveDigital";

    private const string ACTION_NAME_Attack1 = "Attack1";

    private const string ACTION_NAME_Block1 = "Block1";
    private const string ACTION_NAME_Block2 = "Block2";
    private const string ACTION_NAME_Block3 = "Block3";
    private const string ACTION_NAME_Block4 = "Block4";

    private readonly InputActionAsset _inputAsset;
    private readonly UiJoyStick _uiJoyStick;

    private InputModel _inputModel;
    private InputAction _moveDigitalAction;

    private List<InputAction> _buttonActions;

    private CancellationTokenSource _moveCancellationTokenSource;

    public PlayerInputStrategy(InputActionAsset inputAsset, UiJoyStick uiJoyStick)
    {
        _inputAsset = inputAsset;
        _uiJoyStick = uiJoyStick;
        _moveDigitalAction = _inputAsset.FindAction(ACTION_NAME_MoveDigital);
        
        _buttonActions = new List<InputAction>()
        {
            _inputAsset.FindAction(ACTION_NAME_Attack1),
            
            _inputAsset.FindAction(ACTION_NAME_Block1),
            _inputAsset.FindAction(ACTION_NAME_Block2),
            _inputAsset.FindAction(ACTION_NAME_Block3),
            _inputAsset.FindAction(ACTION_NAME_Block4)
        };
    }

    public void Init(InputModel inputModel, Character character, GameBus gameBus)
    {
        _inputModel = inputModel;

        foreach (var inputAction in _buttonActions)
        {
            inputAction.started += OnButtonStarted; // touch started
            inputAction.canceled += OnButtonCanceled; // touch ended
        }

        _moveDigitalAction.started += OnMoveDigitalStarted;
        _moveDigitalAction.canceled += OnMoveDigitalStopped;

        _uiJoyStick.OnJoysticOutput += OnJoystickOutput;
    }

    public void Dispose()
    {
        foreach (var inputAction in _buttonActions)
        {
            inputAction.started -= OnButtonStarted;
            inputAction.canceled -= OnButtonCanceled;
        }
        
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
            case ACTION_NAME_Attack1:
                _inputModel.OnAttack?.Invoke(started, AttackNames.Attack1);
                break;
            
            case ACTION_NAME_Block1:
                _inputModel.OnBlock?.Invoke(started, BlockNames.Block1);
                break;
            case ACTION_NAME_Block2:
                _inputModel.OnBlock?.Invoke(started, BlockNames.Block2);
                break;
            case ACTION_NAME_Block3:
                _inputModel.OnBlock?.Invoke(started, BlockNames.Block3);
                break;
            case ACTION_NAME_Block4:
                _inputModel.OnBlock?.Invoke(started, BlockNames.Block4);
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
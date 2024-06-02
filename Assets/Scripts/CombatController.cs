using System;
using System.Threading;
using System.Threading.Tasks;
using DMZ.Extensions;
using UnityEngine;

public class CombatController
{
    private readonly InputModel _inputModel;
    private readonly CombatModel _combatModel;
    private readonly CombatRepository _combatRepository;
    
    private CancellationTokenSource _attackTokenSource;
    private CancellationTokenSource _horizontalTokenSource;

    private bool _isFailed;
    private bool _isTouching;

    public CombatController(InputModel inputModel, CombatModel combatModel, CombatRepository combatRepository)
    {
        _inputModel = inputModel;
        _combatModel = combatModel;
        _combatRepository = combatRepository;

        _inputModel.OnAttackTouchStarted += OnTouchStarted;
        _inputModel.OnAttackTouchEnded += OnTouchEnded;
    }

    public void Dispose()
    {
        _inputModel.OnAttackTouchStarted -= OnTouchStarted;
        _inputModel.OnAttackTouchEnded -= OnTouchEnded;
        _horizontalTokenSource?.Cancel();
        _attackTokenSource?.Cancel();
    }

    private void OnTouchStarted()
    {
        Debug.Log($"player fightSequenceState is {_combatModel.AttackSequenceState.Value}");

        _isTouching = true;

        switch (_combatModel.AttackSequenceState.Value)
        {
            case CombatState.None:
            case CombatState.Fail:
                Debug.Log("Attacking not available");
                break;
            case CombatState.Idle:
                _horizontalTokenSource = new CancellationTokenSource();
                HorizontalSequencingRecursive((0, 0), _horizontalTokenSource.Token);
                break;
            case CombatState.Pre:

                break;
            case CombatState.Attack:
                //SetFail();
                break;
            case CombatState.After:
                _horizontalTokenSource.Cancel();
                _attackTokenSource.Cancel();
                VerticalSequencing();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void OnTouchEnded()
    {
        if (!_isTouching || _isFailed)
            return;

        _horizontalTokenSource?.Cancel();
        _isTouching = false;

        Debug.Log($"player fightSequenceState is {_combatModel.AttackSequenceState.Value}");

        if (_combatModel.AttackSequenceState.Value is not CombatState.Pre)
            return;

        _attackTokenSource = new CancellationTokenSource();
        await AttackAsync(_attackTokenSource.Token);
    }

    private async void HorizontalSequencingRecursive((int, int) newCode, CancellationToken token)
    {
        if (token.IsCancellationRequested || !_combatRepository.IsSequenceExists(newCode))
            return;

        Debug.Log($"HorizontalSequencing ({newCode.Item1},{newCode.Item2})");
        _combatModel.CurrentSequenceKey.Value = newCode;
        _combatModel.AttackSequenceState.SetAndForceNotify(CombatState.Pre);

        await TimerProcessAsync(_combatRepository.GetPreAttackTime(newCode), token, null);
        if (token.IsCancellationRequested)
            return;

        Debug.Log($"HorizontalSequencing evaluate");

        newCode.Item2++;
        HorizontalSequencingRecursive(newCode, token);
    }

    private void VerticalSequencing()
    {
        Debug.Log($"VerticalSequencing evaluate");

        var newCode = _combatModel.CurrentSequenceKey.Value;
        newCode.Item1++;
        newCode.Item2 = 0;

        if (!_combatRepository.IsSequenceExists(newCode))
            newCode = (0, 0);

        _horizontalTokenSource = new CancellationTokenSource();
        HorizontalSequencingRecursive(newCode, _horizontalTokenSource.Token);
    }


    private void SetFail()
    {
        _isFailed = true;
        Debug.Log($"_isFailed: {_isFailed}");
    }

    private void OnBrake()
    {
        throw new NotImplementedException();
    }

    private async Task AttackAsync(CancellationToken cancellationToken)
    {
        var time = _combatRepository.GetAttackTime(_combatModel.CurrentSequenceKey.Value);
        try
        {
            await SequenceAsync(CombatState.Attack, time, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return;

            time = _combatRepository.GetPostAttackTime(_combatModel.CurrentSequenceKey.Value);
            await SequenceAsync(CombatState.After, time, cancellationToken, onEnd: SetIdle);
        }
        catch (TaskCanceledException)
        {
        }
    }

    private async Task SequenceAsync(CombatState state, float time, CancellationToken token,
        Action onCancel = null, Action onEnd = null, Action onFinal = null)
    {
        if (token.IsCancellationRequested)
            return;
        
        _combatModel.AttackSequenceState.Value = state;
        
        try
        {
            await TimerProcessAsync(time, token,
                progress =>
                {
                    var progress01 = Mathf.Clamp01(progress / time);
                    _combatModel.AttackProgress.Value =
                        new CombatProgressModel(state, _combatModel.CurrentSequenceKey.Value, progress, progress01);
                }, state.ToString());

            if (token.IsCancellationRequested)
                return;

            onEnd?.Invoke();
        }
        catch (TaskCanceledException)
        {
            if (onCancel != null)
            {
                Debug.Log($"Canceled {state}");
                onCancel?.Invoke();
            }
        }
    }

    private void SetIdle()
    {
        _combatModel.AttackSequenceState.Value = CombatState.Idle;
        _combatModel.CurrentSequenceKey.Value = (-1, -1);
    }

    private async Task TimerProcessAsync(float time, CancellationToken cancellationToken, Action<float> progress,
        string description = null)
    {
        float elapsedTime = 0;

        try
        {
            var startTime = Time.time;
            Debug.Log($"Timer started. {time} {description}");

            while (elapsedTime < time)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    OnTimerCancel();
                    return;
                }

                progress?.Invoke(elapsedTime);
                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                elapsedTime += 0.1f;
            }

            progress?.Invoke(time);
            Debug.Log($"Timer elapsed. {time} {description}; count {Time.time - startTime}".Green());
        }
        catch (TaskCanceledException)
        {
            OnTimerCancel();
        }

        return;

        void OnTimerCancel()
        {
            Debug.Log($"Timer cancelled.  {elapsedTime}/{time} {description}".Yellow());
        }
    }
}
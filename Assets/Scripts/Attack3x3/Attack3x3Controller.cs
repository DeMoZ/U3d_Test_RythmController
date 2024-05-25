using System;
using System.Threading;
using System.Threading.Tasks;
using DMZ.Extensions;
using UnityEngine;

public class Attack3x3Controller : Attack.IAttackController
{
    private readonly Attack3x3Bus _inputBus;
    private readonly Attack3x3PlayerData _attackPlayerData;
    private readonly Attack3x3Repository _attackRepository;

    private CancellationTokenSource _attackTokenSource;
    private CancellationTokenSource _horizontalTokenSource;

    private bool _isFailed;

    private float _touchStartTime;
    private float _touchEndTime;
    private float _touchLength;
    private bool _isTouchTimerRunning;

    public Attack3x3Controller(Attack3x3Bus inputBus,
        Attack3x3PlayerData attackPlayerData,
        Attack3x3Repository attackRepository)
    {
        _inputBus = inputBus;
        _attackPlayerData = attackPlayerData;
        _attackRepository = attackRepository;
        _inputBus.OnAttackTouchStarted += OnTouchStarted;
        _inputBus.OnAttackTouchEnded += OnTouchEnded;
    }

    public void Dispose()
    {
        _inputBus.OnAttackTouchStarted -= OnTouchStarted;
        _inputBus.OnAttackTouchEnded -= OnTouchEnded;
        _attackTokenSource?.Cancel();
    }

    private void OnTouchStarted()
    {
        Debug.Log($"player fightSequenceState is {_attackPlayerData.AttackSequenceState.Value}");

        _touchStartTime = Time.time;
        _isTouchTimerRunning = true;

        switch (_attackPlayerData.AttackSequenceState.Value)
        {
            case Attack3x3State.None:
            case Attack3x3State.Fail:
                Debug.Log("Attacking not available");
                break;
            case Attack3x3State.Idle:
                _horizontalTokenSource = new CancellationTokenSource();
                HorizontalSequencing((0, 0), _horizontalTokenSource.Token);
                break;
            case Attack3x3State.Pre:

                break;
            case Attack3x3State.Attack:
                //SetFail();
                break;
            case Attack3x3State.After:
                _attackTokenSource.Cancel();
                //EvaluateSequence();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private async void OnTouchEnded()
    {
        if (!_isTouchTimerRunning || _isFailed)
            return;

        _horizontalTokenSource?.Cancel();
        _isTouchTimerRunning = false;
        _touchEndTime = Time.time;
        _touchLength = _touchEndTime - _touchStartTime;

        Debug.Log($"player fightSequenceState is {_attackPlayerData.AttackSequenceState.Value}");

        if (_attackPlayerData.AttackSequenceState.Value is not Attack3x3State.Pre)
            return;
        //     _attackPlayerData.AttackSequenceState.Value = Attack3x3State.Attack;

        _attackTokenSource = new CancellationTokenSource();
        await AttackAsync();
    }

    private void SetPre()
    {
        // preattack state is also depends on the attack type. The trigger can be the same
        // todo roman _attackPlayerData.CurrentSequenceKey = SetAttackKey
        _attackPlayerData.AttackSequenceState.Value = Attack3x3State.Pre;
    }

    private async void HorizontalSequencing((int, int) newCode, CancellationToken token)
    {
        if (!_attackRepository.IsSequenceExists(newCode))
            return;

        Debug.Log($"HorizontalSequencing ({newCode.Item1},{newCode.Item2})");
        _attackPlayerData.CurrentSequenceKey = newCode;
        _attackPlayerData.AttackSequenceState.SetValueAndForceNotify(Attack3x3State.Pre);

        await TimerProcessAsync(_attackRepository.GetPreAttackTime(newCode), token, null);
        if (token.IsCancellationRequested)
            return;

        Debug.Log($"HorizontalSequencing evaluate");

        // await Pre time and if it ends, try run (x, ++)
        newCode.Item2++;
        HorizontalSequencing(newCode, token);
    }

    private void EvaluateSequence()
    {
        // var currentCode = _attackPlayerData.CurrentSequenceCode;
        // var newDirection = _newDirection.ToString();
        // var newCode = currentCode + newDirection;
        //
        // if (_attackRepository.TryGetSequence(newCode, out var element))
        // {
        //     Debug.Log($"EvaluateSequence success {_attackPlayerData.CurrentSequenceCode} > {newCode}");
        //     _attackPlayerData.CurrentSequenceCode = newCode;
        //     _attackPlayerData.CurrentSequenceElement = element;
        //     SetAttack();
        // }
        // else if (_attackRepository.TryGetSequence(newDirection, out element))
        // {
        //     Debug.Log($"EvaluateSequence success as new {_attackPlayerData.CurrentSequenceCode} > {newDirection}"
        //         .Yellow());
        //     _attackPlayerData.CurrentSequenceCode = newDirection;
        //     _attackPlayerData.CurrentSequenceElement = element;
        //     SetAttack();
        // }
        // else
        // {
        //     Debug.Log($"EvaluateSequence fail  {_attackPlayerData.CurrentSequenceCode} > {newCode}");
        //     SetIdle();
        // }
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

    private async Task AttackAsync()
    {
        var time = _attackRepository.GetAttackTime(_attackPlayerData.CurrentSequenceKey);
        try
        {
            await SequenceAsync(Attack3x3State.Attack, time, _attackTokenSource.Token);

            if (_attackTokenSource.IsCancellationRequested)
                return;

            //     if (_isFailed)
            //     {
            //         time = _attackRepository.GetFailTime(_attackPlayerData.CurrentSequenceCode);
            //         await SequenceAsync(AttackState.SequenceFail, time, _attackTokenSource.Token, onFinal: SetIdle);
            //         _isFailed = false;
            //     }
            //     else
            //     {
            time = _attackRepository.GetPostAttackTime(_attackPlayerData.CurrentSequenceKey);
            await SequenceAsync(Attack3x3State.After, time, _attackTokenSource.Token, onEnd: SetIdle);
            //     }
        }
        catch (TaskCanceledException)
        {
        }
    }

    private async Task SequenceAsync(Attack3x3State state, float time, CancellationToken token,
        Action onCancel = null, Action onEnd = null, Action onFinal = null)
    {
        _attackPlayerData.AttackSequenceState.Value = state;
        try
        {
            await TimerProcessAsync(time, token, null);
            // progress =>
            // {
            //     var progress01 = Mathf.Clamp01(progress / time);
            //     _attackPlayerData.AttackProgress.Value = new AttackProgressData(state, _newDirection, progress,
            //         progress01);
            // }, state.ToString());
            //
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
        // _attackPlayerData.CurrentSequenceCode = default;
        // _attackPlayerData.CurrentSequenceElement = default;
        // _attackPlayerData.AttackSequenceState.Value = AttackState.Idle;
        // _attackPlayerData.AttackProgress.Value = new AttackProgressData(AttackState.Idle, _newDirection, 0, 0);
        _attackPlayerData.AttackSequenceState.Value = Attack3x3State.Idle;
        _attackPlayerData.CurrentSequenceKey = (-1, -1);
    }

    // private async void SetAttack()
    // {
    //     _attackTokenSource = new CancellationTokenSource();
    //     await AttackAsync();
    // }

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
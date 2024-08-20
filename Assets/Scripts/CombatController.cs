//#define LOGGER_ON
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

public class CombatController
{
    private readonly InputModel _inputModel;
    private readonly CharacterModel _characterModel;
    private readonly ICombatRepository _combatRepository;

    private CancellationTokenSource _attackTokenSource;
    private CancellationTokenSource _horizontalTokenSource;

    private bool _isFailed;
    private bool _isTouching;

    public CombatController(InputModel inputModel, CharacterModel characterModel, ICombatRepository combatRepository)
    {
        _inputModel = inputModel;
        _characterModel = characterModel;
        _combatRepository = combatRepository;

        _inputModel.OnAttack += OnAttack;
        _inputModel.OnBlock += OnBlock;
    }

    public void Dispose()
    {
        _inputModel.OnAttack -= OnAttack;
        _inputModel.OnBlock -= OnBlock;
        _horizontalTokenSource?.Cancel();
        _attackTokenSource?.Cancel();
    }

    private void OnAttack(bool isStarted)
    {
        if (isStarted)
            OnTouchStarted();
        else
            OnTouchEnded();
    }

    private void OnTouchStarted()
    {
#if LOGGER_ON
        Debug.Log($"player fightSequenceState is {_characterModel.AttackSequenceState.Value}");
#endif
        _isTouching = true;

        switch (_characterModel.CombatPhaseState.Value)
        {
            case CombatPhase.None:
            case CombatPhase.Fail:
#if LOGGER_ON
                Debug.Log("Attacking not available");
#endif                
                break;
            case CombatPhase.Idle:
                _horizontalTokenSource = new CancellationTokenSource();
                HorizontalSequencingRecursive((0, 0), _horizontalTokenSource.Token);
                break;
            case CombatPhase.Pre:

                break;
            case CombatPhase.Attack:
                //SetFail();
                break;
            case CombatPhase.After:
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
#if LOGGER_ON
        Debug.Log($"player fightSequenceState is {_characterModel.AttackSequenceState.Value}");
#endif
        if (_characterModel.CombatPhaseState.Value is not CombatPhase.Pre)
            return;

        _attackTokenSource = new CancellationTokenSource();
        await AttackAsync(_attackTokenSource.Token);
    }

    private async void HorizontalSequencingRecursive((int, int) newCode, CancellationToken token)
    {
        if (token.IsCancellationRequested || !_combatRepository.IsSequenceExists(newCode))
            return;

#if LOGGER_ON
        Debug.Log($"HorizontalSequencing ({newCode.Item1},{newCode.Item2})");
#endif        
        _characterModel.CurrentSequenceKey.Value = newCode;
        _characterModel.CombatPhaseState.SetAndForceNotify(CombatPhase.Pre);

        await TimerProcessAsync(_combatRepository.GetPreAttackTime(newCode), token, null);
        if (token.IsCancellationRequested)
            return;
#if LOGGER_ON
        Debug.Log($"HorizontalSequencing evaluate");
#endif
        newCode.Item2++;
        HorizontalSequencingRecursive(newCode, token);
    }

    private void VerticalSequencing()
    {
#if LOGGER_ON        
        Debug.Log($"VerticalSequencing evaluate");
#endif
        var newCode = _characterModel.CurrentSequenceKey.Value;
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
#if LOGGER_ON
        Debug.Log($"_isFailed: {_isFailed}");
#endif        
    }

    private void OnBrake()
    {
        throw new NotImplementedException();
    }

    private async Task AttackAsync(CancellationToken cancellationToken)
    {
        var time = _combatRepository.GetAttackTime(_characterModel.CurrentSequenceKey.Value);
        try
        {
            await SequenceAsync(CombatPhase.Attack, time, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return;

            time = _combatRepository.GetPostAttackTime(_characterModel.CurrentSequenceKey.Value);
            await SequenceAsync(CombatPhase.After, time, cancellationToken, onEnd: SetIdle);
        }
        catch (TaskCanceledException)
        {
        }
    }

    private async Task SequenceAsync(CombatPhase state, float time, CancellationToken token,
        Action onCancel = null, Action onEnd = null, Action onFinal = null)
    {
        if (token.IsCancellationRequested)
            return;

        _characterModel.CombatPhaseState.Value = state;

        try
        {
            await TimerProcessAsync(time, token,
                progress =>
                {
                    var progress01 = Mathf.Clamp01(progress / time);
                    _characterModel.AttackProgress.Value =
                        new CombatProgressModel(state, _characterModel.CurrentSequenceKey.Value, progress, progress01);
                }, state.ToString());

            if (token.IsCancellationRequested)
                return;

            onEnd?.Invoke();
        }
        catch (TaskCanceledException)
        {
            if (onCancel != null)
            {
#if LOGGER_ON
                Debug.Log($"Canceled {state}");
#endif                
                onCancel?.Invoke();
            }
        }
    }

    private void SetIdle()
    {
        _characterModel.CombatPhaseState.Value = CombatPhase.Idle;
        _characterModel.CurrentSequenceKey.Value = (-1, -1);
    }

    private async Task TimerProcessAsync(float time, CancellationToken cancellationToken, Action<float> progress,
        string description = null)
    {
        float elapsedTime = 0;

        try
        {
            var startTime = Time.time;
#if LOGGER_ON
            Debug.Log($"Timer started. {time} {description}");
#endif
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
#if LOGGER_ON
            Debug.Log($"Timer elapsed. {time} {description}; count {Time.time - startTime}".Green());
#endif            
        }
        catch (TaskCanceledException)
        {
            OnTimerCancel();
        }

        return;

        void OnTimerCancel()
        {
#if LOGGER_ON
            Debug.Log($"Timer cancelled.  {elapsedTime}/{time} {description}".Yellow());
#endif
        }
    }

    #region Block

    private void OnBlock(bool isStarged, BlockNames blockName)
    {
        if (isStarged)
        {
            _characterModel.BlockPhaseState.SetAndForceNotify(BlockPhase.Pre, blockName);

            // todo roman implement BlockPhase.Block routine after BlockPhase.Pre time
            // this should be stopable in case that the character receives a damage

           // await Task.Yield(_combatRepository.GetPreBlockTime());

        }
        else
        {
            _characterModel.BlockPhaseState.SetAndForceNotify(BlockPhase.After, blockName);
        }
    }

    #endregion Block
}
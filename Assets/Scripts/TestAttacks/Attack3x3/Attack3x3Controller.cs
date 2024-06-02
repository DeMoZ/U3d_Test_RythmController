using System;
using System.Threading;
using System.Threading.Tasks;
using DMZ.Extensions;
using UnityEngine;

namespace Attack3x3
{
    public class Attack3x3Controller : Attack.IAttackController
    {
        private readonly Attack3x3Bus _inputBus;
        private readonly Attack3x3PlayerData _attackPlayerData;
        private readonly Attack3x3Repository _attackRepository;

        private CancellationTokenSource _attackTokenSource;
        private CancellationTokenSource _horizontalTokenSource;

        private bool _isFailed;
        private bool _isTouching;

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
            _horizontalTokenSource?.Cancel();
            _attackTokenSource?.Cancel();
        }

        private void OnTouchStarted()
        {
            Debug.Log($"player fightSequenceState is {_attackPlayerData.AttackSequenceState.Value}");

            _isTouching = true;

            switch (_attackPlayerData.AttackSequenceState.Value)
            {
                case Attack3x3State.None:
                case Attack3x3State.Fail:
                    Debug.Log("Attacking not available");
                    break;
                case Attack3x3State.Idle:
                    _horizontalTokenSource = new CancellationTokenSource();
                    HorizontalSequencingRecursive((0, 0), _horizontalTokenSource.Token);
                    break;
                case Attack3x3State.Pre:

                    break;
                case Attack3x3State.Attack:
                    //SetFail();
                    break;
                case Attack3x3State.After:
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

            Debug.Log($"player fightSequenceState is {_attackPlayerData.AttackSequenceState.Value}");

            if (_attackPlayerData.AttackSequenceState.Value is not Attack3x3State.Pre)
                return;

            _attackTokenSource = new CancellationTokenSource();
            await AttackAsync();
        }

        private async void HorizontalSequencingRecursive((int, int) newCode, CancellationToken token)
        {
            if (!_attackRepository.IsSequenceExists(newCode))
                return;

            Debug.Log($"HorizontalSequencing ({newCode.Item1},{newCode.Item2})");
            _attackPlayerData.CurrentSequenceKey.Value = newCode;
            _attackPlayerData.AttackSequenceState.SetAndForceNotify(Attack3x3State.Pre);

            await TimerProcessAsync(_attackRepository.GetPreAttackTime(newCode), token, null);
            if (token.IsCancellationRequested)
                return;

            Debug.Log($"HorizontalSequencing evaluate");

            newCode.Item2++;
            HorizontalSequencingRecursive(newCode, token);
        }

        private void VerticalSequencing()
        {
            Debug.Log($"VerticalSequencing evaluate");

            var newCode = _attackPlayerData.CurrentSequenceKey.Value;
            newCode.Item1++;
            newCode.Item2 = 0;

            if (!_attackRepository.IsSequenceExists(newCode))
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

        private async Task AttackAsync()
        {
            var time = _attackRepository.GetAttackTime(_attackPlayerData.CurrentSequenceKey.Value);
            try
            {
                await SequenceAsync(Attack3x3State.Attack, time, _attackTokenSource.Token);

                if (_attackTokenSource.IsCancellationRequested)
                    return;

                time = _attackRepository.GetPostAttackTime(_attackPlayerData.CurrentSequenceKey.Value);
                await SequenceAsync(Attack3x3State.After, time, _attackTokenSource.Token, onEnd: SetIdle);
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
                await TimerProcessAsync(time, token,
                    progress =>
                    {
                        var progress01 = Mathf.Clamp01(progress / time);
                        _attackPlayerData.AttackProgress.Value =
                            new Attack3x3PlayerData.AttackProgressData(state,
                                _attackPlayerData.CurrentSequenceKey.Value, progress, progress01);
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
            _attackPlayerData.AttackSequenceState.Value = Attack3x3State.Idle;
            _attackPlayerData.CurrentSequenceKey.Value = (-1, -1);
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
}
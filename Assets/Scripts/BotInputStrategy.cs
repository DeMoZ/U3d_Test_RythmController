using System;
using System.Threading;
using UnityEngine.AI;

public class BotInputStrategy : IInputStrategy
{
    private InputModel _inputModel;
    private Character _character;
    private GameBus _gameBus;
    private CancellationTokenSource _cancellationTokenSource;
    private StateMachineBase<BotStates> _botBehaviour;
    private NavMeshAgent _navMeshAgent;

    public BotInputStrategy()
    {
    }

    public void Init(InputModel inputModel, Character character, GameBus gameBus)
    {
        _inputModel = inputModel;
        _character = character;
        _gameBus = gameBus;

        _navMeshAgent = _character.NavMeshAgent;

        _botBehaviour = new BotBehaviour(_character, _gameBus, _inputModel, OnStateSchanged);
        _botBehaviour.RunStateMachine();

        // _cancellationTokenSource = new CancellationTokenSource();
        // ChaseAsync(_cancellationTokenSource.Token);
        // AttackAsync(_cancellationTokenSource.Token);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _botBehaviour.Dispose();
    }

    private void OnStateSchanged(BotStates state)
    {
        _character.ShowLog(0, state.ToString());

        // it is probably just for outside messages

        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        switch (state)
        {
            case BotStates.Idle:
                break;
            case BotStates.Chase:
                //InputChaseAsync(_cancellationTokenSource.Token);
                break;
            case BotStates.Attack:
                //InputAttackAsync(_cancellationTokenSource.Token);
                break;
            case BotStates.Return:
                //_navMeshAgent.SetDestination(_character.SpawnPosition);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }


    // private async void InputChaseAsync(CancellationToken token)
    // {
    //     try
    //     {
    //         while (!token.IsCancellationRequested)
    //         {
    //             _navMeshAgent.SetDestination(_gameBus.Player.Transform.position);

    //             if (_navMeshAgent.hasPath)
    //             {
    //                 var desiredMovement = _navMeshAgent.desiredVelocity;
    //                 desiredMovement.y = 0;

    //                 _character.SetStatus($"{BotStates.Chase} {desiredMovement}");
    //                 _inputModel.OnMove.Value = new Vector3(Mathf.Clamp(desiredMovement.x, -1f, 1f), 0, Mathf.Clamp(desiredMovement.z, -1f, 1f));
    //             }

    //             await Task.Delay(1, token);
    //             if (_cancellationTokenSource.IsCancellationRequested)
    //                 return;
    //         }
    //     }
    //     catch (TaskCanceledException)
    //     {
    //     }
    // }

    // private async void InputAttackAsync(CancellationToken token)
    // {
    //     try
    //     {
    //         while (!token.IsCancellationRequested)
    //         {
    //             await Task.Delay((int)(GetRandomTime(3, 6) * 1000), token);
    //             if (_cancellationTokenSource.IsCancellationRequested)
    //                 return;

    //             _inputModel.OnAttack?.Invoke(true);

    //             await Task.Delay((int)(GetRandomTime(0.01f, 3f) * 1000), token);
    //             if (token.IsCancellationRequested)
    //                 return;

    //             _inputModel.OnAttack?.Invoke(false);
    //         }
    //     }
    //     catch (TaskCanceledException)
    //     {
    //     }

    //     double GetRandomTime(float min, float max)
    //     {
    //         // return new Random().NextDouble() * (max - min) + min;
    //         return 1;
    //     }
    // }
}
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

        // todo remove
        _inputModel.IsRunning.Value = true;
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
}
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Debug = DMZ.DebugSystem.DMZLogger;

public class ReturnState : StateBase<BotStates>
{
    private const float _returnToIdle = 0.2f;
    private readonly InputModel _inputModel;
    private readonly NavMeshAgent _navMeshAgent;

    public override BotStates Type { get; } = BotStates.Return;

    public ReturnState(Character character, GameBus gameBus, InputModel inputModel) : base(character, gameBus)
    {
        _inputModel = inputModel;
        _navMeshAgent = character.NavMeshAgent;
    }

    public override BotStates Update()
    {
        if (IsInRange(_character.SpawnPosition, _returnToIdle))
            return BotStates.Idle;

        if (IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.chaseRange))
            return BotStates.Chase;

        GetInput();

        return Type;
    }

    public override async Task EnterAsync(CancellationToken token)
    {
        Debug.Log($"Enter");
        _character.ShowLog(2, $"{_inputModel.OnMove.Value}");
        _navMeshAgent.enabled = true;
        await Task.Yield();
    }

    public override async Task ExitAsync(CancellationToken token)
    {
        _navMeshAgent.enabled = false;
        _inputModel.OnMove.Value = Vector3.zero;
        Debug.Log($"Exit");
        _character.ShowLog(2, $"{_inputModel.OnMove.Value}");

        await Task.Yield();
    }

    private void GetInput()
    {
        _navMeshAgent.SetDestination(_character.SpawnPosition);

        if (_navMeshAgent.hasPath)
        {
            var desiredMovement = _navMeshAgent.desiredVelocity;
            desiredMovement.y = 0;
            _inputModel.OnMove.Value = new Vector3(Mathf.Clamp(desiredMovement.x, -1f, 1f), 0, Mathf.Clamp(desiredMovement.z, -1f, 1f));
            _character.ShowLog(1, $"{desiredMovement}");
            _character.ShowLog(2, $"{_inputModel.OnMove.Value}");
        }
    }
}
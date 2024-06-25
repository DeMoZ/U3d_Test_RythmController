using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Debug = DMZ.DebugSystem.DMZLogger;

public class ChaseState : StateBase<BotStates>
{
    private readonly InputModel _inputModel;
    private readonly NavMeshAgent _navMeshAgent;

    public override BotStates Type { get; } = BotStates.Chase;

    public ChaseState(Character character, GameBus gameBus, InputModel inputModel) : base(character, gameBus)
    {
        _inputModel = inputModel;
        _navMeshAgent = _character.NavMeshAgent;
    }

    public override BotStates Update()
    {
        if (IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.meleAttackRange))
            return BotStates.Attack;

        if (!IsInRange(_gameBus.Player.Transform.position, _character.CharacterConfig.chaseStopRange))
            return BotStates.Return;

        GetInput();
        return Type;
    }

    public override async Task EnterAsync(CancellationToken token)
    {
        Debug.Log($"Enter");
        _navMeshAgent.enabled = true;
        await Task.Yield();
    }

    public override async Task ExitAsync(CancellationToken token)
    {
        _navMeshAgent.enabled = false;
        _inputModel.OnMove.Value = Vector3.zero;
        Debug.Log($"Exit");
        await Task.Yield();
    }

    private void GetInput()
    {
        _navMeshAgent.SetDestination(_gameBus.Player.Transform.position);

        if (_navMeshAgent.hasPath)
        {
            var desiredMovement = _navMeshAgent.desiredVelocity;
            desiredMovement.y = 0;

            _character.ShowLog(1, $"{desiredMovement}");
            _inputModel.OnMove.Value = new Vector3(Mathf.Clamp(desiredMovement.x, -1f, 1f), 0, Mathf.Clamp(desiredMovement.z, -1f, 1f));
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using Debug = DMZ.DebugSystem.DMZLogger;

public class NavMeshState : StateBase<BotStates>
{
    protected readonly InputModel _inputModel;
    protected readonly NavMeshAgent _navMeshAgent;
    protected NavMeshPath _navMeshPath;

    public NavMeshState(Character character, GameBus gameBus) : base(character, gameBus)
    {
        _inputModel = character.InputModel;
        _navMeshAgent = _character.NavMeshAgent;
    }

    public override async Task EnterAsync(CancellationToken token)
    {
        Debug.Log($"{GetType()} Enter");
        _navMeshAgent.enabled = true;
        _navMeshPath = new NavMeshPath();
        _character.CharacterModel.OnMovePathEnable?.Invoke(true);

        await Task.Yield();
    }

    public override async Task ExitAsync(CancellationToken token)
    {
        _navMeshAgent.enabled = false;
        _inputModel.OnMove.Value = Vector3.zero;
        _character.CharacterModel.OnMovePathEnable?.Invoke(false);
        Debug.Log($"{GetType()} Exit");
        await Task.Yield();
    }

    protected void CalculateInput(Vector3 toPoint)
    {
        if (_navMeshAgent.CalculatePath(toPoint, _navMeshPath))
        {
            _character.CharacterModel.OnMovePath?.Invoke(_navMeshPath.corners);
            var navMeshInput = CalculateDesiredVelocity(_navMeshAgent, _navMeshPath.corners);
            var clampedInput = new Vector3(Mathf.Clamp(navMeshInput.x, -1f, 1f), 0, Mathf.Clamp(navMeshInput.z, -1f, 1f));
            _inputModel.OnMove.Value = clampedInput;
            _character.ShowLog(1, $"{navMeshInput}");
            _character.ShowLog(2, $"{clampedInput}");
        }
    }

    private Vector3 CalculateDesiredVelocity(NavMeshAgent agent, Vector3[] corners)
    {
        if (corners.Length < 2)
            return Vector3.zero;

        var direction = (corners[1] - agent.transform.position).normalized;

        // var desiredVelocity = direction * agent.speed;
        // return desiredVelocity;
        var desiredVelocity = direction;

        return desiredVelocity;
    }
}

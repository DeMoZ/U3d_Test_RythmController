using System;
using UnityEngine;
using UnityEngine.AI;
using Debug = DMZ.DebugSystem.DMZLogger;

public class NavMeshState<T> : StateBase<T> where T : Enum
{
    protected readonly InputModel _inputModel;
    protected readonly NavMeshAgent _navMeshAgent;
    protected NavMeshPath _navMeshPath;

    public NavMeshState(Character character) : base(character)
    {
        _inputModel = character.InputModel;
        _navMeshAgent = _character.NavMeshAgent;
    }

    public override void Enter()
    {
        base.Enter();
        _navMeshPath = new NavMeshPath();
        _characterModel.OnMovePathEnable?.Invoke(true);
        // todo nav mesh is not enabled on the first update on spawn. Why?
        _navMeshAgent.enabled = true;
    }

    public override void Exit()
    {
        base.Exit();
        _navMeshAgent.enabled = false;
        _inputModel.OnMove.Value = Vector3.zero;
        _characterModel.OnMovePathEnable?.Invoke(false);
    }

    protected void CalculateInput(Vector3 toPoint)
    {
        // todo nav mesh is not enabled on the first update on spawn. Why?
        if (!_navMeshAgent.isActiveAndEnabled)
        {
            _navMeshAgent.enabled = true;
            return;
        }

        if (_navMeshAgent.CalculatePath(toPoint, _navMeshPath))
        {
            _characterModel.OnMovePath?.Invoke(_navMeshPath.corners);
            var navMeshInput = CalculateDesiredVelocity(_navMeshPath.corners);
            var clampedInput = new Vector3(Mathf.Clamp(navMeshInput.x, -1f, 1f), 0, Mathf.Clamp(navMeshInput.z, -1f, 1f));
            _inputModel.OnMove.Value = clampedInput;
            _character.ShowLog(1, $"{navMeshInput}");
            _character.ShowLog(2, $"{clampedInput}");
        }
    }

    protected Vector3 CalculateDesiredVelocity(Vector3[] corners)
    {
        if (corners.Length < 2)
            return Vector3.zero;

        var direction = (corners[1] - _navMeshAgent.transform.position).normalized;

        // var desiredVelocity = direction * agent.speed;
        // return desiredVelocity;
        var desiredVelocity = direction;

        return desiredVelocity;
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

// todo roman need to avoid moving to not movable points
public class AttackRepositionSubState : NavMeshState<AttackSubStates>
{
    // todo roman move to config
    private const float REPOSITION_TIME = 3f;
    private const float DEGREE_STEP = 20f;

    private (float min, float max) ANGLE_RANGE = (DEGREE_STEP, 180f);
    private float _timer;
    private bool _isMoving;
    private float _angle;
    private Queue<Vector3> _targetLocalPath;
    private Vector3 _nextPoint;

    public override AttackSubStates Type { get; } = AttackSubStates.Reposition;

    public AttackRepositionSubState(Character character) : base(character)
    {

    }

    public override void Enter()
    {
        base.Enter();
        _timer = REPOSITION_TIME;
        _isMoving = false;
        CreatePath();
    }

    public override AttackSubStates Update(float deltaTime)
    {
        _timer -= deltaTime;
        if (_timer <= 0 || _characterModel.Target.Value == null)
            return AttackSubStates.Countdown;

        return IsFollowingPath() ? Type : AttackSubStates.Countdown;
    }

    private bool IsFollowingPath()
    {
        if (_isMoving)
        {
            CheckNextPointDistance();
            return CalculateInput(_nextPoint + _characterModel.Target.Value.Transform.position);
        }
        else if (_targetLocalPath.Count > 0)
        {
            _nextPoint = _targetLocalPath.Dequeue();
            _isMoving = CalculateInput(_nextPoint + _characterModel.Target.Value.Transform.position);
            return _isMoving;
        }

        _isMoving = false;
        return false;
    }
    
    /// <summary>
    /// Calculate random point and path around the target
    /// </summary>
    private void CreatePath()
    {
        var target = _characterModel.Target.Value.Transform;
        if (target != null)
        {
            _angle = GetRandomInRange(ANGLE_RANGE.min, ANGLE_RANGE.max);
            _targetLocalPath = GetLocalPath(_angle, target);

            var worldPath = new List<Vector3>();
            foreach (var point in _targetLocalPath)
                worldPath.Add(point + target.position);

            _characterModel.OnMovePath?.Invoke(worldPath.ToArray());
        }
    }

    private Queue<Vector3> GetLocalPath(float angle, Transform target)
    {
        float stepRadians = DEGREE_STEP * Mathf.Deg2Rad;
        float totalAngleRadians = angle * Mathf.Deg2Rad;
        var distance = _characterConfig.MeleAttackRange;
        var result = new Queue<Vector3>((int)Mathf.Ceil(totalAngleRadians / stepRadians));
        var direction = _characterModel.Transform.position - target.position;
        var startPoint = direction * distance;
        var crossY = Vector3.Cross(target.forward, direction).y;

        float sign;
        if (Mathf.Approximately(crossY, 0))
            sign = GetRandomInRange(0, 1) > 0.5f ? 1 : -1;
        else
            sign = crossY > 0 ? 1 : -1;

        Quaternion rotation = Quaternion.Euler(0, DEGREE_STEP * sign, 0);

        var isSkipped = false;
        for (float currentAngle = 0; currentAngle < totalAngleRadians; currentAngle += stepRadians)
        {
            Vector3 rotatedPoint = rotation * startPoint;

            if (isSkipped)
                result.Enqueue(rotatedPoint.normalized * distance);

            startPoint = rotatedPoint;
            isSkipped = true;
        }

        return result;
    }

    /// <summary>
    /// Avoid going back when target moves in opposite direction
    /// </summary>
    private void CheckNextPointDistance()
    {
        if (_targetLocalPath.Count > 0)
        {
            var currentPosition = _navMeshAgent.transform.position;
            var targetPosition = _characterModel.Target.Value.Transform.position;

            var nextPointPosition = _targetLocalPath.Peek() + targetPosition;
            var nextDistance = Vector3.SqrMagnitude(currentPosition - nextPointPosition);

            var nextPoint = _nextPoint + targetPosition;
            var curDistance = Vector3.SqrMagnitude(currentPosition - nextPoint);

            if (nextDistance < curDistance)
                _nextPoint = _targetLocalPath.Dequeue();
        }
    }

    protected new bool CalculateInput(Vector3 toPoint)
    {
        // todo nav mesh is not enabled on the first update on spawn. Why?
        if (!_navMeshAgent.isActiveAndEnabled)
        {
            _navMeshAgent.enabled = true;
            return true; // skip calculation due to nav mesh not enabled but retunrning true to not break the state machine
        }

        if (!_navMeshAgent.CalculatePath(toPoint, _navMeshPath) || Vector3.Distance(_navMeshAgent.transform.position, toPoint) < 0.3f)
            return false;

        var drowablePath = _navMeshPath.corners.ToList();
        drowablePath.AddRange(_targetLocalPath.Select(x => x + _characterModel.Target.Value.Transform.position));
        _characterModel.OnMovePath?.Invoke(drowablePath.ToArray());

        var navMeshInput = CalculateDesiredVelocity(_navMeshPath.corners);
        var clampedInput = new Vector3(Mathf.Clamp(navMeshInput.x, -1f, 1f), 0, Mathf.Clamp(navMeshInput.z, -1f, 1f));
        _inputModel.OnMove.Value = clampedInput;
        _character.ShowLog(1, $"{navMeshInput}");
        _character.ShowLog(2, $"{clampedInput}");

        return clampedInput != Vector3.zero;
    }
}
using System.Collections.Generic;
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

// todo roman show full path, not only one segment
// todo roman need to avoid moving to not movable points
public class AttackRepositionSubState : NavMeshState<AttackSubStates>
{
    // todo roman move to config
    private const float REPOSITION_TIME = 2f;
    private const float DEGREE_STEP = 20f;

    private (float min, float max) ANGLE_RANGE = (DEGREE_STEP, 180f);
    private float _timer;
    private bool _isMoving;
    private float _angle;
    private Queue<Vector3> _localPath;
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

    private void CreatePath()
    {
        // todo calculate random point and path around target
        var target = _characterModel.Target.Value;
        if (target != null)
        {
            _angle = GetRandomInRange(ANGLE_RANGE.min, ANGLE_RANGE.max);
            _localPath = GetLocalPath(_angle, target);

            var worldPath = new List<Vector3>();
            foreach (var point in _localPath)
                worldPath.Add(point + target.position);

            _characterModel.OnMovePath?.Invoke(worldPath.ToArray());
            // TempDrawPath(target, worldPath);
        }
    }

    // // todo roman remove test method
    // private static void TempDrawPath(Transform target, List<Vector3> worldPath)
    // {
    //     var go = new GameObject("RepositionObject");
    //     go.transform.position = target.position;
    //     var line = go.AddComponent<LineRenderer>();
    //     line.widthMultiplier = 0.01f;
    //     line.useWorldSpace = true;
    //     line.positionCount = worldPath.Count;
    //     line.SetPositions(worldPath.ToArray());
    // }

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

        for (float currentAngle = 0; currentAngle < totalAngleRadians; currentAngle += stepRadians)
        {
            Vector3 rotatedPoint = rotation * startPoint;
            result.Enqueue(rotatedPoint.normalized * distance);
            startPoint = rotatedPoint;
        }

        return result;
    }

    public override AttackSubStates Update(float deltaTime)
    {
        _timer -= deltaTime;
        if (_timer <= 0 || _characterModel.Target.Value == null)
            return AttackSubStates.Countdown;

        var isFollowingPath = IsFollowingPath();

        return isFollowingPath ? Type : AttackSubStates.Countdown;
    }

    private bool IsFollowingPath()
    {
        if (_isMoving)
        {
            var distance = Vector3.Distance(_nextPoint, _characterModel.Transform.position);
            if (distance < 0.1f)
                _isMoving = false;

            return true;
        }
        else
        {
            if (_localPath.Count > 0)
            {
                var target = _characterModel.Target.Value;
                if (target != null)
                {
                    _isMoving = true;
                    _nextPoint = _localPath.Dequeue() + target.position;
                    CalculateInput(_nextPoint);
                    return true;
                }
                else
                {
                    _isMoving = false;
                    _localPath.Clear();
                }
            }
        }

        return false;
    }
}
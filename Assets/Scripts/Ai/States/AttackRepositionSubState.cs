using System.Collections.Generic;
using UnityEngine;
using Debug = DMZ.DebugSystem.DMZLogger;

public class AttackRepositionSubState : NavMeshState<AttackSubStates>
{
    // todo roman move to config
    private const float REPOSITION_TIME = 2f;
    private const float step = 20f;

    private (float min, float max) ANGLE_RANGE = (60f, 250f);
    private float _timer;
    private float _angle;
    private Queue<Vector3> _localPath;

    public override AttackSubStates Type { get; } = AttackSubStates.Reposition;

    public AttackRepositionSubState(Character character) : base(character)
    {

    }

    public override void Enter()
    {
        base.Enter();
        _timer = REPOSITION_TIME;
        CreatePath();
    }

    private void CreatePath()
    {
        // todo calculate random point and path around target
        var target = _characterModel.Target.Value;
        if (target != null)
        {
            _angle = GetRandomInRange(ANGLE_RANGE.min, ANGLE_RANGE.max);

            _localPath = GetLocalPath(_angle);

            var worldPath = new List<Vector3>();
            foreach (var point in _localPath)
            {
                worldPath.Add(point + target.position);
            }

            // todo roman remove test block
            {
                var go = new GameObject("Reposition object");
                go.transform.position = target.position;
                var line = go.AddComponent<LineRenderer>();
                line.widthMultiplier = 0.01f;
                line.useWorldSpace = true;
                line.positionCount = worldPath.Count;
                line.SetPositions(worldPath.ToArray());
            }
            _characterModel.OnMovePath?.Invoke(worldPath.ToArray());

            Debug.LogError($"Reposition path: {worldPath.Count}");
        }
        else
        {
            Debug.LogError($"No target at {_character.gameObject.name}");
        }
    }

    private Queue<Vector3> GetLocalPath(float angle)
    {
        var result = new Queue<Vector3>();
        float stepRadians = step * Mathf.Deg2Rad;
        float totalAngleRadians = 130f * Mathf.Deg2Rad;

        for (var i = 0f; i < totalAngleRadians; i += stepRadians)
        {
            var coord = new Vector2(Mathf.Sin(i), Mathf.Cos(i)).normalized * 0.01f;
            var direction = new Vector3(coord.x, 0, coord.y) * angle;
            result.Enqueue(direction);
        }

        return result;
    }

    public override AttackSubStates Update(float deltaTime)
    {
        _timer -= deltaTime;
        if (_timer <= 0 || _characterModel.Target.Value == null)
            return AttackSubStates.Countdown;

        // todo move path relative updated target position. If path became blocked, recalculate new position and path 

        return Type;
    }
}
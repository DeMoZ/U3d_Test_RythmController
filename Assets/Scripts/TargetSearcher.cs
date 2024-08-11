using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetSearcher : IDisposable
{
    private readonly CharacterModel _characterModel;
    private readonly CharacterConfig _characterConfig;
    private readonly List<ITargetable> _targets;

    public TargetSearcher(CharacterModel characterModel, CharacterConfig characterConfig, List<ITargetable> targets)
    {
        _characterModel = characterModel;
        _characterConfig = characterConfig;
        _targets = targets;
    }

    public void Dispose()
    {
        _targets.Clear();
    }

    public void OnUpdate()
    {
        ITargetable target = null;
        var minDistanceSquared = float.MaxValue;
        var chaseRangeSquared = _characterConfig.ChaseRange * _characterConfig.ChaseRange;

        foreach (var sceneTarget in _targets.Where(t => t.Transform != null))
        {
            float distanceSquared = (sceneTarget.Transform.position - _characterModel.Transform.position).sqrMagnitude;
            if (distanceSquared < minDistanceSquared)
            {
                minDistanceSquared = distanceSquared;
                target = sceneTarget;
            }
        }

        _characterModel.Target.Value = minDistanceSquared <= chaseRangeSquared ? target : null;
    }
}
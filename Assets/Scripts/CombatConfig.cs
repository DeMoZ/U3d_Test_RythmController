using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/" + nameof(CombatConfig), fileName = nameof(CombatConfig))]
public class CombatConfig : SerializedScriptableObject
{
    [Header("Default timings")] public float PreAttackTime = 0.2f;
    public float AttackTime = 0.4f;
    public float PostAttackTime = 0.4f;
    public float FailTime = 1f;

    public List<List<AttackElement>> Sequences;
}

public class AttackElement
{
    public float? PreAttackTime;
    public float? AttackTime;
    public float? PostAttackTime;
    public float? FailTime;
    public (int, int) Code { get; private set; }
    public string StrCode { get; private set; }

    public void Init(int sequenceIndex, int powerIndex)
    {
        Code = (sequenceIndex, powerIndex);
        StrCode = $"{sequenceIndex}{powerIndex}";
    }
}
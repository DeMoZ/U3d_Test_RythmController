using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/" + nameof(CharacterConfig), fileName = nameof(CharacterConfig))]
public class CharacterConfig : SerializedScriptableObject
{
    [field: SerializeField] public float SpeedOffset { get; private set; } = 0.1f;
    [field: SerializeField] public float WalkSpeed { get; private set; } = 2.0f;
    [field: SerializeField] public float SprintSpeed { get; private set; } = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    //[Range(0.0f, 0.3f)]
    [field: SerializeField] public float RotationSmoothTime { get; private set; } = 0.2f;
    [field: SerializeField] public float ActingRotationSmoothTime { get; private set; } = 0.1f;

    [Tooltip("Acceleration and deceleration")]
    [field: SerializeField] public float SpeedChangeRate { get; private set; } = 10.0f;

    //[Space]
    [field: SerializeField] public float MeleAttackRange { get; private set; } = 2;
    [field: SerializeField] public float ChaseRange { get; private set; } = 4;
    [field: SerializeField] public float ChaseStopRange { get; private set; } = 5;

    [Tooltip("Angle that enought for bot to attack target")]
    [field: SerializeField] public float AttackRotationAngle { get; private set; } = 10;
}
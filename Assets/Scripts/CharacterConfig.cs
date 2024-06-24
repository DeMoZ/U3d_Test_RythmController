using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/" + nameof(CharacterConfig), fileName = nameof(CharacterConfig))]
public class CharacterConfig : SerializedScriptableObject
{
    public float SpeedOffset = 0.1f;
    public float walkSpeed = 2.0f;
    public float sprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float rotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float speedChangeRate = 10.0f;
    
    [Space]
    public float meleAttackRange = 2;
    public float chaseRange = 4;
    public float chaseStopRange = 5;
}
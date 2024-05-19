using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public Animator Animator => animator;
}

using UnityEngine;

namespace Attack3x3
{
    public class Character : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public Animator Animator => animator;
    }
}
using UnityEngine;

namespace Ui
{
    public class UiAttackGroup : MonoBehaviour
    {
        [SerializeField] private OnScreenControlButton[] attackButtons;

        private void Awake()
        {
            foreach (var button in attackButtons)
                button.OnInteract += OnInteract;
        }

        private void OnInteract(OnScreenControlButton controlButton, float value)
        {
            if (false) // todo roman check some game state 
                return;

            controlButton.SendValue(value);
        }

        private void OnDestroy()
        {
            foreach (var button in attackButtons)
                button.OnInteract -= OnInteract;
        }
    }
}
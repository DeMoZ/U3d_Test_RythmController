using UnityEngine;

namespace Ui
{
    public class UiButtonsGroup : MonoBehaviour
    {
        [SerializeField] private OnScreenControlButton[] buttons;

        private void Awake()
        {
            foreach (var button in buttons)
                button.OnInteract += OnInteract;
        }

        private void OnInteract(OnScreenControlButton controlButton, float value)
        {
            if (!CheckInteractionAllowed()) 
                return;

            controlButton.SendValue(value);
            return;

            // todo roman inmplement CheckInteractionAllowed
            bool CheckInteractionAllowed() => true;
        }
        
        private void OnDestroy()
        {
            foreach (var button in buttons)
                button.OnInteract -= OnInteract;
        }
    }
}
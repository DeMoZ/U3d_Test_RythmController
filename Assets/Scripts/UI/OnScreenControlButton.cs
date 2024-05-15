using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace Ui
{
    public class OnScreenControlButton : OnScreenControl, IPointerDownHandler, IPointerUpHandler
    {
        [InputControl(layout = "Button")] [SerializeField]
        private string actionPath;

        public event Action<OnScreenControlButton, float> OnInteract;

        public void OnPointerUp(PointerEventData data)
        {
            OnInteract?.Invoke(this, 0);
        
        }

        public void OnPointerDown(PointerEventData data)
        {
            OnInteract?.Invoke(this, 1);
        }
        
        protected override string controlPathInternal
        {
            get => actionPath;
            set => actionPath = value;
        }

        public void SendValue(float value)
        {
            SendValueToControl(value);
        }
    }
}
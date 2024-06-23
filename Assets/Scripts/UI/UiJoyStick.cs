// #define LOGGER_ON

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Debug = DMZ.DebugSystem.DMZLogger;
using Sirenix.OdinInspector;
using System;
// using UnityEngine.InputSystem.Layouts;
// using UnityEngine.InputSystem.OnScreen;

public class UiJoyStick : MonoBehaviour/*OnScreenControl*/, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    // [SerializeField] private bool useDinamicArea;
    // [ShowIf(nameof(useDinamicArea))]
    [SerializeField] private RectTransform area;
    // [ShowIf(nameof(useDinamicArea))]
    // [SerializeField] private bool startFromTouchCenter;
    // [ShowIf(nameof(useDinamicArea))]
    // [SerializeField] private bool followTouch;
    [Space]
    [SerializeField] private RectTransform plate;
    [SerializeField] private RectTransform handle;
    [Space]
    [SerializeField] private bool setHandleRange;
    [ShowIf(nameof(setHandleRange))]
    [SerializeField] private float joystickRange = 50f;
    [ShowIf(nameof(IsSetHandleRangeVisible))]
    [SerializeField] private float handleOffset = 5f;
    [Space]
    [SerializeField] private float magnitudeMultiplier = 1f;
    [SerializeField] private bool invertXOutputValue;
    [SerializeField] private bool invertYOutputValue;
    [Space]
    // [SerializeField] private bool useActionPath;
    // [ShowIf(nameof(useActionPath))]
    // [InputControl(layout = "Button")]
    // [SerializeField] private string actionPath;
    [Space]
    [SerializeField] private UnityEvent<Vector2> onJoystickOutput;

    private float _joystickRange = 50f;

    public Action<Vector2> OnJoysticOutput { get; set; }

    void Start()
    {
        _joystickRange = setHandleRange ? joystickRange : CalculateJoystickRange();
#if LOGGER_ON
        Debug.Log($"Start _joystickRange = {_joystickRange}");
#endif
        if (handle)
            UpdateHandleRectPosition(Vector2.zero);
    }

    private float CalculateJoystickRange()
    {
        var plateRadius = plate.sizeDelta.x / 2f;
        var handleRadius = handle.sizeDelta.x / 2f;
        return plateRadius - handleRadius - handleOffset;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
#if LOGGER_ON
        Debug.Log("OnPointerDown");
#endif
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(plate, eventData.position, eventData.pressEventCamera, out Vector2 position);
        position = ApplySizeDelta(position);
        var clampedPosition = ClampValuesToMagnitude(position);
        var outputPosition = ApplyInversionFilter(position);
        OutputPointerEventValue(outputPosition * magnitudeMultiplier);

        if (handle)
            UpdateHandleRectPosition(clampedPosition * _joystickRange);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
#if LOGGER_ON
        Debug.Log("OnPointerUp");
#endif
        OutputPointerEventValue(Vector2.zero);

        if (handle)
            UpdateHandleRectPosition(Vector2.zero);
    }

    private void OutputPointerEventValue(Vector2 pointerPosition)
    {
        onJoystickOutput?.Invoke(pointerPosition);
        OnJoysticOutput?.Invoke(pointerPosition);

        // if (useActionPath)
        //     SendValueToControl(pointerPosition);
    }

    private void UpdateHandleRectPosition(Vector2 newPosition)
    {
        handle.anchoredPosition = newPosition;
    }

    private Vector2 ApplySizeDelta(Vector2 position)
    {
        var x = (position.x / plate.sizeDelta.x) * 2.5f;
        var y = (position.y / plate.sizeDelta.y) * 2.5f;
        return new Vector2(x, y);
    }

    private Vector2 ClampValuesToMagnitude(Vector2 position)
    {
        return Vector2.ClampMagnitude(position, 1);
    }

    private Vector2 ApplyInversionFilter(Vector2 position)
    {
        if (invertXOutputValue)
            position.x = InvertValue(position.x);

        if (invertYOutputValue)
            position.y = InvertValue(position.y);

        return position;
    }

    private float InvertValue(float value)
    {
        return -value;
    }

    // protected override string controlPathInternal
    // {
    //     get => actionPath;
    //     set => actionPath = value;
    // }

    #region Odin inspector
    private bool IsSetHandleRangeVisible()
    {
        return !setHandleRange;
    }
    #endregion

}

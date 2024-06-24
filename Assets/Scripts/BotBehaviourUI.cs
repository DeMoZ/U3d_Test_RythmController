using TMPro;
using UnityEngine;

public class BotBehaviourUI : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text statusText;
    private Transform _transform;
    private Transform _camTransform;

    public void Init(Camera camera)
    {
        canvas.worldCamera = camera;
        _transform = transform;
        _camTransform = camera.transform;
    }

    public void SetStatus(BotStates state)
    {
        statusText.text = state.ToString();
    }

    private void Update()
    {
        if(_transform == null || _camTransform == null)
            return;

        _transform.LookAt(_transform.position + _camTransform.rotation * Vector3.forward, _camTransform.rotation * Vector3.up);
    }
}
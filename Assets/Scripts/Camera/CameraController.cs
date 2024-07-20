using System;
using Cinemachine;
using DMZ.Events;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [field: SerializeField] public Camera MainCamera { get; private set; }
    [field: SerializeField] public CinemachineVirtualCamera VirtualCamera { get; private set; }
    [field: SerializeField] public Transform TrackedDolly { get; private set; }

    private Transform _target;
    private Transform _player;
    private DMZState<Transform> _onSetTarget;

    public void Init(Transform player, DMZState<Transform> onSetTarget)
    {
        _player = player;
        _onSetTarget = onSetTarget;
        _onSetTarget.Subscribe(OnSetTarget);

        VirtualCamera.Follow = _player;
    }

    private void OnDestroy()
    {
        _target = null;
        _onSetTarget.Unsubscribe(OnSetTarget);
    }

    public void OnSetTarget(Transform target)
    {
        _target = target;
    }

    void Update()
    {
        if (_target == null)
            TrackedDolly.position = _player.position;
        else
            TrackedDolly.position = _target.position;
    }
}

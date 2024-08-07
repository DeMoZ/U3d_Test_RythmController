using System.Collections.Generic;
using Cinemachine;
using DMZ.Events;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform trackedDolly;
    [SerializeField] private CinemachineVirtualCamera[] virtualCameras;

    private Transform _target;
    private Transform _player;
    private DMZState<Transform> _onSetTarget;
    private Queue<CinemachineVirtualCamera> _cameraQueue;
    private CinemachineVirtualCamera _curCamera;
    private CinemachineBrain _cinemachineBrain;

    public Camera MainCamera => mainCamera;

    public void Init(Transform player, DMZState<Transform> onSetTarget)
    {
        _cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
        _cameraQueue = new Queue<CinemachineVirtualCamera>(virtualCameras);

        _player = player;
        _onSetTarget = onSetTarget;
        _onSetTarget.Subscribe(OnSetTarget);
    }

    private void OnDestroy()
    {
        _target = null;
        _onSetTarget.Unsubscribe(OnSetTarget);
    }

    public void OnSetTarget(Transform target)
    {
        if (_curCamera != null)
        {
            _curCamera.LookAt = null;
            _curCamera.gameObject.SetActive(false);
            _cameraQueue.Enqueue(_curCamera);
        }

        _curCamera = _cameraQueue.Dequeue();
        _curCamera.LookAt = _player;
        _curCamera.Follow = target;
        _curCamera.gameObject.SetActive(true);

        _target = target;
    }

    void Update()
    {
        if (_target == null)
            trackedDolly.position = _player.position;
        else
            trackedDolly.position = _target.position;
    }
}

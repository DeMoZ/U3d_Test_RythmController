using System.Collections.Generic;
using Cinemachine;
using DMZ.Events;
using UnityEngine;

/// todo roman [Time.deltaTime] implement global deltatime
public class CameraController : MonoBehaviour
{
    [Tooltip("Offset for target position and camera FOV")]
    [SerializeField] private float targetOffset = 0.2f;

    [Tooltip("Virtual cameras body transponser follow offset")]
    [SerializeField] private Vector3 defaultFollowOffset = new(0, 4, -5);

    [Tooltip("Virtual cameras body transponser follow offset changing when target is in range of FOV")]
    [SerializeField] private float cameraFovSoftLerpSpeed = 0.2f;

    [Tooltip("Virtual cameras body transponser follow offset changing when target is NOT in range of FOV")]
    [SerializeField] private float cameraFovHardLerpSpeed = 2.5f;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera[] virtualCameras;

    private Transform _player;
    private DMZState<ITargetable> _onSetTarget;
    private CinemachineTransposer _curCameraTransposer;
    private ITargetable _target;
    private Queue<CinemachineVirtualCamera> _cameraQueue;
    private float _defaultMultiplier;
    private Vector3 _defaultFollowOffsetNormalized;
    private CinemachineVirtualCamera _curCamera;
    private Vector3 _curCameraTransposerOffset;

    public Camera MainCamera => mainCamera;

    // private void Awake(){
    //     OnSetTarget(null);
    // }

    public void Init(Transform player, DMZState<ITargetable> onSetTarget)
    {
        _cameraQueue = new Queue<CinemachineVirtualCamera>(virtualCameras);

        _defaultMultiplier = defaultFollowOffset.magnitude;
        _defaultFollowOffsetNormalized = defaultFollowOffset.normalized;

        _player = player;
        _onSetTarget = onSetTarget;
        OnSetTarget(null);
        _onSetTarget.Subscribe(OnSetTarget);
    }

    private void OnDestroy()
    {
        _onSetTarget.Unsubscribe(OnSetTarget);
    }

    public void OnSetTarget(ITargetable target)
    {
        if (_curCamera != null)
        {
            _curCamera.LookAt = null;
            _curCamera.gameObject.SetActive(false);
            _cameraQueue.Enqueue(_curCamera);
        }

        _curCamera = _cameraQueue.Dequeue();
        _curCamera.LookAt = _player;
        _curCamera.Follow = target == null ? _player : target.Transform;
        _curCamera.gameObject.SetActive(true);
        _curCameraTransposer = _curCamera.GetCinemachineComponent<CinemachineTransposer>();

        _target = target;
    }

    private void Update()
    {
        if (_target?.Transform == null)
        {
            if (_curCamera != null)
            {
                SetCameraTransposerBodyFollowOffset(_defaultFollowOffsetNormalized * _defaultMultiplier, cameraFovSoftLerpSpeed);
            }

            return;
        }

        bool isFullyVisible = IsVisible(out var distance);
        var _newCameraTransposerOffset = _defaultFollowOffsetNormalized * (_defaultMultiplier + distance * 100);

        if (isFullyVisible)
        {
            if (_newCameraTransposerOffset.sqrMagnitude < _curCameraTransposer.m_FollowOffset.sqrMagnitude)
            {
                SetCameraTransposerBodyFollowOffset(_defaultFollowOffsetNormalized * _defaultMultiplier, cameraFovSoftLerpSpeed);
            }
            else
            {
                SetCameraTransposerBodyFollowOffset(_newCameraTransposerOffset, cameraFovSoftLerpSpeed);
            }
        }
        else
        {
            SetCameraTransposerBodyFollowOffset(_newCameraTransposerOffset, cameraFovHardLerpSpeed);
        }
    }

    private void SetCameraTransposerBodyFollowOffset(Vector3 newFollowOffset, float lerpSpeed)
    {
        _curCameraTransposerOffset = _curCameraTransposer.m_FollowOffset;
        var lerpedOffset = Vector3.Lerp(_curCameraTransposerOffset, newFollowOffset, Time.deltaTime * lerpSpeed);
        _curCameraTransposer.m_FollowOffset = lerpedOffset;
    }

    private bool IsVisible(out float distance)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(_target.Transform.position);

        // Check if the point is within the horizontal bounds
        if (viewportPoint.x < targetOffset)
        {
            distance = targetOffset - viewportPoint.x;
            return false;
        }
        if (viewportPoint.x > 1 - targetOffset)
        {
            distance = viewportPoint.x - (1 - targetOffset);
            return false;
        }

        // Check if the point is within the vertical bounds
        if (viewportPoint.y < targetOffset)
        {
            distance = targetOffset - viewportPoint.y;
            return false;
        }
        if (viewportPoint.y > 1 - targetOffset)
        {
            distance = viewportPoint.y - (1 - targetOffset);
            return false;
        }

        // Check if the point is in front of the camera
        if (viewportPoint.z < 0)
        {
            distance = -viewportPoint.z;
            return false;
        }

        distance = 0;
        return true;
    }

    /*
        private bool IsFullyVisible()
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

            Vector3[] corners = GetBoundingBoxCorners(_target.Renderer.bounds);
            foreach (Vector3 corner in corners)
            {
                if (!IsPointInFrustum(planes, corner))
                {
                    return true;
                }
            }

            return false;
        }

        private Vector3[] GetBoundingBoxCorners(Bounds bounds)
        {
            Vector3[] corners = new Vector3[8];
            Vector3 center = bounds.center;
            Vector3 extents = bounds.extents;

            corners[0] = center + new Vector3(-extents.x, -extents.y, -extents.z);
            corners[1] = center + new Vector3(extents.x, -extents.y, -extents.z);
            corners[2] = center + new Vector3(extents.x, -extents.y, extents.z);
            corners[3] = center + new Vector3(-extents.x, -extents.y, extents.z);
            corners[4] = center + new Vector3(-extents.x, extents.y, -extents.z);
            corners[5] = center + new Vector3(extents.x, extents.y, -extents.z);
            corners[6] = center + new Vector3(extents.x, extents.y, extents.z);
            corners[7] = center + new Vector3(-extents.x, extents.y, extents.z);

            return corners;
        }

        private bool IsPointInFrustum(Plane[] planes, Vector3 point)
        {
            foreach (Plane plane in planes)
            {
                if (plane.GetDistanceToPoint(point) < 0)
                {
                    return false;
                }
            }

            return true;
        }
        */
}
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor.Rendering;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    [SerializeField] private CinemachineVirtualCamera[] allVirtualCameras;

    [Header("Tracked Object Offest Smooth Flip")]
    public float flipDuration;
    private Coroutine currentFlip;
    private float originalOffsetX = 0f;
    private int flipDirection = 1;

    [Header("Y Lerping during jump/fall")]
    [SerializeField] private float fallPanAmount = 0.25f;
    [SerializeField] private float fallYPanTime = 0.35f;
    public float fallSpeedYDampingChangeTreshold = -15f;
    public bool IsLerspingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    float _normYPanAmount;

    bool _cameraPanning = false;
    private Coroutine _lerpYPanCoroutine;
    private Coroutine _panCameraCoroutine;

    private Vector2 _startingTrackedObjectOffset;
    private PlayerData player;

    public CinemachineVirtualCamera respawnCamera;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        for (int i = 0; i < allVirtualCameras.Length; i++)
        {
            if (allVirtualCameras[i].enabled)
            {
                // set the current active camera
                _currentCamera = allVirtualCameras[i];

                //set the framingtrasposer
                _framingTrasposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }
        _normYPanAmount = _framingTrasposer.m_YDamping;
        _startingTrackedObjectOffset = _framingTrasposer.m_TrackedObjectOffset;
    }
    
    public CinemachineVirtualCamera _currentCamera;
    private CinemachineFramingTransposer _framingTrasposer;
    //private CinemachineConfiner2D _confiner;

    void Start()
    {
       //_confiner = _currentCamera.GetComponent<CinemachineConfiner2D>();
        originalOffsetX = _framingTrasposer.m_TrackedObjectOffset.x;
        flipDirection = _framingTrasposer.m_TrackedObjectOffset.x >= 0 ? 1 : -1;
        player = GameManager.Instance.player.GetComponent<PlayerData>();
    }

    void Update()
    {

        if (player.velocity.y < fallSpeedYDampingChangeTreshold && !IsLerspingYDamping && !LerpedFromPlayerFalling)
        {
            LerpYDamping(true);
        }

        if (player.velocity.y >= -0.1f && !IsLerspingYDamping && LerpedFromPlayerFalling)
        {
            LerpedFromPlayerFalling = false;

            LerpYDamping(false);
        }

    }

    #region offeset Flip
    public void Flip()
    {
        flipDirection *= -1;
        if (!_cameraPanning)
        {
            if (currentFlip != null)
            {
                StopCoroutine(currentFlip);
            }
            currentFlip = StartCoroutine(SmoothFlipTo(originalOffsetX * flipDirection));
        }
    }
    private IEnumerator SmoothFlipTo(float targetX)
    {
        Vector3 offset = _framingTrasposer.m_TrackedObjectOffset;
        float startX = offset.x;

        float elapsed = 0f;
        while (elapsed < flipDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flipDuration;
            offset.x = Mathf.Lerp(startX, targetX, t);
            _framingTrasposer.m_TrackedObjectOffset = offset;
            yield return null;
        }

        offset.x = targetX;
        _framingTrasposer.m_TrackedObjectOffset = offset;
        currentFlip = null;
    }
    #endregion

    #region Y Pan during jump/fall
    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    private IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerspingYDamping = true;

        //grab the starting damping amount
        float startDampAmount = _framingTrasposer.m_YDamping;
        float endDampAmount = 0f;

        //determine the end damping amount
        if (isPlayerFalling)
        {
            endDampAmount = fallPanAmount;
            LerpedFromPlayerFalling = true;
        }

        else
        {
            endDampAmount = _normYPanAmount;
        }

        //lerp the pan amount
        float elapsedTime = 0f;
        while (elapsedTime < fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedPanAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime / fallPanAmount));
            _framingTrasposer.m_YDamping = lerpedPanAmount;

            yield return null;
        }

        IsLerspingYDamping = false;
    }
    #endregion

    #region Camera Pan

    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        //handle pan from trigger
        if (!panToStartingPos)
        {
            _cameraPanning = true;
            //set the direction and the distance
            switch (panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.left;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.right;
                    break;
            }

            endPos *= panDistance;
            startingPos = _startingTrackedObjectOffset;
            endPos += startingPos;
        }

        //handle the pan to starting point
        else
        {
            startingPos = _framingTrasposer.m_TrackedObjectOffset;
            endPos = _startingTrackedObjectOffset;
        }

        //handle the actual panning of the camera
        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector2 panLerp = Vector2.Lerp(startingPos, endPos, (elapsedTime / panTime));
            _framingTrasposer.m_TrackedObjectOffset = panLerp;

            yield return null;
        }

        if (panToStartingPos)
        {
            _cameraPanning = false;
        }
    }

    #endregion

    #region Swap Camera

    public void Respawn()
    {
        ChangeCamera(respawnCamera);
    }

    public void ChangeCamera(CinemachineVirtualCamera newCamera)
    {
        //if the new camera is already the current camera, do nothing
        if (_currentCamera == newCamera) return;

        //deactivate the current camera
        _currentCamera.enabled = false;
        //activate the new camera
        newCamera.enabled = true;
        //set the new camera as the current camera
        _currentCamera = newCamera;
        //update our composer variable
        _framingTrasposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _startingTrackedObjectOffset = _framingTrasposer.m_TrackedObjectOffset;
        _normYPanAmount = _framingTrasposer.m_YDamping;
    }
    public void swapCamerasLeftRight(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        //if the current camera is the camera on the left and ouur trigger exit direction was on the right
        if (_currentCamera == cameraFromLeft && triggerExitDirection.x > 0f)
        {
            //activate the new camera
            cameraFromRight.enabled = true;
            //deactivate the old camera
            cameraFromLeft.enabled = false;
            //set the ne camera as the current camera
            _currentCamera = cameraFromRight;
            //update out composer variable
            _framingTrasposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            _startingTrackedObjectOffset = _framingTrasposer.m_TrackedObjectOffset;
            _normYPanAmount = _framingTrasposer.m_YDamping;
        }

        //if the current camera is the camera on the right and out trigger hit direction was on the left
        else if (_currentCamera == cameraFromRight && triggerExitDirection.x < 0f)
        {
            //activate the new camera
            cameraFromLeft.enabled = true;
            //deactivate the old camera
            cameraFromRight.enabled = false;
            //set the new camera as the current camera
            _currentCamera = cameraFromLeft;
            //update our composer variable
            _framingTrasposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            _startingTrackedObjectOffset = _framingTrasposer.m_TrackedObjectOffset;
            _normYPanAmount = _framingTrasposer.m_YDamping;
        }
    }

    public void swapCamerasUpDown(CinemachineVirtualCamera cameraOnTop, CinemachineVirtualCamera cameraOnBottom, Vector2 triggerExitDirection)
    {
        //if the current camera is the camera on the top and our trigger exit direction was on the bottom
        if (_currentCamera == cameraOnTop && triggerExitDirection.y < 0f)
        {
            //activate the new camera
            cameraOnBottom.enabled = true;
            //deactivate the old camera
            cameraOnTop.enabled = false;
            //set the new camera as the current camera
            _currentCamera = cameraOnBottom;
            //update our composer variable
            _framingTrasposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            _startingTrackedObjectOffset = _framingTrasposer.m_TrackedObjectOffset;
            _normYPanAmount = _framingTrasposer.m_YDamping;
        }

        //if the current camera is the camera on the bottom and our trigger hit direction was on the top
        else if (_currentCamera == cameraOnBottom && triggerExitDirection.y > 0f)
        {
            //activate the new camera
            cameraOnTop.enabled = true;
            //deactivate the old camera
            cameraOnBottom.enabled = false;
            //set the new camera as the current camera
            _currentCamera = cameraOnTop;
            //update our composer variable
            _framingTrasposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            _startingTrackedObjectOffset = _framingTrasposer.m_TrackedObjectOffset;
            _normYPanAmount = _framingTrasposer.m_YDamping;
        }
    }
    #endregion

    public void ChangeCameraBoundaries(CompositeCollider2D cameraBounds)
    {
        //_confiner.m_BoundingShape2D = cameraBounds;
        //_confiner.InvalidateCache();
    }
}

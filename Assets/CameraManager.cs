using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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

    [SerializeField] float normYPanAmount = 2f;
    private Coroutine _lerpYPanCoroutine;
    private PlayerData player;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public  CinemachineVirtualCamera _currentCamera;
    private CinemachineFramingTransposer _framingTrasposer;

    void Start()
    {
        _framingTrasposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        originalOffsetX = Mathf.Abs(_framingTrasposer.m_TrackedObjectOffset.x);
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
        if (currentFlip != null)
        {
            StopCoroutine(currentFlip);
        }
        currentFlip = StartCoroutine(SmoothFlipTo(originalOffsetX * flipDirection));
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
            endDampAmount = normYPanAmount;
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
}

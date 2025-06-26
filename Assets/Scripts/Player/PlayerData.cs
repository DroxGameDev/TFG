using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.U2D.Animation;
public enum GravityMode{
    Up,
    Down,
    Left,
    Right,
    Cancel
}
public class PlayerData : MonoBehaviour
{
    public Camera mainCamera;
    [Range(0f, 100f)] public float defaultCameraSize;
    [Space(10)]
    
    [Header("Health")]
    [Range(0, 10)] public int health;
    [Range(0, 50)] public int maxHealth;

    [Header("Movement")]
    [Range(0f, 20f)] public float moveSpeed;
    [Range(0f, 30f)] public float acceleration;
    [Range(0f, 30f)] public float decceleration;
    [Range(0f, 1.5f)] public float velPower;
    public float moveMod = 1;

    [Space(10)]
    [Range(0f, 1f)] public float crocuhModifier;

    [Space(10)]
    [Range(0f, 0.5f)] public float frictionAmount;
    public Vector2 velocity = new Vector2(0f, 0f);
    [Space(10)]
    public bool isFacingRight = true;
    [Space(10)]
    public Door nearbyDoor;

    [Header("Jumping")]
    [Range(0f, 20f)] public float jumpForce;
    [Range(0f, 1f)] public float jumpCutMultiplier;
    [Space(10)]
    [Range(0f, 1f)] public float coyoteTime;
    [Range(0f, 1f)] public float jumpBufferTime;
    [HideInInspector] public float jumpMod = 1;

    [Space(10)]
    [Header("Gravity")]
    [Range(0f, 5f)] public float gravityScale;
    [Range(0f, 5f)] public float fallGravityMultiplier;
    public GravityMode gravityMode = GravityMode.Down;
    [Range(0f, 5f)] public float jumpHangTheshold;
    [Range(0f, 5f)] public float jumpHangMultiplier;

    [Space(10)]
    [Header("Attacking")]
    [Range(0, 5)] public int damage;
    [Range(0, 10f)] public float damageKnockback;
    [Space(10)]
    [Range(0, 1f)] public float prepareAttackTime;
    [Range(0f, 1f)] public float attackTime;
    [Range(0f, 1f)] public float attackCooldownOvertime;
    [Range(0f, 1f)] public float attackBufferTime;
    [Range(0f, 5f)] public float attackComboTime;
    [Space(10)]
    public AttackCombo attackComboStep = AttackCombo.Attack1;
    [Space(10)]
    [Range(0, 10f)] public float attackImpulse;
    [Space(10)]
    public GameObject attackOrigin;

    [Header("Damage")]
    [Range(0f, 1f)]public float damageWait;
    [Range(0, 1f)] public float hitTime;
    [Range(0, 5f)] public float invicibilityTime;
    public SpriteRenderer sprite;
    [Range(0, 10f)] public float invicibilityEffectSpeed;
    [Space(10)]

    [Header("Die")]
    [Range(0, 25f)] public float deathKnockBack;
    public Collider2D damageCollider;

    [Space(10)]
    [Header("Check")]

    [Range(0f,1f)] public float groundCheckDistance;

    public LayerMask groundLayer;
    [Space(10)]
    public LayerMask obstacleLayer;
    public LayerMask obstacleLayerMinusOpenWall;

    [Header("Metal Lines")]
    //Steel = push
    public Transform linesOrigin;
    public LayerMask metalLayers;
    [Range(0f, 10f)] public float metalCheckRadius;
    [Range(0f, 5f)] public float metalCheckMinRadius;
    public GameObject linePrefab;
    [Range(0f, 5f)] public float selectMetalTime;

    [Space(10)]
    public Material selectMetalMaterial;
    [Range(0f, 0.1f)] public float selectMetalTransitionStep;
    [Space(10)]
    [Range(0f, 2f)] public float returnArrowSpeedMod;

    [Header("Steel")]
    public bool burningSteel;
    [Range(0f, 40f)] public float steelPushPower;
    [Range(0f, 5f)] public float steelPushTime;

    [Space(10)]

    [Header("Iron")]
    public bool burningIron;
    [Range(0f, 100f)] public float ironPullPower;
    [Range(0f, 5f)] public float ironPullTime;
    public LayerMask walkableAreaLayer;

    [Header("Shoot Coins")]
    public GameObject showedCoin;
    public Transform shootPoint;
    [Range(0f, 5f)] public float showCoinTime;

    [Header("Tin")]
    public bool burningTin;
    [Range(0f, 0.1f)] public float tinTransitionStep;
    [Space(10)]
    [Range(0f, 10f)] public float seeThroughMistSize;
    public Material mist;
    public Material hiddenWall;
    [Space(10)]
    [Range(0f, 100f)] public float tinCameraSize;
    public CinemachineVirtualCamera virtualCamera;

    [Header("Pewter")]
    public bool burningPewter;
    [Range(0, 5)] public int pewterDamage;
    [Range(0, 10f)] public float pewterDamageKnokback;
    [Range(1f, 3f)] public float pewterMovementModifier;
    [Range(1f, 3f)] public float pewterJumpModifier;
    [Space(10)]
    public Material smearFramesMaterial;
    [Space(10)]
    public bool boxNearby;
    public GameObject boxToPush;
    public Collider2D pushCollider;
    [Range(0f, 1f)] public float pewterPushMovementModifier;


    [Header("Sprite Libraries")]
    public SpriteLibrary playerSpriteLibrary;
    public SpriteLibraryAsset defaultSprites;
    public SpriteLibraryAsset showCoinSprites;
    [Space(10)]

    public Animator anim;

    [Header("States")]
    public bool grounded = true;
    public bool running = false;
    public bool jumping = false;
    public bool falling = false;
    public bool preparingAttack = false;
    public bool attacking = false;
    public bool wallWalking = false;
    public bool pushing = false;
    public bool damaged = false;
    public bool dead = false;

    [Space(10)]
    public bool movingWithPowers = false;
    public bool showingCoin = false;

    void Awake()
    {
        moveMod = 1;

        isFacingRight = true;

        gravityScale = 1f;
        gravityMode = GravityMode.Down;

        damage = 1;
        attackComboStep = AttackCombo.Attack1;
        attackOrigin.GetComponent<PlayerAttackInfo>().isFacingRight = true;

        burningIron = false;
        burningSteel = false;
        burningTin = false;
        burningPewter = false;

        showedCoin = null;
        shootPoint.position = new Vector3(shootPoint.position.x, linesOrigin.position.y, 0);

        boxNearby = false;
        boxToPush = null;

        grounded = false;
        running = false;
        jumping = false;
        falling = false;
        preparingAttack = false;
        attacking = false;
        wallWalking = false;
        pushing = false;
        movingWithPowers = false;
        showingCoin = false;
        damaged = false;
        dead = false;
    }

    void OnEnable()
    {
        GameManager.Instance.RegisterPlayer(transform);   
    }

    public void ChangeGravityMode(GravityMode mode)
    {
        if (mode == GravityMode.Cancel)
        {
            gravityMode = GravityMode.Cancel;
            //transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            setGravity(0, 0f);
        }
        else if (mode == GravityMode.Left)
        {
            gravityMode = GravityMode.Left;
            wallWalking = true;
            //transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            setGravity(1f, 0f);
        }
        else if (mode == GravityMode.Right)
        {
            gravityMode = GravityMode.Right;
            wallWalking = true;
            //transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            setGravity(-1f, 0f);
        }
        else if (mode == GravityMode.Up)
        {
            gravityMode = GravityMode.Up;
            wallWalking = true;
            //transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            setGravity(0f, -1f);
        }
        else
        {
            gravityMode = GravityMode.Down;
            wallWalking = false;
            setGravity(0f, 1f);
            //transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
    public void setGravity(float newGracityScaleX, float newGracityScaleY)
    {
        ConstantForce2D forceMode = GetComponent<ConstantForce2D>();
        forceMode.force = new Vector2(Physics2D.gravity.x * newGracityScaleX, Physics2D.gravity.y * newGracityScaleY);
    }

    public void Flip()
    {
        if (Time.timeScale == 1f)
        {
            isFacingRight = !isFacingRight;

            attackOrigin.GetComponent<PlayerAttackInfo>().isFacingRight = !attackOrigin.GetComponent<PlayerAttackInfo>().isFacingRight;

            Vector2 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;

            /*
            Vector3 currentShootPoint = playerData.shootPoint.localPosition;
            currentShootPoint.x *= -1f;
            playerData.shootPoint.localPosition = currentShootPoint;
            */
        }
    }
}

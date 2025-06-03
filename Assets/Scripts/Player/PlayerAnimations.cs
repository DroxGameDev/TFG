using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerAnimations : MonoBehaviour
{
    private PlayerData playerData;
    [Range (0f, 15)] public int animationDebugIndex = 15;
    public string[] animationName;
    private static readonly int idle = Animator.StringToHash("Idle");
    private static readonly int run = Animator.StringToHash("Run");
    private static readonly int jump = Animator.StringToHash("Jump");
    private static readonly int fall = Animator.StringToHash("Fall");
    private static readonly int crouchIdle = Animator.StringToHash("Crouch_Idle");
    private static readonly int crouchRun = Animator.StringToHash("Crouch_Walk");
    private static readonly int attack1 = Animator.StringToHash("Attack1");
    private static readonly int attack2 = Animator.StringToHash("Attack2");
    private static readonly int attack3 = Animator.StringToHash("Attack3");
    private static readonly int punch1 = Animator.StringToHash("Punch1");
    private static readonly int punch2 = Animator.StringToHash("Punch2");
    private static readonly int punch3 = Animator.StringToHash("Punch3");

    private static readonly int pushIdle = Animator.StringToHash("PushIdle");
    private static readonly int push = Animator.StringToHash("Push");
    private static readonly int pull = Animator.StringToHash("Pull");



    private SpriteLibraryAsset currentSpriteLibrary;

    [Header("States")]
    private float _lockedTill;
    private int currentState = 0;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        currentSpriteLibrary = playerData.defaultSprites;
    }

    void Update()
    {

        if (animationDebugIndex < animationName.Length)
            playerData.anim.CrossFade(animationName[animationDebugIndex], 0, 0);
        else{

            var state = GetState();

            if (playerData.showingCoin && currentSpriteLibrary != playerData.showCoinSprites)
            {
                playerData.playerSpriteLibrary.spriteLibraryAsset = playerData.showCoinSprites;
                currentSpriteLibrary = playerData.showCoinSprites;
            }
            else if (!playerData.showingCoin && currentSpriteLibrary != playerData.defaultSprites)
            {
                playerData.playerSpriteLibrary.spriteLibraryAsset = playerData.defaultSprites;
                currentSpriteLibrary = playerData.defaultSprites;
            }

            if (state == currentState) return;
            playerData.anim.CrossFade(state, 0, 0);
            currentState = state;

        }
        
    }

    private int GetState(){
        if(Time.time < _lockedTill) return currentState;

        if (playerData.attacking)
        {
            switch (playerData.attackComboStep)
            {
                case AttackCombo.Attack2:
                    return LockState(playerData.burningPewter ? punch2: attack2, playerData.attackCooldownTime);
                case AttackCombo.Attack3:
                    return LockState(playerData.burningPewter ? punch3: attack3, playerData.attackCooldownTime);
                default:
                    return LockState(playerData.burningPewter ? punch1: attack1, playerData.attackCooldownTime);
            }
        }

        if (playerData.wallWalking)
        {
            if (playerData.running && Mathf.Abs(playerData.velocity.x) > 0.01f)
            {
                return crouchRun;
            }
            else
            {
                return crouchIdle;
            } 
        }

        if (playerData.jumping) return jump;
        
        if (playerData.grounded)
        {
            if (playerData.running && playerData.pushing)
            {
                if ((playerData.isFacingRight && playerData.velocity.x > 0.01f) || (!playerData.isFacingRight && playerData.velocity.x < 0.01f))
                {
                    return push;
                }
                else if ((playerData.isFacingRight && playerData.velocity.x < 0.01f) || (!playerData.isFacingRight && playerData.velocity.x > 0.01f))
                {
                    return pull;
                }
            }
            else if (playerData.pushing) return pushIdle;

            if (playerData.running && Mathf.Abs(playerData.velocity.x) > 0.01f)
            {
                return run;
            }
            else
            {
                return idle;
            }
        }

        return playerData.velocity.y > 0 ? jump : fall;

        int LockState(int s, float t){
            _lockedTill = Time.deltaTime+t;
            return s;
        }
    }
}

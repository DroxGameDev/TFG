using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class PlayerAnimations : MonoBehaviour
{

    private Animator anim;
    private Rigidbody2D rb; 
    private PlayerData playerData;
    [Range (0f, 15)] public int animationDebugIndex = 15;
    public string[] animationName;

    private static readonly int idle = Animator.StringToHash("Idle");
    private static readonly int run = Animator.StringToHash("Run");
    private static readonly int jump = Animator.StringToHash("Jump");
    private static readonly int fall = Animator.StringToHash("Fall");
    private static readonly int crouchIdle = Animator.StringToHash("Crouch_Idle");
    private static readonly int crouchRun = Animator.StringToHash("Crouch_Walk");

    private SpriteLibrary playerSpriteLibrary;
    private SpriteLibraryAsset currentSpriteLibrary;

    [Header("States")]
    private float _lockedTill;
    private int currentState = 0;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerData = GetComponent<PlayerData>();
        playerSpriteLibrary = GetComponent<SpriteLibrary>();
        currentSpriteLibrary = playerData.defaultSprites;
    }

    void Update()
    {

        if (animationDebugIndex < animationName.Length)
            anim.CrossFade(animationName[animationDebugIndex], 0, 0);
        else{

            var state = GetState();

            if (playerData.showingCoin && currentSpriteLibrary != playerData.showCoinSprites)
            {
                playerSpriteLibrary.spriteLibraryAsset = playerData.showCoinSprites;
                currentSpriteLibrary = playerData.showCoinSprites;
            }
            else if (!playerData.showingCoin && currentSpriteLibrary != playerData.defaultSprites)
            {
                playerSpriteLibrary.spriteLibraryAsset = playerData.defaultSprites;
                currentSpriteLibrary = playerData.defaultSprites;
            }

            if (state == currentState) return;
            anim.CrossFade(state, 0, 0);
            currentState = state;

        }
        
    }

    private int GetState(){
        if(Time.time < _lockedTill) return currentState;

        if (playerData.jumping) return jump;
        
        if (playerData.grounded)
        {
            if (playerData.running && Mathf.Abs(playerData.velocity.x) > 0.01f)
            {

                if (playerData.wallWalking)
                    return crouchRun;
                else
                    return run;
            }
            else
            {
                if (playerData.wallWalking)
                    return crouchIdle;
                else
                    return idle;
            }
        }

        return playerData.velocity.y > 0 ? jump : fall;

        int lockState(int s, float t){
            _lockedTill = Time.time+t;
            return s;
        }
    }
}

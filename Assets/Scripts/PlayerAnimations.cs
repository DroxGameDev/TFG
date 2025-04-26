using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

    private Animator _anim;
    [Range (0f, 5)] public int animationDebugIndex = 5;
    public string[] animationName;

    private static readonly int idle = Animator.StringToHash("Idle");
    private static readonly int run = Animator.StringToHash("Run");
    private static readonly int jump = Animator.StringToHash("Jump");
    private static readonly int fall = Animator.StringToHash("Fall");

    [Header("States")]
    public bool _grounded = true;
    public bool _running = false;
    public bool _jumping = false;
    public bool _falling = false;
    private float _lockedTill;
    private int _currentState = 0;

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    void Update()
    {

        if (animationDebugIndex < animationName.Length)
            _anim.CrossFade(animationName[animationDebugIndex], 0, 0);
        else{

            var state = GetState();

            if(state == _currentState) return;
            _anim.CrossFade(state, 0, 0);
            _currentState = state;

        }
        
    }

    private int GetState(){
        if(Time.time < _lockedTill) return _currentState;

        if (_jumping) return jump;
        if (_falling) return fall;
        if (_grounded){
            if(_running) return run;

            else return idle;
        }

        return idle;

        int lockState(int s, float t){
            _lockedTill = Time.time+t;
            return s;
        }
    }
}

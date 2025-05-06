using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

    private Animator _anim;
    private Rigidbody2D _rb;
    private PlayerData _playerData;
    [Range (0f, 6)] public int animationDebugIndex = 5;
    public string[] animationName;

    private static readonly int idle = Animator.StringToHash("Idle");
    private static readonly int run = Animator.StringToHash("Run");
    private static readonly int jump = Animator.StringToHash("Jump");
    private static readonly int fall = Animator.StringToHash("Fall");

    [Header("States")]
    private float _lockedTill;
    private int _currentState = 0;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _playerData = GetComponent<PlayerData>();
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

        if (_playerData.jumping) return jump;
        if (_playerData.grounded){
            if(_playerData.running && Mathf.Abs(_rb.velocity.x) > 0.01f) return run;

            else return idle;
        }

        return _rb.velocity.y > 0 ? jump : fall;

        int lockState(int s, float t){
            _lockedTill = Time.time+t;
            return s;
        }
    }
}

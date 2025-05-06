using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [Header("Movement")]
    [Range (0f, 20f)] public  float moveSpeed;
    [Range (0f, 30f)] public  float acceleration;
    [Range (0f, 30f)] public  float decceleration;
    [Range (0f, 1.5f)] public  float velPower;

    [Space(10)] 
    [Range (0f, 0.5f)] public  float frictionAmount;

    [Header("Jumping")]
    [Range (0f, 20f)] public  float jumpForce;
    [Range (0f, 1f)] public  float jumpCutMultiplier;
    [Space(10)] 
    [Range (0f, 1f)] public  float coyoteTime;
    [Range (0f, 1f)] public  float jumpBufferTime;

    [Space(10)] 
    [Range (0f, 5f)] public  float gravityScale;
    [Range (0f, 5f)] public  float fallGravityMultiplier;
    
    [Range (0f, 5f)] public float jumpHangTheshold;
    [Range (0f, 5f)] public float jumpHangMultiplier;

    [Space(10)]

    [Header("Check")]
    public Transform groundCheck;
    [Range (0f, 0.5f)] public  float groundCheckRadius;

    [Space(10)]
    public LayerMask groundLayer;

    [Header("Metal Lines")]
    //Steel = push
    public LayerMask metalLayers;
    [Range (0f, 10f)] public float metalCheckRadius;
    [Range (0f, 5f)] public float metalCheckMinRadius;
    public GameObject linePrefab;
    [Range(0f,5f)] public float selectMetalTime;
    
    [Space(10)] 

    [Header("Steel")]
    [Range(0f, 40f)] public float steelPushPower;
    [Range(0f, 5f)] public float steelPushTime;

    [Space(10)] 

    [Header("Iron")]
    [Range(0f, 40f)] public float ironPullPower;
    [Range(0f, 5f)] public float ironPullTime;

    [Header("States")]
    public bool grounded = true;
    public bool running = false;
    public bool jumping = false;
    public bool falling = false;
    public bool timeStoped = false;
    public bool movingWithPowers = false;

}

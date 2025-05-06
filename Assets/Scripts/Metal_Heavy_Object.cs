using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal_Heavy_Object : MonoBehaviour
{

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
    }


    void FixedUpdate()
    {
        if(rb.velocity == Vector2.zero && rb.constraints != RigidbodyConstraints2D.FreezePositionX){
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
    }

}

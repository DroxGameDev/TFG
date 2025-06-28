using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInfo : MonoBehaviour
{
    public string sceneName;
    public CompositeCollider2D cameraBounds;

    void Awake()
    {
        cameraBounds = GetComponent<CompositeCollider2D>();
    }
}   

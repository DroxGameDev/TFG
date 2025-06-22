using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakSet : MonoBehaviour
{
    public CloakAnchor[] cloakAnchors;

    [Range(10,50)] public float minLerpSpeed;
    [Range(10,50)]public float maxLerpSpeed;

    [Range(0f, 1f)]public float maxOffsetx;
    [Range(0f, 1f)]public float maxOffsety;

    [Range(0, 0.1f)] public float cloackGravity;

    public Vector2 idleOffset = Vector2.zero;

    void Awake()
    {
        
        cloakAnchors = GetComponentsInChildren<CloakAnchor>();

        foreach (CloakAnchor c in cloakAnchors)
        {
            float lerp = Random.Range(minLerpSpeed, maxLerpSpeed);
            c.lerpSpeed = lerp;

            c.maxOffsetx = maxOffsetx;
            c.maxOffsety = maxOffsety;

            c.gravityAdition = cloackGravity;

            c.idleOffset = idleOffset;

        }
    }
}

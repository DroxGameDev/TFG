using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloakAnchor : MonoBehaviour
{
    public PlayerData playerData;
    public Rigidbody2D playerRb;
    [Range(0f, 1f)]public float maxOffsetx;
    [Range(0f, 1f)]public float maxOffsety;
    
    [Range(0f, 0.1f)] public float gravityAdition;

    public Vector2 idleOffset = Vector2.zero;
    private Vector2 partOffset = Vector2.zero;
    [Range(0f, 100f)] public float lerpSpeed;

    private Transform[] cloakParts;
    private Transform cloakAnchor;

    private bool playerFacingRight = true;

    private void Awake()
    {
        cloakAnchor = GetComponent<Transform>();
        cloakParts = GetComponentsInChildren<Transform>();
    }

    private void Update()
    {
        Transform pieceToFollow = cloakAnchor;
    
        if (playerRb.velocity.magnitude <= 0.1f || playerData.gravityMode == GravityMode.Left || playerData.gravityMode == GravityMode.Right)
        {
            partOffset = idleOffset;
        }
        else
        {
            Vector2 direction = playerRb.velocity.normalized * -1;

            float xOffset = Mathf.Lerp(0f, maxOffsetx, Mathf.Abs(direction.x)) * Mathf.Sign(direction.x);
            float yOffset = Mathf.Lerp(0f, maxOffsety, Mathf.Abs(direction.y)) * Mathf.Sign(direction.y) - gravityAdition;

            partOffset = new Vector2(xOffset + idleOffset.x, yOffset);
        }

        DetectFlip();

        foreach (Transform cloakPart in cloakParts)
        {
            if (!cloakPart.Equals(cloakAnchor)){
                Vector2 targetPosition = (Vector2)pieceToFollow.position + partOffset;
                Vector2 newPositionLerped = Vector2.Lerp(cloakPart.position, targetPosition, Time.deltaTime * lerpSpeed);

                cloakPart.position = newPositionLerped;

                // Dirección hacia la parte anterior
                Vector2 directionToFollow = (Vector2)pieceToFollow.position - (Vector2)cloakPart.position;

                // Evitar errores si las posiciones son iguales
                if (directionToFollow.sqrMagnitude > 0.0001f)
                {
                    float angle = Mathf.Atan2(directionToFollow.y, directionToFollow.x) * Mathf.Rad2Deg;

                    // Suavizado de rotación
                    Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
                    cloakPart.rotation = Quaternion.Lerp(cloakPart.rotation, targetRotation, Time.deltaTime * lerpSpeed);


                }
                pieceToFollow = cloakPart;
            }
        }
    }

    private void DetectFlip()
    {
        if ((playerData.isFacingRight && !playerFacingRight) || (!playerData.isFacingRight && playerFacingRight))
        {
            cloakAnchor.localScale = new Vector3(cloakAnchor.localScale.x *-1, cloakAnchor.localScale.y, cloakAnchor.localScale.z);
            playerFacingRight = !playerFacingRight;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SteelPower2 : Iron_Steel2
{
    private float burningSteelCounter;
    private bool impulsePushedObject = false;
    public bool active = false;
    public IEnumerator SteelInputupdate(bool context)
    {
        if (context)
        {
            input = true;
        }

        else if (!context)
        {
            input = false;
        }

        yield return null;
    }
    void Update()
    {
        OnUpdate();

        //Debug.Log ("1:" + state);

        if (input && state == PowerState.inactive)
        {
            ChangeState(PowerState.select);
            OnSelect();
        }

        //Debug.Log ("2:" + state);

        else if ((!input || selectMetalCounter <= 0) && state == PowerState.select && playerData.burningSteel)
        {

            if (selectedMetal != null)
            {
                ChangeState(PowerState.impulse);
                OnImpulse();
            }
            else
            {
                ChangeState(PowerState.inactive);
                OnInactive();
            }
            input = false;
        }

        //Debug.Log ("3:" + state);

        else if (state == PowerState.impulse && playerData.burningSteel)
        {
            burningSteelCounter -= Time.deltaTime;

            if (burningSteelCounter <= 0.001f)
            {
                burningSteelCounter = 0f;
                ChangeState(PowerState.inactive);
                OnInactive();
            }
        }

        //Debug.Log ("4:" + state);

    }
    public override void OnInactive()
    {
        playerData.burningSteel = false;
        playerData.movingWithPowers = false;
        playerData.timeStoped = false;
        ResetLines();
    }

    public override void OnSelect()
    {
        if (GetNearbyMetals())
        {
            playerData.burningSteel = true;
            playerData.timeStoped = true;
            selectMetalCounter = playerData.selectMetalTime;
        }
        else
        {
            ChangeState(PowerState.inactive);
            OnInactive();
        }
    }

    public override void OnForce()
    {
    }

    public override void OnImpulse()
    {
        playerData.movingWithPowers = true;
        playerData.timeStoped = false;
        burningSteelCounter = playerData.steelPushTime;

        StartCoroutine(pushObject(rb, selectedMetal.metal.attachedRigidbody));

        ResetLines();
    }

    private Vector2 getImpulse(Vector2 metalPosition)
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);
        Vector2 directorVector = metalPosition - position + (selectedMetal.metal.offset * -1);
        directorVector.Normalize();
        rb.velocity = Vector3.zero;
        Vector2 forceToApply = directorVector * playerData.steelPushPower * selectedMetal.iValue * -1;

        return forceToApply;
    }

    private IEnumerator pushObject(Rigidbody2D origin, Rigidbody2D target)
    {
        Vector2 forceAmount;

        if (target.tag == "Floor" || target.tag == "Walkable_Area")
        {
            Vector2 MetalClosestPoint = target.GetComponent<BoxCollider2D>().ClosestPoint(origin.gameObject.transform.position);
            forceAmount = getImpulse(MetalClosestPoint);
            origin.AddForce(forceAmount, ForceMode2D.Impulse);
        }
        else
        {
            forceAmount = getImpulse(target.gameObject.transform.position);
            if (target.tag == "Environment_metal")
            {
                origin.AddForce(forceAmount, ForceMode2D.Impulse);
            }
            else if (target.tag == "Heavy_Metal")
            {
                origin.AddForce(forceAmount, ForceMode2D.Impulse);

                yield return new WaitForFixedUpdate();

                if (origin.velocity.x < 0.01f)
                {
                    StartCoroutine(target.GetComponent<Metal_Heavy_Object>().Impulse(forceAmount * -1));
                }
            }
            else if (target.tag == "Coin")
            {
                yield return StartCoroutine(moveAwayFromPlayer(target.GetComponent<Collider2D>(), origin.GetComponent<Collider2D>()));

                if (impulsePushedObject)
                {
                    target.AddForce(forceAmount * -1, ForceMode2D.Impulse);
                }
                else
                {
                    origin.AddForce(forceAmount, ForceMode2D.Impulse);
                }
            }
        }
    }
    private Vector2 getPointToForce(Vector2 metalPosition)
    {
        Vector2 playerPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 directorVector = metalPosition - playerPosition /*+ (selectedMetal.metal.offset * -1)*/;
        directorVector.Normalize();
        return  playerPosition + directorVector * playerData.metalCheckRadius;
    }

    private IEnumerator moveAwayFromPlayer(Collider2D metal, Collider2D player)
    {
        Vector2 objectivePosition = getPointToForce(metal.transform.position);
        bool metalObstacleReached = false;

        if (metal.gameObject.tag == "Heavy_Metal")
        {
            var heavyMetal = metal.GetComponent<Metal_Heavy_Object>();
            heavyMetal.ForceMove();
        }

        Vector2 currentPosition = metal.transform.position;

        while (Vector2.Distance(currentPosition, objectivePosition) > 0.01f && !metalObstacleReached)
        {
            currentPosition = metal.transform.position;
            Vector2 direction = (objectivePosition - currentPosition).normalized;

            float step = playerData.steelPushPower / metal.attachedRigidbody.mass * Time.fixedDeltaTime;

            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(playerData.obstacleLayer);
            filter.useLayerMask = true;

            RaycastHit2D[] hits = new RaycastHit2D[5]; ;
            int hitCount = metal.attachedRigidbody.Cast(direction, filter, hits, step);

            if (hitCount > 0)
            {
                // Si hay colisión, mueve solo hasta el punto de colisión
                metal.attachedRigidbody.MovePosition(currentPosition + direction * hits[0].distance);
                if (hits[0].distance < 0.1f)
                {
                    Debug.Log(direction.y);
                    if (Mathf.Abs(direction.y) > 0.7f)
                    {
                        metalObstacleReached = true;
                    }

                    else
                    {
                        //nueva dirección
                        if (player.transform.position.x > currentPosition.x)
                        {
                            direction = Vector2.left;
                        }
                        else
                        {
                            direction = Vector2.right;
                        }
                        //nueva posición
                        objectivePosition = currentPosition + direction * (playerData.metalCheckRadius - Vector2.Distance(currentPosition, player.transform.position));
                    }
                }
            }
            else
            {
                // Si no hay colisión, mueve normalmente
                Vector2 newPosition = Vector2.MoveTowards(currentPosition, objectivePosition, step);
                metal.attachedRigidbody.MovePosition(newPosition);
            }

            yield return new WaitForFixedUpdate();
        }

        if (metal.gameObject.tag == "Heavy_Metal")
        {
            var heavyMetal = metal.GetComponent<Metal_Heavy_Object>();
            heavyMetal.Stop();
            impulsePushedObject = false;
        }

        if (metal.gameObject.tag == "Coin" && !metalObstacleReached)
        {
            impulsePushedObject = true;
        }
        else if (metal.gameObject.tag == "Coin" && metalObstacleReached)
        {
            impulsePushedObject = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (active)
        {
            if (selectedMetal == null)
                return;

            Vector2 point = getPointToForce(selectedMetal.metal.transform.position);
            Debug.DrawLine(transform.position, point, Color.red, 1f);
            Debug.DrawLine(selectedMetal.metal.transform.position, point, Color.blue, 1f);

            Gizmos.DrawWireSphere(transform.position, playerData.metalCheckRadius);
        }
        
    }
}

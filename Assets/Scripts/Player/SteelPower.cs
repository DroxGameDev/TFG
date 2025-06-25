using System.Collections;
using UnityEngine;

public class SteelPower : Iron_Steel
{
    private float burningSteelCounter;
    private bool impulsePushedObject = false;
    private bool pushingObject = false;
    public bool active = false;
    public IEnumerator SteelInputupdate(bool context)
    {
        input = context;

        yield return null;
    }
    void Update()
    {
        OnUpdate();

        switch (state)
        {
            case PowerState.inactive:
                if (input)
                {
                    ChangeState(PowerState.select);
                    OnSelect();
                }
                break;

            case PowerState.select:
                if ((!input || selectMetalCounter <= 0) && playerData.burningSteel)
                {
                    HandleSelection();
                    input = false;
                }
                break;

            case PowerState.impulse:
                if (playerData.burningSteel)
                    HandleImpulse();
                break;

            case PowerState.force:
                if (playerData.burningSteel)
                    HandleForce();
                break;
        }

        if (playerData.burningSteel && (playerResources.steelEmpty || playerData.damaged || playerData.dead))
        {
            ChangeState(PowerState.inactive);
            OnInactive();
            input = false;
        }
    }

    private void HandleSelection()
    {
        if (selectedMetal == null)
        {
            ChangeState(PowerState.inactive);
            OnInactive();
        }
        else if (selectedMetal.metal.tag == "Arrow")
        {
            ChangeArrowDirection(selectedMetal.metal.GetComponent<ArrowCollisions>().origin, 1);
            ChangeState(PowerState.inactive);
            OnInactive();
        }
        else
        {
            base.OnImpulse();
            ChangeState(PowerState.impulse);
            OnImpulse();
            StartCoroutine(pushObject(rb, selectedMetal.metal.attachedRigidbody));
        }
    }
    private void HandleImpulse()
    {
        if (pushingObject)
        {
            ChangeState(PowerState.force);
            OnForce();
        }
        else
        {
            burningSteelCounter -= Time.deltaTime;

            if (burningSteelCounter <= 0.001f)
            {
                burningSteelCounter = 0f;
                ChangeState(PowerState.inactive);
                OnInactive();
            }
        }
    }
    private void HandleForce()
    {
        if (selectedMetal.metal == null)
        {
            ChangeState(PowerState.inactive);
            OnInactive();
        }
        else if (!pushingObject)
        {
            if (selectedMetal.metal.tag == "Heavy_Metal")
            {
                ChangeState(PowerState.inactive);
                OnInactive();
            }
            else
            {
                ChangeState(PowerState.impulse);
                OnImpulse();
            }
        }
    }
    public override void OnInactive()
    {
        playerData.burningSteel = false;
        Time.timeScale = 1f;
        ResetLines();
    }

    public override void OnSelect()
    {
        if (GetNearbyMetals())
        {
            base.OnSelect();
            playerData.burningSteel = true;
            Time.timeScale = 0.1f;
            selectMetalCounter = playerData.selectMetalTime;
            setLinesDirection();
            DetectPushing();
        }
        else
        {
            ChangeState(PowerState.inactive);
            OnInactive();
        }
    }

    public override void OnForce()
    {
        playerData.burningSteel = true;
        playerData.movingWithPowers = true;
        Time.timeScale = 1f;
        
    }

    public override void OnImpulse()
    {
        playerData.movingWithPowers = true;
        Time.timeScale = 1f;
        burningSteelCounter = playerData.steelPushTime;
    }


    private Vector2 getImpulse(Vector2 metalPosition)
    {
        Vector2 position = new Vector2(playerData.linesOrigin.position.x, playerData.linesOrigin.position.y);

        Vector2 directorVector;
        if (selectedMetal.metal.tag == "Walkable_Area")
        {
            directorVector = metalPosition - position + (selectedMetal.metal.GetComponent<WalkableArea>().worldOffest * -1);
        }
        else
        {
            directorVector = metalPosition - position;
        }
        directorVector.Normalize();
        rb.velocity = Vector3.zero;
        
        return directorVector * playerData.steelPushPower * selectedMetal.iValue * -1; ;
    }

    private IEnumerator pushObject(Rigidbody2D origin, Rigidbody2D target)
    {
        Vector2 forceAmount;


        if (target.tag == "Walkable_Area")
        {
            Vector2 MetalClosestPoint = target.GetComponent<BoxCollider2D>().ClosestPoint(playerData.linesOrigin.position);
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

                if (Mathf.Abs(origin.velocity.x) < 0.01f)
                {
                    pushingObject = true;
                    yield return StartCoroutine(moveAwayFromPlayer(target.GetComponent<Collider2D>(), origin.GetComponent<Collider2D>()));

                }
            }
            else if (target.tag == "Coin" || target.tag == "Vial")
            {
                pushingObject = true;
                
                if (target.gameObject == playerData.showedCoin)
                    playerResources.SteelCoin(target.gameObject.GetComponent<Coin>());

                if (target.tag == "Coin")
                {
                    if(!Physics2D.Raycast(target.gameObject.GetComponent<Coin>().checkGroundPosition.position,Vector2.down, 1f, playerData.groundLayer))
                        target.gameObject.GetComponent<Coin>().attackCollider.enabled = true;
                }

                yield return StartCoroutine(moveAwayFromPlayer(target.GetComponent<Collider2D>(), origin.GetComponent<Collider2D>()));

                if(target != null){
                    if (target.tag == "Coin")
                    {
                        target.gameObject.GetComponent<Coin>().attackCollider.enabled = false;
                    }

                    if (impulsePushedObject)
                    {
                        playerData.movingWithPowers = false;
                        target.AddForce(forceAmount * -1, ForceMode2D.Impulse);
                    }
                    else
                    {
                        origin.AddForce(forceAmount, ForceMode2D.Impulse);
                    }
                }
                
            }
        }
        yield return new WaitForEndOfFrame();
    }
    private Vector2 getPointToForce(Vector2 metalPosition)
    {
        Vector2 playerPosition = new Vector2(playerData.linesOrigin.position.x, playerData.linesOrigin.position.y);
        Vector2 directorVector = metalPosition - playerPosition;
        directorVector.Normalize();
        return  playerPosition + directorVector * playerData.metalCheckRadius;
    }

    private IEnumerator moveAwayFromPlayer(Collider2D metal, Collider2D player)
    {
        Vector2 objectivePosition = getPointToForce(metal.transform.position);
        bool metalObstacleReached = false;

        Vector2 currentPosition = metal.transform.position;
        Vector2 direction = (objectivePosition - currentPosition).normalized;

        if (metal.gameObject.tag == "Heavy_Metal")
        {
            var heavyMetal = metal.GetComponent<Metal_Heavy_Object>();
            heavyMetal.ForceMove();

            if (player.transform.position.x > currentPosition.x)
            {
                direction = Vector2.left;
            }
            else
            {
                direction = Vector2.right;
            }
            //nueva posición
            objectivePosition = currentPosition + direction * (playerData.metalCheckRadius - Vector2.Distance(currentPosition, playerData.linesOrigin.position));

        }

        while (Vector2.Dot(direction, objectivePosition - currentPosition) > 0.1f && !metalObstacleReached )
        {
            if (metal == null || metal.gameObject == null || metal.attachedRigidbody == null)
            {
                pushingObject = false;
                yield break;
            }

            currentPosition = metal.transform.position;

            float step = playerData.steelPushPower / metal.attachedRigidbody.mass * Time.fixedDeltaTime;

            ContactFilter2D filter = new ContactFilter2D();
            if (selectedMetal.metal.tag == "Vial" || selectedMetal.metal.tag == "Coin")
            {
                filter.SetLayerMask(playerData.obstacleLayerMinusOpenWall);
            }
            else
            {
                filter.SetLayerMask(playerData.obstacleLayer);
            }
            
            //filter.useLayerMask = true;

            RaycastHit2D[] hits = new RaycastHit2D[5]; ;
            int hitCount = metal.attachedRigidbody.Cast(direction, filter, hits, step);

            if (hitCount > 0)
            {
                float distanceToObstacle = hits[0].distance;
                // Si hay colisión, mueve solo hasta el punto de colisión
                if (distanceToObstacle < 0.1f)
                {
                    metal.attachedRigidbody.velocity = Vector2.zero;

                    if (Mathf.Abs(direction.y) > 0.7f || direction.y == 0f)
                    {
                        metalObstacleReached = true;
                    }

                    else
                    {
                        //nueva dirección
                        direction = player.transform.position.x > currentPosition.x ? Vector2.left : Vector2.right;
                        //nueva posición
                        objectivePosition = currentPosition + direction * (playerData.metalCheckRadius - Vector2.Distance(currentPosition, playerData.linesOrigin.position));
                    }
                }
                else
                {
                    // Reduce velocidad para no atravesar el obstáculo
                    metal.attachedRigidbody.velocity = direction * (distanceToObstacle / Time.fixedDeltaTime);
                }
            }
            else
            {
                metal.attachedRigidbody.velocity = direction * (step / Time.fixedDeltaTime);
            }

            yield return new WaitForFixedUpdate();
        }

        if (metal.gameObject.tag == "Heavy_Metal")
        {
            var heavyMetal = metal.GetComponent<Metal_Heavy_Object>();
            heavyMetal.Stop();
        }

        if ((metal.gameObject.tag == "Coin" || metal.gameObject.tag == "Vial") && !metalObstacleReached)
        {
            impulsePushedObject = true;
            metal.attachedRigidbody.velocity = Vector2.zero;
        }
        else if ((metal.gameObject.tag == "Coin" || metal.gameObject.tag == "Vial") && metalObstacleReached)
        {
            impulsePushedObject = false;
        }
        pushingObject = false;
    }

    private void OnDrawGizmos()
    {
        if (active)
        {
            /*
            if (selectedMetal == null)
                return;

            Vector2 point = getPointToForce(selectedMetal.metal.transform.position);
            Debug.DrawLine(transform.position, point, Color.red, 1f);
            Debug.DrawLine(selectedMetal.metal.transform.position, point, Color.blue, 1f);

            Gizmos.DrawWireSphere(transform.position, playerData.metalCheckRadius);
            */
            Gizmos.color = Color.blue;
            //Gizmos.DrawWireSphere(groundPosition,groundRadius);

            Vector2 positon = new Vector2(playerData.linesOrigin.position.x, playerData.linesOrigin.position.y);
            Vector2 direction = selectMetalVector + positon;
            Gizmos.DrawLine(positon, direction);
            
            Debug.Log("Select Metal Vector: " + selectMetalVector);
        }
        
    }
}

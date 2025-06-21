using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEditor;
using UnityEngine;

public class IronPower : Iron_Steel
{
    private float burningIronCounter;
    private Vector2 directorVectorImpulse;
    
    private Vector2 forceTargetPosition;
     public WalkableArea walkableArea;

    public List<WalkableArea> nearWalkableAreas;
    

    private bool obstacleReached = false;
    
    public IEnumerator IronInputupdate(bool context)
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
 
        if (input && state == PowerState.inactive && !playerData.showingCoin)
        {

            ChangeState(PowerState.select);
            OnSelect();
        }
        else if (playerData.burningIron && playerResources.ironEmpty)
        {
            ChangeState(PowerState.inactive);
            OnInactive();
            input = false;
        }

        else if ((!input || selectMetalCounter <= 0) && state == PowerState.select && playerData.burningIron)
        {
            if (selectedMetal == null)
            {
                ChangeState(PowerState.inactive);
                OnInactive();
            }
            else if (selectedMetal.metal.tag == "Arrow")
            {
                ChangeArrowDirection(selectedMetal.metal.GetComponent<ArrowCollisions>().origin, -1);
                ChangeState(PowerState.inactive);
                OnInactive();
            }
            else
            {
                base.OnForce();
                ChangeState(PowerState.force);
                OnForce();
            }
            input = false;
        }
        //Debug.Log ("3:" + state);

        else if (state == PowerState.force && playerData.burningIron)
        {

            if (ObjectiveReached(selectedMetal.metal))
            {
                if (selectedMetal.metal.tag == "Environment_metal")
                {
                    if (playerData.grounded)
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
                else if (selectedMetal.metal.tag == "Coin" || selectedMetal.metal.tag == "Vial")
                {
                    playerResources.IronItem(selectedMetal.metal.gameObject);

                    ChangeState(PowerState.inactive);
                    OnInactive();
                }
                else if (CheckIfWalkable())
                {
                    ChangeState(PowerState.wallWalking);
                    OnWallWalk();
                }
                else
                {
                    ChangeState(PowerState.inactive);
                    OnInactive();
                }
            }

            else if (obstacleReached)
            {
                ChangeState(PowerState.inactive);
                OnInactive();
            }

        }

        else if (state == PowerState.wallWalking && playerData.burningIron && (!CheckIfWalkable() || playerData.gravityMode == GravityMode.Down))
        {
            ChangeState(PowerState.inactive);
            OnInactive();
        }

        //Debug.Log ("4:" + state);

        else if (state == PowerState.impulse && playerData.burningIron)
        {
            burningIronCounter -= Time.deltaTime;

            if (burningIronCounter <= 0.001f)
            {
                ChangeState(PowerState.inactive);
                OnInactive();
            }
        }

        //Debug.Log ("5:" + state);
    }
    public override void OnInactive(){
        playerData.burningIron = false;
        Time.timeScale = 1f;
        obstacleReached = false;
        playerData.ChangeGravityMode(GravityMode.Down);
        ResetLines();
    }

    public override void OnSelect()
    {
        if (GetNearbyMetals())
        {
            base.OnSelect();
            playerData.burningIron = true;
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

    public override void OnForce(){
        playerData.movingWithPowers = true;
        Time.timeScale = 1f;
        //playerData.cancelGravity = true;

        rb.velocity = Vector3.zero;

        if (selectedMetal.metal.tag == "Environment_metal")
        {
            directorVectorImpulse = (selectedMetal.metal.transform.position - transform.position).normalized;
            StartCoroutine(moveTowards(selectedMetal.metal));
        }
        else if (selectedMetal.metal.tag == "Coin" || selectedMetal.metal.tag == "Vial")
        {
            StartCoroutine(moveTowardsPlayer(selectedMetal.metal, col));
        }
        else
        {
            StartCoroutine(moveTowards(selectedMetal.metal));
        }
    }

    private IEnumerator moveTowardsPlayer(Collider2D metal, Collider2D player)
    {
        Vector2 forcePlayerPosition;
        bool metalObstacleReached = false;

        if (metal.gameObject.tag == "Heavy_Metal")
        {
            var heavyMetal = metal.GetComponent<Metal_Heavy_Object>();
            heavyMetal.ForceMove();
        }

        Vector2 currentPosition;
        Vector2 direction;

        Rigidbody2D rbMetal = metal.attachedRigidbody;
        
        while (state == PowerState.force && !ObjectiveReached(metal) && !metalObstacleReached)
        {
            forcePlayerPosition = playerData.linesOrigin.position;
            currentPosition = metal.transform.position;
            direction = (forcePlayerPosition - currentPosition).normalized;

            float speed = playerData.ironPullPower / rbMetal.mass;
            float step = speed * Time.fixedDeltaTime;

            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(playerData.obstacleLayer);

            RaycastHit2D[] hits = new RaycastHit2D[5]; ;
            int hitCount = rbMetal.Cast(direction, filter, hits, step);

            if (hitCount > 0)
            {
                rbMetal.velocity = Vector2.zero;

                if (hits[0].distance < 0.1f)
                {
                    obstacleReached = true;
                    metalObstacleReached = true;
                }
            }
            else
            {
                rbMetal.velocity = direction * speed;
            }

            yield return new WaitForFixedUpdate();
        }

        if (metal.gameObject.tag == "Heavy_Metal")
        {
            var heavyMetal = metal.GetComponent<Metal_Heavy_Object>();
            heavyMetal.Stop();
        }
    }
    private IEnumerator moveTowards(Collider2D target)
    {
        // Determinar punto objetivo inicial
        if (target.gameObject.tag == "Floor" || target.gameObject.tag == "Walkable_Area")
        {
            forceTargetPosition = target.ClosestPoint(transform.position);
        }
        else
        {
            forceTargetPosition = target.transform.position;
        }

        Vector2 currentPosition;
        Vector2 direction; 
        
        while (state == PowerState.force && !ObjectiveReached(target) && !obstacleReached)
        {
            forceTargetPosition = (target.gameObject.tag == "Floor" || target.gameObject.tag == "Walkable_Area") ?
            target.ClosestPoint(transform.position) : target.transform.position;

            currentPosition = transform.position;
            direction = (forceTargetPosition - currentPosition).normalized;

            float speed = playerData.ironPullPower / rb.mass;
            float step = speed * Time.fixedDeltaTime;

            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(playerData.obstacleLayer);
            filter.useLayerMask = true;

            RaycastHit2D[] hits = new RaycastHit2D[5]; ;
            int hitCount = col.Cast(direction, filter, hits, step);


            if (hitCount > 0)
            {
                Debug.Log(hits[0].distance);      
                // Si hay colisión, mueve solo hasta el punto de colisión
                rb.velocity = Vector2.zero;
                if (hits[0].distance < 1f)
                {
                    if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                    {
                        direction = transform.position.x < target.transform.position.x ?
                            Vector2.right : Vector2.left;

                        rb.velocity = direction * speed;
                    }
                    else
                    {
                        if (target.tag == "Heavy_Metal")
                        {
                            yield return StartCoroutine(moveTowardsPlayer(target, col));
                        }
                        else
                        {
                            obstacleReached = true;
                        }
                    }
                }
            }
            else
            { 
                rb.velocity = direction * speed;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    private bool ObjectiveReached(Collider2D target){
        return col.IsTouching(target);
    }
    private bool CheckIfWalkable(){
        return col.IsTouchingLayers(playerData.walkableAreaLayer);
    }

    private void OnWallWalk(){
        playerData.movingWithPowers = false;
        rb.velocity = Vector2.zero;
        playerData.ChangeGravityMode(walkableArea.gravity);
    }

    public override void OnImpulse(){
        burningIronCounter = playerData.ironPullTime;
        //playerData.cancelGravity = false;

        directorVectorImpulse.Normalize();

        rb.velocity = Vector3.zero;
        Vector2 forceToApply = directorVectorImpulse * playerData.ironPullPower;

        if (selectedMetal.metal.attachedRigidbody.bodyType == RigidbodyType2D.Static){
            rb.velocity = Vector2.zero;
            rb.AddForce(forceToApply, ForceMode2D.Impulse);
        }
    }

    public void SetWalkableArea(WalkableArea area)
    {
        if (walkableArea == null)
            walkableArea = area;
    }

    public void ResetWalkableArea(WalkableArea area)
    {
        if (area == walkableArea)
            walkableArea = null;
    }

    private void OnDrawGizmos()
    {
        if (rb != null)
        {
            /*
            Vector2 position = new Vector2(transform.position.x, transform.position.y);

            Vector2 velocityEnd = position + new Vector2(rb.velocity.x, rb.velocity.y);
            
            Vector2 velocityXPositive = position + new Vector2(rb.velocity.x, 0);
            Vector2 velocityYPositive = position + new Vector2(0, rb.velocity.y);

            Gizmos.color = Color.blue; // Color para las líneas de velocidad
            Gizmos.DrawLine(position, velocityEnd); // Línea de velocidad tota
            Gizmos.DrawLine(position, velocityXPositive); // Dirección X positiva
            Gizmos.DrawLine(position, velocityYPositive); // Dirección Y positiva

            // Opcional: Dibuja esferas en los extremos para mayor claridad
            Gizmos.color = Color.red; // Color para las esferas
            Gizmos.DrawSphere(velocityEnd, 0.1f); 
            Gizmos.DrawSphere(velocityXPositive, 0.1f);
            Gizmos.DrawSphere(velocityYPositive, 0.1f);
            */
        }
    }


}

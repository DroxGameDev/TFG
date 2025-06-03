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

            if (selectedMetal != null)
            {
                base.OnForce();
                ChangeState(PowerState.force);
                OnForce();
            }
            else
            {
                ChangeState(PowerState.inactive);
                OnInactive();
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
                    ChangeState(PowerState.impulse);
                    OnImpulse();
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

        if(selectedMetal.metal.tag == "Environment_metal"){
            directorVectorImpulse = selectedMetal.metal.transform.position - transform.position;
            directorVectorImpulse.Normalize();
        }

        if (selectedMetal.metal.tag == "Coin" || selectedMetal.metal.tag == "Vial")
        {
            StartCoroutine(moveTowardsPlayer(selectedMetal.metal, col));
        }
        else
        {
            StartCoroutine(moveTowards(col, selectedMetal.metal));
        }
    }

    private IEnumerator moveTowardsPlayer(Collider2D metal, Collider2D player)
    {
        Vector2 forcePlayerPosition = playerData.linesOrigin.position;
        bool metalObstacleReached = false;
        if (metal.gameObject.tag == "Heavy_Metal")
        {
            var heavyMetal = metal.GetComponent<Metal_Heavy_Object>();
            heavyMetal.ForceMove();
        }
        Vector2 currentPosition = metal.transform.position;
        Vector2 direction = (forcePlayerPosition - currentPosition).normalized;
        while (state == PowerState.force && !ObjectiveReached(metal) && !metalObstacleReached)
        {   
            forcePlayerPosition = playerData.linesOrigin.position;
            currentPosition = metal.transform.position;

            float step = playerData.ironPullPower * playerData.ironPullPowerMult / metal.attachedRigidbody.mass * Time.fixedDeltaTime;

            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(playerData.obstacleLayer);

            //filter.useLayerMask = true;

            RaycastHit2D[] hits = new RaycastHit2D[5]; ;
            int hitCount = metal.attachedRigidbody.Cast(direction, filter, hits, step);

            if (hitCount > 0)
            {
                // Si hay colisión, mueve solo hasta el punto de colisión
                metal.attachedRigidbody.MovePosition(currentPosition + direction * hits[0].distance);
                if (hits[0].distance < 0.1f)
                {
                    obstacleReached = true;
                    metalObstacleReached = true;

                    if (direction.y >= 0f || direction.y < -0.4f)
                    {
                        obstacleReached = true;
                        metalObstacleReached = true;
                    }
                    else
                    {
                        if (metal.transform.position.x < playerData.linesOrigin.position.x)
                        {
                            direction = Vector2.right;
                        }
                        else
                        {
                            direction = Vector2.left;
                        }
                        
                    }
                }
            }
            else
            {
                // Si no hay colisión, mueve normalmente
                Vector2 newPosition = Vector2.MoveTowards(currentPosition, forcePlayerPosition, step);
                metal.attachedRigidbody.MovePosition(newPosition);
            }

            yield return new WaitForFixedUpdate();
        }

        if (metal.gameObject.tag == "Heavy_Metal")
        {
            var heavyMetal = metal.GetComponent<Metal_Heavy_Object>();
            heavyMetal.Stop();
        }
    }
    private IEnumerator moveTowards(Collider2D origin, Collider2D target)
    {

        if (target.gameObject.tag == "Floor" || target.gameObject.tag == "Walkable_Area")
        {
            forceTargetPosition = target.ClosestPoint(origin.gameObject.transform.position);
        }
        else
        {
            forceTargetPosition = target.transform.position;
        }

        Vector2 currentPosition = origin.transform.position;
        Vector2 direction = (forceTargetPosition - currentPosition).normalized; 
        
        while (state == PowerState.force && !ObjectiveReached(target) && !obstacleReached)
        {
            currentPosition = origin.transform.position;

            float step = playerData.ironPullPower * playerData.ironPullPowerMult * Time.fixedDeltaTime;

            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(playerData.obstacleLayer);
            filter.useLayerMask = true;

            RaycastHit2D[] hits = new RaycastHit2D[5]; ;
            int hitCount = col.Cast(direction, filter, hits, step);

            if (hitCount > 0)
            {
                // Si hay colisión, mueve solo hasta el punto de colisión
                origin.attachedRigidbody.MovePosition(currentPosition + direction * hits[0].distance);
                if (hits[0].distance < 0.1f)
                {
                    if (direction.y >= 0f || direction.y < -0.4f)
                    {
                        if (target.tag == "Heavy_Metal")
                        {
                            yield return StartCoroutine(moveTowardsPlayer(target, origin));
                        }
                        else
                        {
                            obstacleReached = true;
                        }
                    }
                    else
                    {
                        if (origin.transform.position.x < target.transform.position.x)
                        {
                            direction = Vector2.right;
                        }
                        else
                        {
                            direction = Vector2.left;
                        }

                    }
                }
            }
            else
            {
                // Si no hay colisión, mueve normalmente
                Vector2 newPosition = Vector2.MoveTowards(currentPosition, forceTargetPosition, step);
                origin.attachedRigidbody.MovePosition(newPosition);
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

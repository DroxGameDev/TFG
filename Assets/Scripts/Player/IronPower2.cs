using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class IronPower2 : Iron_Steel2
{
    private float burningIronCounter;
    private Vector2 directorVectorImpulse;
    
    private bool wallWalking;
    
    public IEnumerator IronInputupdate(bool context)
    {
        if(context){
            input = true;
        }

        else if(!context){
            input = false;
        }

        yield return null;
    }
    public void Start()
    {
        
    }

    void Update()
    {
        Debug.Log(state);

        OnUpdate();
        if (input && state == PowerState.inactive){

            ChangeState(PowerState.select);
            OnSelect();
        }
        

        else if((!input || selectMetalCounter <= 0) && state == PowerState.select && playerData.burningIron){

            if(selectedMetal != null){
                ChangeState(PowerState.force);
                OnForce();
            }
            else{
                ChangeState(PowerState.inactive);
                OnInactive();
            }
            input = false;
        }
        //Debug.Log ("3:" + state);

        else if(state == PowerState.force && playerData.burningIron){
            if (ObstacleReached()){
                ChangeState(PowerState.inactive);
                OnInactive();
            }
            
            else if (ObjectiveReached(selectedMetal.metal)){
                if (selectedMetal.metal.tag == "Environment_metal"){
                    ChangeState(PowerState.impulse);
                    OnImpulse();
                }
                else{
                    ChangeState(PowerState.inactive);
                    OnInactive();
                }
            }
        }

        //Debug.Log ("4:" + state);

        else if(state == PowerState.impulse && playerData.burningIron){
            burningIronCounter -= Time.deltaTime;

            if(burningIronCounter <= 0.001f){
                ChangeState(PowerState.inactive);
                OnInactive();
            }
        }

        //Debug.Log ("5:" + state);
    }
    public override void OnInactive(){
        playerData.burningIron = false;
        playerData.movingWithPowers = false;
        playerData.timeStoped = false;
        //playerData.cancelGravity = false;
        ResetLines();
    }

    public override void OnSelect()
    {
        if (GetNearbyMetals()){
            playerData.burningIron = true;
            playerData.timeStoped = true;
            selectMetalCounter = playerData.selectMetalTime;
        }
        else{
            ChangeState(PowerState.inactive);
            OnInactive();
        }
    }

    public override void OnForce(){
        playerData.movingWithPowers = true;
        playerData.timeStoped = false;
        //playerData.cancelGravity = true;

        rb.velocity = Vector3.zero;

        if(selectedMetal.metal.tag == "Environment_metal"){
            directorVectorImpulse = selectedMetal.metal.transform.position - transform.position;
            directorVectorImpulse.Normalize();
        }

        StartCoroutine(moveTowards(collider, selectedMetal.metal));
    }

    private IEnumerator moveTowards(Collider2D origin, Collider2D target){  
        Vector2 targetPosition;
        if(target.gameObject.tag == "Floor"){
            targetPosition = target.ClosestPoint(origin.gameObject.transform.position);
        }
        else{
            targetPosition = target.transform.position;
        }
        
        while(state== PowerState.force && !ObjectiveReached(target) && !ObstacleReached()){
            float step = playerData.ironPullPower * playerData.ironPullPowerMult * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            origin.transform.position = Vector2.MoveTowards(origin.transform.position, targetPosition, step);
        }

    }

    private bool ObjectiveReached(Collider2D target){
        return playerData.forceCollider.IsTouching(target);
    }

    private bool ObstacleReached(){
        return playerData.forceCollider.IsTouchingLayers(playerData.obstacleLayer);
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
        
        ResetLines();
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

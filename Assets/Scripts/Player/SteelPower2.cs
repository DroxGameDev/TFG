using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelPower2 : Iron_Steel2
{
    private float burningSteelCounter;
    public IEnumerator SteelInputupdate(bool context)
    {
        if(context){
            input = true;
        }

        else if(!context){
            input = false;
        }

        yield return null;
    }
    void Update()
    {
        OnUpdate();

        //Debug.Log ("1:" + state);

        if (input && state == PowerState.inactive){
            ChangeState(PowerState.select);
            OnSelect();
        }

        //Debug.Log ("2:" + state);

        else if((!input || selectMetalCounter <= 0) && state == PowerState.select && playerData.burningSteel){

            if(selectedMetal != null){
                ChangeState(PowerState.impulse);
                OnImpulse();
            }
            else{
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
    public override void OnInactive(){
        playerData.burningSteel = false;
        playerData.movingWithPowers = false;
        playerData.timeStoped = false;
        ResetLines();
    }

    public override void OnSelect()
    {
        if (GetNearbyMetals()){
            playerData.burningSteel = true;
            playerData.timeStoped = true;
            selectMetalCounter = playerData.selectMetalTime;
        }
        else{
            ChangeState(PowerState.inactive);
            OnInactive();
        }
    }

    public override void OnForce(){
    }

    public override void OnImpulse(){
        playerData.movingWithPowers = true;
        playerData.timeStoped = false;
        burningSteelCounter = playerData.steelPushTime;

        StartCoroutine(pushObject(rb,selectedMetal.metal.attachedRigidbody));

        ResetLines();
    }

    private Vector2 getImpulse(Vector3 metalPosition){

        Vector2 directorVector = metalPosition-transform.position;
        directorVector.Normalize();
        rb.velocity = Vector3.zero;
        Vector2 forceToApply = directorVector * playerData.steelPushPower * selectedMetal.iValue * -1;

        return forceToApply;
    }

    private IEnumerator pushObject(Rigidbody2D origin, Rigidbody2D target){
        Vector2 forceAmount;

        if(target.tag == "Floor"){
            Vector2 MetalClosestPoint = target.GetComponent<BoxCollider2D>().ClosestPoint(origin.gameObject.transform.position);
            forceAmount = getImpulse(MetalClosestPoint);
            origin.AddForce(forceAmount, ForceMode2D.Impulse);
        }
        else{
            forceAmount = getImpulse(target.gameObject.transform.position);
            if (target.tag == "Environment_metal"){
                origin.AddForce(forceAmount, ForceMode2D.Impulse);
            }
            else if (target.tag == "Heavy_Metal"){
                origin.AddForce(forceAmount, ForceMode2D.Impulse);

                yield return new WaitForFixedUpdate();

                if(origin.velocity.x == 0f){
                        target.constraints = RigidbodyConstraints2D.FreezeRotation;
                        target.AddForce(forceAmount * -1, ForceMode2D.Impulse);
                }
            }
        }
        

    }
}

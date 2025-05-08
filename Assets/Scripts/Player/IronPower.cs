using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class IronPower : Iron_Steel
{
    private bool usingIron = false;
    private bool movingTowards = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerData = GetComponent<PlayerData>();
        nearMetalLines = new List<LineObject>();

    }

    public IEnumerator IronInputupdate(bool context)
    {
        if(!playerData.movingWithPowers){
            if (context && nearMetalLines.Count == 0){

                GetNearbyMetals();
            }

            else if ((!context||selectMetalCounter <= 0.01) && nearMetalLines.Count > 0){

                playerData.timeStoped = false;

                movingWithPowerCounter = playerData.ironPullTime;
                playerData.movingWithPowers = true;
                usingIron = true;
                
                Time.timeScale = 1f;

        
                Vector2 directorVector = selectedMetal.metal.transform.position-transform.position;
                directorVector.Normalize();

                rb.velocity = Vector3.zero;

                Vector2 forceToApply = directorVector * playerData.ironPullPower;

                yield return StartCoroutine(moveTowardsMetal(forceToApply));

                StartCoroutine(pullObject(rb,selectedMetal.metal.attachedRigidbody, forceToApply));


                selectedMetal = null;

                for (int i = 0; i < nearMetalLines.Count; i++){
                    Destroy(nearMetalLines[i].line);
                }    

                nearMetalLines.Clear();
            }

            yield return null;

        }
    }

    void Update()
    {
        if(movingWithPowerCounter>0.01f && usingIron && !movingTowards){
            movingWithPowerCounter -= Time.deltaTime;
        }

        if (playerData.movingWithPowers && movingWithPowerCounter <= 0.01f){
            playerData.movingWithPowers = false;
            usingIron = false;
            movingWithPowerCounter = 0f;
        }
        onUpdate();
    }

    public IEnumerator moveTowardsMetal(Vector2 force){
        
        movingTowards = true;
        playerData.cancelGravity = true;
    
        while (movingTowards && playerData.movingWithPowers) {
            float speedDifX = force.x - rb.velocity.x;
            float speedDifY = force.y - rb.velocity.y;

            float movementX = Math.Abs(speedDifX) * playerData.ironPullPowerMult * Mathf.Sign(speedDifX);
            float movementY = Math.Abs(speedDifY) * playerData.ironPullPowerMult * Mathf.Sign(speedDifY);
            
            rb.AddForce(new Vector2(movementX, movementY));

            yield return new WaitForFixedUpdate();

            Collider2D check = Physics2D.OverlapCircle(transform.position, 0.5f, playerData.metalLayers);

            if (check != null && 
                check.attachedRigidbody.gameObject == selectedMetal.metal.attachedRigidbody.gameObject){
                movingTowards = false;
            }
        }    

        playerData.cancelGravity = false;
        rb.velocity = Vector2.zero;
    }

    private IEnumerator pullObject(Rigidbody2D origin, Rigidbody2D target, Vector2 forceAmount){

        if (target.bodyType == RigidbodyType2D.Static){
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

    private void OnDrawGizmos()
    {
        if (active){
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,0.5f);
        }
        
    }
    
}

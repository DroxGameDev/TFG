using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IronPower : Iron_Steel
{

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerData = GetComponent<PlayerData>();
        nearMetalLines = new List<LineObject>();

    }

    public IEnumerator IronInputupdate(bool context)
    {
        if (context && nearMetalLines.Count == 0){

            GetNearbyMetals();
        }

        while(selectMetalCounter > 0.01f && context){

            yield return null;
        }
        
        if ((!context||selectMetalCounter <= 0.01) && nearMetalLines.Count > 0){
            playerData.timeStoped = false;

            movingWithPowerCounter = playerData.ironPullTime;
            playerData.movingWithPowers = true;

            if(selectedMetal != null){
                
                Vector2 directorVector = selectedMetal.metal.transform.position-transform.position;
                directorVector.Normalize();
                rb.velocity = Vector3.zero;
                Vector2 forceToApply = directorVector * playerData.ironPullPower * selectedMetal.iValue;

                StartCoroutine(pullObject(rb,selectedMetal.metal.attachedRigidbody, forceToApply));

            }

            selectedMetal = null;

            for (int i = 0; i < nearMetalLines.Count; i++){
                Destroy(nearMetalLines[i].line);
            }    

            nearMetalLines.Clear();
            Time.timeScale = 1;
        }
    }

    

    void Update()
    {
        if(movingWithPowerCounter>0.01f){
            movingWithPowerCounter -= Time.deltaTime;
        }
        else if (playerData.movingWithPowers && movingWithPowerCounter <= 0.01f && playerData.running){
            playerData.movingWithPowers = false;
            movingWithPowerCounter = 0f;
        }
        onUpdate();
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
    
}

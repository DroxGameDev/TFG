using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelPower : MonoBehaviour
{
    private Rigidbody2D rb;
    private PlayerData playerData;

    [Header("Steel")]
    private List<LineObject> nearMetalLines;
    private Vector2 selectMetalVector;
    private LineObject selectedMetal = null;
    private float steelPushCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerData = GetComponent<PlayerData>();
        nearMetalLines = new List<LineObject>();

    }

    public IEnumerator SteelInputupdate(bool context)
    {
        if (context && nearMetalLines.Count == 0){

            Collider2D[] nearMetals = Physics2D.OverlapCircleAll(transform.position, playerData.metalCheckRadius, playerData.metalEnvironmentLayer);

            for (int i = 0; i < nearMetals.Length; i++){

                GameObject newLinePrefab = Instantiate(playerData.linePrefab);
                LineObject newLineObject = new LineObject(newLinePrefab, nearMetals[i]);
                nearMetalLines.Add(newLineObject);
            }    

            

        }

        if (nearMetalLines.Count > 0){
            playerData.timeStoped = true;

            Time.timeScale = 0f;

            while(context)
            yield return null;

            playerData.timeStoped = false;
        }
        
        if (!context && nearMetalLines.Count > 0){

            steelPushCounter = playerData.steelPushTime;
            playerData.movingWithPowers = true;

            if(selectedMetal != null){

                Vector2 directionVector = selectedMetal.metal.transform.position-transform.position;
                directionVector.Normalize();
                rb.velocity = Vector3.zero;
                rb.AddForce(directionVector * playerData.steelPushPower * selectedMetal.iValue * -1, ForceMode2D.Impulse);
            }

            selectedMetal = null;
            for (int i = 0; i < nearMetalLines.Count; i++){
                Destroy(nearMetalLines[i].line);
            }    
            nearMetalLines.Clear();
        }
        Time.timeScale = 1;
    }

    public void GetSelectMetalAngle(Vector2 context)
    {
        selectMetalVector = context;
    }

    void Update()
    {
        if(steelPushCounter>0){
            steelPushCounter -= Time.deltaTime;
        }
        else if (steelPushCounter <= 0 && playerData.running){
            playerData.movingWithPowers = false;
            steelPushCounter = 0f;
        }

        //actualizar lineas
        if(nearMetalLines.Count > 0){


            #region metal lines positioning
            for(int i = 0; i < nearMetalLines.Count; i++){
                LineObject actualLine = nearMetalLines[i];
                
                actualLine.lineRenderer.SetPosition(0, transform.position);
                actualLine.lineRenderer.SetPosition(1, actualLine.metal.transform.position);

                Vector2 MetalClosestPoint = nearMetalLines[i].metal.GetComponent<BoxCollider2D>().ClosestPoint(transform.position);
                float lineDistance = Vector2.Distance(transform.position, MetalClosestPoint);

                if(lineDistance <= playerData.metalCheckMinRadius){
                    if (actualLine.iValue != 1){
                        actualLine.iValue = 1f;
                        ChangeMaterialAlpha(actualLine.lineRenderer.material, 1f);
                    }  
                }
                else{
                        actualLine.iValue = Mathf.InverseLerp(playerData.metalCheckRadius,playerData.metalCheckMinRadius, lineDistance);

                        if (actualLine.iValue > 0.5f){
                            actualLine. iValue = 0.6f;
                        }
                        else if (actualLine.iValue >= 0.01){
                            actualLine.iValue = 0.25f;
                        }

                        ChangeMaterialAlpha(actualLine.lineRenderer.material, actualLine.iValue);        
                }

                actualLine.lineRenderer.material.SetFloat("_GlowAmount", 0);
            }
            #endregion
            
            float selectedMetalAngle = 360f;

            for(int i = 0; i< nearMetalLines.Count; i++){
                Vector2 actualMetalVector = nearMetalLines[i].metal.transform.position-transform.position;
                float posibleAngle = Vector2.Angle(selectMetalVector, actualMetalVector);

                if (posibleAngle < selectedMetalAngle){
                    selectedMetal = nearMetalLines[i];
                    selectedMetalAngle = posibleAngle;
                }

                nearMetalLines[i].lineRenderer.material.SetFloat("_GlowAmount", 0);
            }

            selectedMetal.lineRenderer.material.SetFloat("_GlowAmount", 1);
            
        }

        if(playerData.movingWithPowers && IsGrounded())
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(playerData.frictionAmount));

            amount *= Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
        
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(playerData.groundCheck.position, playerData.groundCheckRadius, playerData.groundLayer);
    }

    private void ChangeMaterialAlpha(Material material, float alpha)
    {
        material.SetFloat("_Alpha", alpha);
    }
    /*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position,playerData.metalCheckRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position,playerData.metalCheckMinRadius);

    }
    */
}

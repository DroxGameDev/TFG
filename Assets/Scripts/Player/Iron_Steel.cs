using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iron_Steel : MonoBehaviour
{
    [HideInInspector] public static Rigidbody2D rb;
    [HideInInspector] public static PlayerData playerData;

    [HideInInspector] public static List<LineObject> nearMetalLines;
    [HideInInspector] public static Vector2 selectMetalVector;
    [HideInInspector] public static LineObject selectedMetal = null;
    [HideInInspector] public static float selectMetalCounter;
    [HideInInspector] public static float movingWithPowerCounter;

    [Header("Debug")]
    [SerializeField] public static bool active = false;
    private float radius1;
    private float radius2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerData = GetComponent<PlayerData>();
        nearMetalLines = new List<LineObject>();
    }

    public void GetNearbyMetals(){
        Collider2D[] nearMetals = Physics2D.OverlapCircleAll(transform.position, playerData.metalCheckRadius, playerData.metalLayers);

        for (int i = 0; i < nearMetals.Length; i++){

            GameObject newLinePrefab = Instantiate(playerData.linePrefab);
            LineObject newLineObject = new LineObject(newLinePrefab, nearMetals[i]);
            nearMetalLines.Add(newLineObject);
        }    


        if (nearMetalLines.Count > 0){
            playerData.timeStoped = true;
            selectMetalCounter = playerData.selectMetalTime;
            Time.timeScale = 0.1f;
        }
    }

    public void GetSelectMetalAngle(Vector2 context)
    {
        selectMetalVector = context;
    }

    public void onUpdate()
    {
        if(selectMetalCounter > 0.01f){
            selectMetalCounter -= Time.unscaledDeltaTime;
        }
        if (!playerData.timeStoped){
            selectMetalCounter = 0;
        }
        //actualizar lineas
        if(nearMetalLines.Count > 0){


            #region metal lines position
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
                            actualLine.iValue = 0.6f;
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

        #region debug
        radius1 = playerData.metalCheckRadius;
        radius2 = playerData.metalCheckMinRadius;
        #endregion

    }
    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(playerData.groundCheck.position, playerData.groundCheckRadius, playerData.groundLayer);
    }

    private void ChangeMaterialAlpha(Material material, float alpha)
    {
        material.SetFloat("_Alpha", alpha);
    }

    IEnumerator FakeAddForceMotion(Vector2 forceAmount, Rigidbody2D rb)
    {
        float i = 1f;
        
        while (i > 0f)
        {
            rb.velocity = new Vector2(forceAmount.x * i * -1, -playerData.gravityScale); // !! For X axis positive force

            if(i > 0){
                i -= Time.deltaTime;
            }
            
            yield return new WaitForFixedUpdate();      
        }

        rb.velocity = Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        if (active){
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,radius1);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position,radius2);
        }
        
    }
}

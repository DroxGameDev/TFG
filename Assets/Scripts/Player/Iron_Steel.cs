using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PowerState
{
    inactive,
    select,
    force,
    wallWalking,
    impulse
}

public class Iron_Steel : MonoBehaviour
{
    public static PowerState state;
    [HideInInspector] public bool input;

    [HideInInspector] public static Rigidbody2D rb;
    [HideInInspector] public static PlayerData playerData;
    [HideInInspector] public static Collider2D col;
    [HideInInspector] public static PlayerResources playerResources;

    [HideInInspector] public static List<LineObject> nearMetalLines;
    [HideInInspector] public static Vector2 selectMetalVector;
    [HideInInspector] public static LineObject selectedMetal = null;
    [HideInInspector] public static float selectMetalCounter;
    public static bool transitioning = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        playerData = GetComponent<PlayerData>();
        nearMetalLines = new List<LineObject>();
        playerResources = GetComponent<PlayerResources>();
    }
    public static void GetSelectMetalAngle(Vector2 context)
    {
        selectMetalVector = context;
    }

    public bool GetNearbyMetals()
    {
        Collider2D[] nearMetals = Physics2D.OverlapCircleAll(playerData.linesOrigin.position, playerData.metalCheckRadius, playerData.metalLayers);

        int linesBehindWalls = 0;

        for (int i = 0; i < nearMetals.Length; i++)
        {

            GameObject newLinePrefab = Instantiate(playerData.linePrefab);
            LineObject newLineObject = new LineObject(newLinePrefab, nearMetals[i]);
            nearMetalLines.Add(newLineObject);

            if (IsWallBetween(newLineObject)) linesBehindWalls++;
        }

        return nearMetalLines.Count > 0 && linesBehindWalls < nearMetalLines.Count;
    }

    public void setLinesDirection()
    {
        int value = playerData.burningIron ? 1 : 0;

        for (int i = 0; i < nearMetalLines.Count; i++)
        {
            nearMetalLines[i].lineRenderer.material.SetInt("_In", value);
        }
    }

    private bool IsWallBetween(LineObject lineObject)
    {
        Vector2 MetalClosestPoint = lineObject.metal.GetComponent<BoxCollider2D>().ClosestPoint(playerData.linesOrigin.position);
        float lineDistance = Vector2.Distance(playerData.linesOrigin.position, MetalClosestPoint);

        Vector2 directorVector = MetalClosestPoint + (lineObject.metal.GetComponent<BoxCollider2D>().offset * -1)
                                    - new Vector2(playerData.linesOrigin.position.x, playerData.linesOrigin.position.y);

        directorVector.Normalize();

        RaycastHit2D raycast = Physics2D.Raycast(playerData.linesOrigin.position, directorVector, lineDistance, playerData.obstacleLayer);
        return raycast;
    }

    public virtual void OnInactive()
    {
        StartCoroutine(EndTransition());
    }
    public virtual void OnSelect()
    {
        StartCoroutine(StartTransition());
    }
    public virtual void OnForce()
    {
        StartCoroutine(EndTransition());
    }
    public virtual void OnImpulse()
    {
        StartCoroutine(EndTransition());
    }
    
    private IEnumerator StartTransition()
    {
        float currentStep = 0;
        transitioning = true;

        while (currentStep < 1f)
        {
            playerData.selectMetalMaterial.SetFloat("_SelectMetalAreaStep", currentStep);

            currentStep += playerData.selectMetalTransitionStep;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        transitioning = false;
    }
    
    private IEnumerator EndTransition()
    {
        float currentStep = 1;
        transitioning = true;

        while (currentStep > 0f)
        {
            playerData.selectMetalMaterial.SetFloat("_SelectMetalAreaStep", currentStep);

            currentStep -= playerData.selectMetalTransitionStep;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        transitioning = false;
    }

    public void OnUpdate()
    {
        selectMetalCounter -= Time.unscaledDeltaTime;

        if (state == PowerState.select || state == PowerState.force || state == PowerState.wallWalking || state == PowerState.impulse)
        {
            //actualizar lineas
            #region metal lines position
            for (int i = 0; i < nearMetalLines.Count; i++)
            {
                if (nearMetalLines[i].metal != null)
                {
                    LineObject actualLine = nearMetalLines[i];

                    Vector2 MetalClosestPoint = nearMetalLines[i].metal.GetComponent<BoxCollider2D>().ClosestPoint(playerData.linesOrigin.position);

                    actualLine.lineRenderer.SetPosition(0, playerData.linesOrigin.position);

                    if (actualLine.metal.tag == "Floor")
                    {
                        actualLine.lineRenderer.SetPosition(1, MetalClosestPoint);
                    }
                    else if (actualLine.metal.tag == "Walkable_Area")
                    {
                        Vector2 worldOffset = actualLine.metal.transform.TransformVector(actualLine.metal.GetComponent<BoxCollider2D>().offset * -1);
                        actualLine.lineRenderer.SetPosition(1, MetalClosestPoint + worldOffset);
                    }
                    else
                    {
                        actualLine.lineRenderer.SetPosition(1, actualLine.metal.transform.position);
                    }

                    float lineDistance = Vector2.Distance(playerData.linesOrigin.position, MetalClosestPoint);

                    Vector2 directorVector = MetalClosestPoint - new Vector2(playerData.linesOrigin.position.x, playerData.linesOrigin.position.y);
                    directorVector.Normalize();

                    if (IsWallBetween(nearMetalLines[i]))
                    {
                        actualLine.iValue = 0f;
                        ChangeMaterialAlpha(actualLine.lineRenderer.material, 0f);

                    }
                    else if (lineDistance <= playerData.metalCheckMinRadius)
                    {
                        if (actualLine.iValue != 1f)
                        {
                            actualLine.iValue = 1f;
                            ChangeMaterialAlpha(actualLine.lineRenderer.material, 1f);
                        }
                    }
                    else
                    {
                        actualLine.iValue = Mathf.InverseLerp(playerData.metalCheckRadius, playerData.metalCheckMinRadius, lineDistance);

                        if (actualLine.iValue > 0.5f)
                        {
                            actualLine.iValue = 0.6f;
                        }
                        else if (actualLine.iValue >= 0.01)
                        {
                            actualLine.iValue = 0.25f;
                        }

                        ChangeMaterialAlpha(actualLine.lineRenderer.material, actualLine.iValue);
                    }

                    actualLine.lineRenderer.material.SetFloat("_GlowAmount", 0);
                }
            }
            #endregion

            if (state == PowerState.select)
            {
                selectedMetal = null;
                float selectedMetalAngle = 360f;

                for (int i = 0; i < nearMetalLines.Count; i++)
                {
                    if (nearMetalLines[i].metal != null)
                    {
                        Vector2 currentMetalVector = nearMetalLines[i].metal.transform.position - playerData.linesOrigin.position;
                        float posibleAngle = Vector2.Angle(selectMetalVector, currentMetalVector);

                        if (posibleAngle < selectedMetalAngle && nearMetalLines[i].iValue != 0)
                        {
                            selectedMetal = nearMetalLines[i];
                            selectedMetalAngle = posibleAngle;
                        }

                        nearMetalLines[i].lineRenderer.material.SetFloat("_GlowAmount", 0);
                    }
                }

                if (selectedMetal != null)
                {
                    selectedMetal.lineRenderer.material.SetFloat("_GlowAmount", 1);
                }
            }

            if (state == PowerState.force || state == PowerState.wallWalking || state == PowerState.impulse)
            {
                for (int i = 0; i < nearMetalLines.Count; i++)
                {
                    if (nearMetalLines[i] != selectedMetal)
                    {
                        ChangeMaterialAlpha(nearMetalLines[i].lineRenderer.material, 0f);
                    }
                }

                if (selectedMetal != null)
                {
                    selectedMetal.lineRenderer.material.SetFloat("_GlowAmount", 1);
                }
            }
        }

        if (playerData.movingWithPowers && playerData.grounded)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(playerData.frictionAmount));

            amount *= Mathf.Sign(rb.velocity.x);

            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }

        if (state == PowerState.inactive && playerData.movingWithPowers && playerData.running)
        {
            playerData.movingWithPowers = false;
        }

    }
    private void ChangeMaterialAlpha(Material material, float alpha)
    {
        material.SetFloat("_Alpha", alpha);
    }

    public void ChangeState(PowerState newState)
    {
        state = newState;
    }

    public void ResetLines()
    {
        if (selectedMetal != null)
        {
            selectedMetal = null;
        }

        for (int i = 0; i < nearMetalLines.Count; i++)
        {
            Destroy(nearMetalLines[i].line);
        }
        nearMetalLines.Clear();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibilityChecker : MonoBehaviour
{
    public AffectedByGravity origin;
    
    void OnBecameInvisible()
    {
        origin.GravitySleep();
        origin.rb.Sleep();
    }

    void OnBecameVisible()
    {
        origin.GravityAwake();
        origin.rb.WakeUp();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal_Heavy_Object : AffectedByGravity
{
    void Start()
    {
        OnStart();
    }

    public IEnumerator Impulse(Vector2 force)
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForFixedUpdate();

        while (rb.velocity.magnitude > 0f)
        {
            yield return new WaitForFixedUpdate();
        }

        Stop();
    }

    public void ForceMove()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void Stop()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }

    void FixedUpdate()
    {
        //if (moving && rb.velocity == Vector2.zero) Stop();
    }

}

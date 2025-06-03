using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal_Heavy_Object : AffectedByGravity
{

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

    public void StartPush()
    {
       //rb.bodyType = RigidbodyType2D.Kinematic;
    }

    public void EndPush()
    {
        //rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
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

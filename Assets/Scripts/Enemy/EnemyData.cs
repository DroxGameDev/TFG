using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    light,
    medium,
    hard
}

public class EnemyData : AffectedByGravity
{
    public int health;

    [Header("Damage")]
    public DamageType damageType;
    public float damageWait;
    [Range(0, 1f)] public float hitTime;

    [Header("Animation")]
    public Animator anim;
    public bool isFacingRight = true;
    public bool damaged;

    public void Flip()
    {
        if (Time.timeScale == 1f)
        {
            isFacingRight = !isFacingRight;
            Vector2 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;

            /*
            Vector3 currentShootPoint = playerData.shootPoint.localPosition;
            currentShootPoint.x *= -1f;
            playerData.shootPoint.localPosition = currentShootPoint;
            */
        }
    }


}


using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BreakeableObject : MonoBehaviour, IDamageable
{
    [SerializeField] private int health = 3; // Visible en el Inspect
    public SpriteRenderer sprite;
    private bool shaking = false;
    [Range (0f, 100f)] public float shakeAmount;
    public int Health
    {
        get => health;
        set => health = value;
    }
    public void OnDamage()
    {
        health--;
        StartCoroutine(Shake());
    }
    public void OnDie()
    {

    }

    void Update()
    {
        if (shaking)
        {
            Vector2 newPosition = Random.insideUnitCircle * (Time.deltaTime * shakeAmount);
            newPosition.y = sprite.transform.localPosition.y;

            sprite.transform.localPosition = newPosition;
        }
    }

    IEnumerator Shake()
    {
        Vector2 originalPosition = sprite.transform.localPosition;

        if (!shaking) shaking = true;

        yield return new WaitForSeconds(0.25f);

        shaking = false;
        sprite.transform.localPosition = originalPosition;
    }
}

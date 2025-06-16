using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BreakeableObject : MonoBehaviour, IDamageable
{
    [SerializeField] private int health = 3; // Visible en el Inspect
    public SpriteRenderer sprite;
    private bool shaking = false;
    [Range(0f, 100f)] public float shakeAmount;
    Vector2 originalPosition;
    [Range(0f, 1f)] public float hitTime;
    public int Health
    {
        get => health;
        set => health = value;
    }
    void Start()
    {
        originalPosition = sprite.transform.localPosition;
    }

    public void OnDamage(int damage, float knockbackAmount ,bool originFacingRight)
    {
        health -= damage;
        StartCoroutine(Shake());

        if (health <= 0) OnDie();
    }
    public void OnDie()
    {
        HitStop.Instance.Stop(hitTime);
        StartCoroutine(DestroyWait());
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
        if (!shaking) shaking = true;

        yield return new WaitForSeconds(0.25f);

        shaking = false;
        sprite.transform.localPosition = originalPosition;
    }

    IEnumerator DestroyWait()
    {
        while (Time.timeScale != 1f)
            yield return null;
        Destroy(this.gameObject);
    }
}

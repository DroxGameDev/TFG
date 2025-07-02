using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial_Jump : MonoBehaviour
{
    TextMeshPro textMeshPro;

    void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        textMeshPro.enabled = false;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            textMeshPro.enabled = true;
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}

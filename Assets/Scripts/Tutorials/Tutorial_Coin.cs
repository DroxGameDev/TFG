using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial_Coin : MonoBehaviour
{
    TextMeshPro textMeshPro;

    void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        textMeshPro.enabled = false;
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            textMeshPro.enabled = true;
        }

        if (collision.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}

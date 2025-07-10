using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial_Move : MonoBehaviour
{
    TextMeshPro textMeshPro;
    public GameObject initialCanvas;

    void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        textMeshPro.enabled = false;
    }

    void Update()
    {
        if (!initialCanvas.activeSelf)
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

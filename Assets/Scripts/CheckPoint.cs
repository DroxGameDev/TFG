using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Transform RespawnPoint;
    public string Scene;
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameManager.Instance.UpdateCheckpoint(RespawnPoint, Scene);
    }
}

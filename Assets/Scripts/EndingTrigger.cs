using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndingTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Ending());
        }
    }

    IEnumerator Ending()
    {
        MenuManager.Instance.EndMenuTriggered();
        GameManager.Instance.player.GetComponent<PlayerInput>().actions.Disable();
        yield return new WaitForSecondsRealtime(5f);
        ViewManager.Instance.Restart();
    }
}

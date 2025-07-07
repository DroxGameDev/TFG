using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesChase : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform spikeCenter;
    [SerializeField] Transform destinyPosition;
    [SerializeField] Transform originPosition;
    [SerializeField] GameObject spikes;

    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;

    [SerializeField] float slowDownDistance;
    [SerializeField] float accelerateDistance;
    [SerializeField] Vector3 chaseDirection;
    [SerializeField] bool chasing;
    [SerializeField] bool moving;

    void Start()
    {
        player = GameManager.Instance.player;
    }

    void Update()
    {
        if (!moving) return;

        float distance;

        if (chasing)
        {
            if (Mathf.Abs(chaseDirection.x) > 0f)
                distance = player.position.x - spikeCenter.transform.position.x;
            else
                distance = player.position.y - spikeCenter.transform.position.y;


            //calcule velocity depending on the distance (more close = more slow)
            float factor = Mathf.Clamp01(Mathf.Abs(distance) / slowDownDistance);

            float currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, factor);

            if (distance > accelerateDistance) currentSpeed *= 2;

            spikes.transform.position += chaseDirection * currentSpeed * Time.deltaTime;

            //check if has arrive to destiny
            if (Mathf.Abs(chaseDirection.x) > 0f)
            {
                if (Mathf.Abs(spikes.transform.position.x - destinyPosition.position.x) <= 0.1f)
                    StopChase();
            }
            else
            {
                if (Mathf.Abs(spikes.transform.position.y - destinyPosition.position.y) <= 0.1f)
                    StopChase();
            }

        }

        else
        {
            spikes.transform.position += chaseDirection * -1 * maxSpeed * Time.deltaTime;

            //check if has arrive to destiny
            if (Mathf.Abs(chaseDirection.x) > 0f)
            {
                if (Mathf.Abs(spikes.transform.position.x - originPosition.position.x) <= 0.1f)
                    StopChase();
            }
            else
            {
                if (Mathf.Abs(spikes.transform.position.y - originPosition.position.y) <= 0.1f)
                    StopChase();
            }
        }

    }

    void StopChase()
    {
        moving = false;
        chasing = false;
    }

    public void BeginChase()
    {
        moving = true;
        chasing = true;
    }

    public void ChaseReturn()
    {
        moving = true;
        chasing = false;
    }
}

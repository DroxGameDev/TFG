using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PewterPower : MonoBehaviour
{
    private PlayerData playerData;
    private PlayerResources playerResources;
    private PlayerAttackInfo attackInfo;
    private bool input = false;
    private int noPewterDamage;
    private float noPewterKonckback;

    private GameObject objectPushing;

    void Start()
    {
        playerData = GetComponent<PlayerData>();
        playerResources = GetComponent<PlayerResources>();
        attackInfo = playerData.attackOrigin.GetComponent<PlayerAttackInfo>();

        noPewterDamage = playerData.damage;
        noPewterKonckback = playerData.damageKnockback;

        attackInfo.burningPewter = false;
        playerData.smearFramesMaterial.SetInt("_burningPewter", 0);
    }
    public void PewterInput()
    {
        if (input) //stop burning
        {
            StopPewter();
        }
        else //start burning
        {
            input = true;
            playerData.burningPewter = true;
            attackInfo.burningPewter = true;

            playerData.moveMod = playerData.pewterMovementModifier;

            if (playerData.pushing) playerData.moveMod = playerData.pewterPushMovementModifier;

            playerData.jumpMod = playerData.pewterJumpModifier;
            playerData.damage = playerData.pewterDamage;
            playerData.damageKnockback = playerData.pewterDamageKnokback;

            playerData.smearFramesMaterial.SetInt("_burningPewter", 1);
            StartCoroutine(Healing());
        }
    }


    public void PushImputUpdate()
    {
        if (playerData.pushing) //stop pushing
        {
            StopPushing();
        }
        else //start pushing
        {
            playerData.pushing = true;
            playerData.moveMod = playerData.pewterPushMovementModifier;
            objectPushing = playerData.boxToPush;
            objectPushing.transform.SetParent(transform, true);
            objectPushing.GetComponent<Metal_Heavy_Object>().ForceMove();
            StartCoroutine(Pushing());
        }
    }
    IEnumerator Pushing()
    {
        Rigidbody2D rb = objectPushing.GetComponent<Rigidbody2D>();
        while (playerData.pushing && objectPushing != null)
        {
            rb.velocity = new Vector2(playerData.velocity.x, rb.velocity.y);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Healing()
    {
        while (playerData.burningPewter)
        {
            if (playerData.health < playerData.maxHealth)
            {
                playerData.health++;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    void Update()
    {
        if (playerData.burningPewter && (playerResources.pewterEmpty || playerData.dead))
        {
            StopPewter();
        }

        if (playerData.pushing && Mathf.Abs(objectPushing.GetComponent<Rigidbody2D>().velocity.y) > 0.001f)
        {
            playerData.pushing = false;
            playerData.moveMod = playerData.pewterMovementModifier;
            objectPushing.transform.SetParent(null);
            objectPushing.GetComponent<Metal_Heavy_Object>().Stop();
            objectPushing = null;
        }

    }

    void StopPewter()
    {
        if (playerData.pushing) //stop pushing
        {
            StopPushing();
        }

        input = false;
        playerData.burningPewter = false;
        attackInfo.burningPewter = false;
        playerData.moveMod = 1;
        playerData.jumpMod = 1;
        playerData.damage = noPewterDamage;
        playerData.damageKnockback = noPewterKonckback;
        playerData.smearFramesMaterial.SetInt("_burningPewter", 0);
    }

    public void StopPushing()
    {
        playerData.pushing = false;
        playerData.moveMod = playerData.pewterMovementModifier;
        objectPushing.transform.SetParent(null);
        objectPushing.GetComponent<Metal_Heavy_Object>().Stop();
        objectPushing = null;
    }

}

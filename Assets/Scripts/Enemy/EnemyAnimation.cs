using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private EnemyData enemyData;
    [Range(0f, 15)] public int animationDebugIndex = 15;
    public string[] animationName;

    private static readonly int idle = Animator.StringToHash("Idle");
    private static readonly int run = Animator.StringToHash("Run");
    private static readonly int walk = Animator.StringToHash("Walk");
    private static readonly int damageLight = Animator.StringToHash("DamageLight");
    private static readonly int damageMedium = Animator.StringToHash("DamageMedium");
    private static readonly int damageHard = Animator.StringToHash("DamageHard");



    [Header("States")]
    private float _lockedTill;
    private int currentState = 0;

    void Start()
    {
        enemyData = GetComponent<EnemyData>();
    }

    void Update()
    {
        if (animationDebugIndex < animationName.Length)
            enemyData.anim.CrossFade(animationName[animationDebugIndex], 0, 0);
        else
        {
            var state = GetState();

            if (state == currentState) return;
            enemyData.anim.CrossFade(state, 0, 0);
            currentState = state;
        }
    }

    private int GetState()
    {
        if (Time.time < _lockedTill) return currentState;

        if (enemyData.damaged)
        {
            switch (enemyData.damageType)
            {
                case DamageType.light:
                    return LockState(damageLight, enemyData.damageWait);
                case DamageType.medium:
                    return LockState(damageMedium, enemyData.damageWait);
                case DamageType.hard:
                    return LockState(damageHard, enemyData.damageWait);
            }
        }

        if (enemyData.walking) return walk;
        if (enemyData.running) return run;

        return idle;

        int LockState(int s, float t){
            _lockedTill = Time.deltaTime+t;
            return s;
        }
    }
}

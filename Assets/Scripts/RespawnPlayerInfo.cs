using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "ScriptableObjects/RespawnPlayerInfo", fileName = "RespawnPlayerInfo")]

public class RespawnPlayerInfo : ScriptableObject
{
    public int health = 10;
    public int coins = 0;
    public int ironVials = 0;
    public int steelVials  = 0;
    public int tinVials= 0;
    public int pewterVials= 0;
    
    public float ironReserve= 0;
    public float steelReserve= 0;
    public float tinReserve= 0;
    public float pewterReserve= 0;
}

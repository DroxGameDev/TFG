using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum VialType
{
    iron,
    steel,
    tin,
    pewter
}
public class Vial : AffectedByGravity
{
    public VialType type;
}

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

    [Space(10)]


    [Header("Metal Indicator")]
    public SpriteRenderer metalSymbolSpriteRenderer;
    public Sprite metalSymbolSpriteIron;
    public Sprite metalSymbolSpriteSteel;
    public Sprite metalSymbolSpriteTin;
    public Sprite metalSymbolSpritePewter;
    

    void Awake() {
    
        switch (type) {
            case VialType.iron:
                metalSymbolSpriteRenderer.sprite = metalSymbolSpriteIron;
                break;
            case VialType.steel:
                metalSymbolSpriteRenderer.sprite = metalSymbolSpriteSteel;
                break;
            case VialType.tin:
                metalSymbolSpriteRenderer.sprite = metalSymbolSpriteTin;
                break;
            case VialType.pewter:
                metalSymbolSpriteRenderer.sprite = metalSymbolSpritePewter;
                break;
        }

    }
}



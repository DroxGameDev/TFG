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

    [Header("Metal Vial")]
    public SpriteRenderer metalVialSpriteRenderer;
    public Sprite metalVialSpriteIron;
    public Sprite metalVialSpriteSteel;
    public Sprite metalVialSpriteTin;
    public Sprite metalVialSpritePewter;
    
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
                metalVialSpriteRenderer.sprite = metalVialSpriteIron;
                metalSymbolSpriteRenderer.sprite = metalSymbolSpriteIron;
                break;
            case VialType.steel:
                metalVialSpriteRenderer.sprite = metalVialSpriteSteel;
                metalSymbolSpriteRenderer.sprite = metalSymbolSpriteSteel;
                break;
            case VialType.tin:
                metalVialSpriteRenderer.sprite = metalVialSpriteTin;
                metalSymbolSpriteRenderer.sprite = metalSymbolSpriteTin;
                break;
            case VialType.pewter:
                metalVialSpriteRenderer.sprite = metalVialSpritePewter;
                metalSymbolSpriteRenderer.sprite = metalSymbolSpritePewter;
                break;
        }

    }
}



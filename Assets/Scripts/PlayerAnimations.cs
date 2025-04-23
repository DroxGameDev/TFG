using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

    private Animator _anim;   
    private Material _mat;
    public string[] animationName;
    public Texture2D[] normalMaps;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        //Renderer renderer = GetComponent<Renderer>();
        //_mat = renderer.material;

        
        /*
        _mat.EnableKeyword("_NORMALMAP");
        _mat.SetTexture("_NormalMap", normalMaps[1]);
        */
    }

    // Update is called once per frame
    void Update()
    {
        _anim.CrossFade(animationName[1], 0, 0);
    }
}

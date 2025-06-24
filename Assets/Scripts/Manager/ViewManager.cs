using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewManager : MonoBehaviour
{
    // Start is called before the first frame update

    public bool active = false;
    void Start()
    {   
        if(active)
        SceneManager.LoadScene("Test_Envioriment",LoadSceneMode.Additive);  
    }
}

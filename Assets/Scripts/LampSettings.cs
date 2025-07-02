using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


#if UNITY_EDITOR
[CustomEditor(typeof(LampPieces))]
public class LampSettings : Editor
{
    public override void OnInspectorGUI()
    {
        LampPieces lampPieces = (LampPieces)target;
        lampPieces.lampStick = (GameObject)EditorGUILayout.ObjectField("Lamp Stick", lampPieces.lampStick, typeof(GameObject), true);
        lampPieces.lampChain = (GameObject)EditorGUILayout.ObjectField("Lamp Chain", lampPieces.lampChain, typeof(GameObject), true);
        lampPieces.lampHook = (GameObject)EditorGUILayout.ObjectField("Lamp Hook", lampPieces.lampHook, typeof(GameObject), true);
        GUILayout.Space(10);

        if (GUILayout.Button("Stick"))
        {
            if (!lampPieces.lampStick.activeSelf)
            {
                lampPieces.lampStick.SetActive(true);
            }
            else
            {
                lampPieces.lampStick.SetActive(false);
            }
        }

        if (GUILayout.Button("Chain"))
        {
            if (!lampPieces.lampChain.activeSelf)
            {
                lampPieces.lampChain.SetActive(true);
            }
            else
            {
                lampPieces.lampChain.SetActive(false);
            }
        }
        if (GUILayout.Button("Hook"))
        {
            if (!lampPieces.lampHook.activeSelf)
            {
                lampPieces.lampHook.SetActive(true);
            }
            else
            {
                lampPieces.lampHook.SetActive(false);
            }
        }

        if (GUILayout.Button("None"))
        {
            lampPieces.lampStick.SetActive(false);
            lampPieces.lampChain.SetActive(false);
            lampPieces.lampHook.SetActive(false);
        }
    }
}
#endif


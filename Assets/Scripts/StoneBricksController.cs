using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StoneBricksController : MonoBehaviour
{
    public StoneBricks stoneBricks;
}

[System.Serializable]

public class StoneBricks
{
    public SpriteRenderer spriteRenderer;

    public List<Sprite> sprites = new List<Sprite>();

    [Range(0, 9)] public int index;
}

#if UNITY_EDITOR
[CustomEditor(typeof(StoneBricksController))]

public class StoneBricksEditor : Editor
{
    StoneBricksController stoneBricksController;

    private void OnEnable()
    {
        stoneBricksController = (StoneBricksController)target;
    }

    public override void OnInspectorGUI()
    {
        // Dibuja el Inspector por defecto (para editar los campos públicos)
        DrawDefaultInspector();

        // Seguridad básica
        var brick = stoneBricksController.stoneBricks;
        if (brick == null || brick.spriteRenderer == null || brick.sprites == null || brick.sprites.Count == 0)
            return;

        // Clamp del índice (por si la lista cambia)
        brick.index = Mathf.Clamp(brick.index, 0, brick.sprites.Count - 1);

        // Actualizar el sprite según el índice
        Sprite selectedSprite = brick.sprites[brick.index];
        if (brick.spriteRenderer.sprite != selectedSprite)
        {
            brick.spriteRenderer.sprite = selectedSprite;

            // Marcar escena y objetos como modificados para guardar los cambios
            EditorUtility.SetDirty(brick.spriteRenderer);
            EditorUtility.SetDirty(stoneBricksController);
        }
    }
}

#endif
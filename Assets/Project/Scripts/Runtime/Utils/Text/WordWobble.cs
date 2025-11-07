using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordWobble : MonoBehaviour
{
    public bool enable;
    public float speed = 1;
    public float verticalOffset = 1;
    TMP_Text textComponent;

    private void Start()
    {
        textComponent = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (!enable)
        {
            return;
        }
        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
            {
                continue;
            }

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                var orig = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orig + new Vector3(0, Mathf.Sin(Time.time * speed + orig.x * 0.01f) * verticalOffset, 0);

            }
        }
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textComponent.UpdateGeometry(meshInfo.mesh, i);
        }
    }



}

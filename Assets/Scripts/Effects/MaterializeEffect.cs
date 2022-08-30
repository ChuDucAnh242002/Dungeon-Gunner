using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterializeEffect : MonoBehaviour
{
    public IEnumerator MaterializeRoutine(Shader materializeShader, Color materializeColor, float materializeTime,
        SpriteRenderer[] spriteRendererArray, Material normalMaterial){
        Material materializeMaterial = new Material(materializeShader);

        materializeMaterial.SetColor("_EmissionColor", materializeColor);

        foreach (SpriteRenderer spriteRenderer in spriteRendererArray){
            spriteRenderer.material = materializeMaterial;
        }

        float dissolveAmount = 0f;

        while (dissolveAmount < 1f){
            dissolveAmount += Time.deltaTime / materializeTime;

            materializeMaterial.SetFloat("_DissolveAmount", dissolveAmount);

            yield return null;
        }

        foreach (SpriteRenderer spriteRenderer in spriteRendererArray){
            spriteRenderer.material = normalMaterial;
        }
    }
}

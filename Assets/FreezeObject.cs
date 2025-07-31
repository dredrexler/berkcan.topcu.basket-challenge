using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FreezeObject : MonoBehaviour
{
    public string frozenShaderName = "Custom/IceShader";
    public float freezeLerpTime = 1.0f;
    public float freezeAmount = 1.0f;

    private Renderer objectRenderer;
    private bool isFrozen = false;
    private List<Shader> originalShaders = new List<Shader>();
    private List<Material> originalMaterials = new List<Material>();

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>() ? GetComponent<Renderer>() : GetComponentInChildren<Renderer>();
        if (objectRenderer == null) return;

        // Save original shaders and materials for unfreeze
        foreach (Material mat in objectRenderer.materials)
        {
            originalShaders.Add(mat.shader);
            originalMaterials.Add(new Material(mat)); // Deep copy
        }
    }

    // Call this to set frozen/unfrozen state
    public void SetFrozen(bool frozen)
    {
        if (isFrozen == frozen) return; // No change

        if (frozen)
        {
            AssignFrozenShader();
            StartCoroutine(FreezeOverTime(freezeLerpTime));
        }
        else
        {
            StopAllCoroutines();
            RestoreOriginalMaterial();
        }
        isFrozen = frozen;
    }

    private void AssignFrozenShader()
    {
        Shader iceShader = Shader.Find(frozenShaderName);
        for (int i = 0; i < objectRenderer.materials.Length; i++)
        {
            objectRenderer.materials[i].shader = iceShader;
        }
    }

    private void RestoreOriginalMaterial()
    {
        for (int i = 0; i < objectRenderer.materials.Length && i < originalMaterials.Count; i++)
        {
            // Restore entire material properties, not just shader
            objectRenderer.materials[i].CopyPropertiesFromMaterial(originalMaterials[i]);
            objectRenderer.materials[i].shader = originalShaders[i];
        }
    }

    private IEnumerator FreezeOverTime(float time)
    {
        float elapsed = 0;
        float currentFreezeAmount = objectRenderer.material.GetFloat("_FrozenAmount");

        List<float> initialRefraction = new List<float>();
        List<float> initialMetallic = new List<float>();
        List<float> initialTranslucency = new List<float>();
        List<Color> initialSubsurfaceColor = new List<Color>();

        foreach (Material material in objectRenderer.materials)
        {
            initialRefraction.Add(material.GetFloat("_Refraction"));
            initialMetallic.Add(material.GetFloat("_Metallic"));
            initialTranslucency.Add(material.GetFloat("_Translucency"));
            initialSubsurfaceColor.Add(material.GetColor("_SubsurfaceColor"));
        }

        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / time;

            for (int i = 0; i < objectRenderer.materials.Length; i++)
            {
                Material material = objectRenderer.materials[i];
                float newRefraction = Mathf.Lerp(initialRefraction[i], 0.155f, t);
                float newMetallic = Mathf.Lerp(initialMetallic[i], 0.395f, t);
                float newTranslucency = Mathf.Lerp(initialTranslucency[i], 0.648f, t);
                Color newSubsurfaceColor = Color.Lerp(initialSubsurfaceColor[i], new Color(136 / 255f, 219 / 255f, 1f), t);

                material.SetFloat("_FrozenAmount", Mathf.Lerp(currentFreezeAmount, freezeAmount, t));
                material.SetFloat("_Refraction", newRefraction);
                material.SetFloat("_Metallic", newMetallic);
                material.SetFloat("_Translucency", newTranslucency);
                material.SetColor("_SubsurfaceColor", newSubsurfaceColor);
            }

            yield return null;
        }
        // Set final values
        for (int i = 0; i < objectRenderer.materials.Length; i++)
        {
            Material material = objectRenderer.materials[i];
            material.SetFloat("_FrozenAmount", freezeAmount);
            material.SetFloat("_Refraction", 0.155f);
            material.SetFloat("_Metallic", 0.395f);
            material.SetFloat("_Translucency", 0.648f);
            material.SetColor("_SubsurfaceColor", new Color(136 / 255f, 219 / 255f, 1f));
        }
    }
}

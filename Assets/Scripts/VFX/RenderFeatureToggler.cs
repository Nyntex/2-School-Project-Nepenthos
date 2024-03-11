using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RenderFeatureToggler : MonoBehaviour
{
    [SerializeField]
    private List<ScriptableRendererFeature> optionalRenderFeatures = new List<ScriptableRendererFeature>();

    private void Awake()
    {
        DeactivateOptionalRenderFeatures();
    }

    public void DeactivateOptionalRenderFeatures()
    {
        foreach (var feature in optionalRenderFeatures)
        {
            feature.SetActive(false);
        }
    }
}
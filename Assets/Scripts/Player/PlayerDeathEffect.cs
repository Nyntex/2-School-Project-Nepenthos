using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PlayerDeathEffect : MonoBehaviour
{
    [SerializeField]
    private float effectStartingDistance;
    [SerializeField]
    private ScriptableRendererFeature deathEffect;
    [SerializeField]
    private Material deathMaterial;
    [SerializeField]
    private VolumeProfile globalVolumeProfile;
    ChromaticAberration chromAbb;
    //ColorAdjustments colorAdjust;

    float intensity = 0f;
    float lerpVal = 0f;

    bool fading = false;

    public bool activeOnStart = false;

    // check with Morten for detail on these values
    public float vignetteStart = 0f;
    public float vignetteEnd = 2.5f;
    public float scanLinesStart = 1;
    public float scanLinesEnd = 0.88f;
    public float glitchStart = 0;
    public float glitchEnd = 2.5f;
    public float chromAbbStart = 0f;
    public float chromAbbEnd = 1f;
    //public float colorAdjustStart = 0f;
    //public float colorAdjustEnd = -100f;


    void Start()
    {
        deathEffect.SetActive(activeOnStart);

        if (!globalVolumeProfile.TryGet(out chromAbb)) throw new System.NullReferenceException(nameof(chromAbb));
        //if (!globalVolumeProfile.TryGet(out colorAdjust)) throw new System.NullReferenceException(nameof(colorAdjust));
    }
    private void Update()
    {
        if(fading)
        {
            intensity = Mathf.Lerp(intensity, 0f, lerpVal);
            lerpVal -= 0.05f * Time.deltaTime;
            if(lerpVal<=0f)
            {
                deathEffect.SetActive(false);
                lerpVal = 0f;
                fading = false;
            }
        }
    }

    public void UpdateIntensityByDistance(float distance)
    {

        if (distance > effectStartingDistance) 
        { 
            Stop();
            return;
        }
        intensity = Mathf.Lerp(intensity, (effectStartingDistance - distance) / effectStartingDistance, lerpVal);
        lerpVal += 0.05f * Time.deltaTime;
        lerpVal = Mathf.Clamp(lerpVal, 0f, 1f);
        //Debug.LogWarning("DISTANCE: " + distance + " INTENSITY: "+intensity);
        if (!deathEffect.isActive) deathEffect.SetActive(true);

        deathMaterial.SetFloat("_VignetteScale", vignetteEnd*intensity);
        deathMaterial.SetFloat("_ScanLinesStrength", scanLinesEnd + ((scanLinesStart-scanLinesEnd)*intensity));
        deathMaterial.SetFloat("_GlitchStrength", glitchEnd*intensity);

        chromAbb.intensity.Override(chromAbbEnd*intensity);
        //colorAdjust.saturation.Override(colorAdjustEnd*intensity);
    }

    public void UpdateIntensityByHealth(float currentHealth, float maxHealth)
    {
        fading = (currentHealth == maxHealth);
        if (fading)
        {
            chromAbb.intensity.Override(chromAbbStart);
            return;
        }
        intensity = Mathf.Lerp(intensity, (maxHealth - currentHealth) / maxHealth, lerpVal);
        lerpVal += 0.05f * Time.deltaTime;
        lerpVal = Mathf.Clamp(lerpVal, 0f, 1f);
        if (!deathEffect.isActive) deathEffect.SetActive(true);

        deathMaterial.SetFloat("_VignetteScale", vignetteEnd * intensity);
        deathMaterial.SetFloat("_ScanLinesStrength", scanLinesEnd + ((scanLinesStart - scanLinesEnd) * intensity));
        deathMaterial.SetFloat("_GlitchStrength", glitchEnd * intensity);

        chromAbb.intensity.Override(chromAbbEnd * intensity);
        //colorAdjust.saturation.Override(colorAdjustEnd * intensity);
    }

    private void OnApplicationQuit()
    {
        deathEffect.SetActive(false);
    }

    public void Stop()
    {
        fading = true;
    }
}
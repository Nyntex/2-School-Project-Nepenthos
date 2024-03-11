using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GammaSlider : MonoBehaviour, IConfigSaveable
{
    public void ChangeValue()
    {
        GammaSetting.Instance.ChangeGammaTo(GetComponent<Slider>().value);
    }

    private void OnEnable()
    {
        GammaSetting.Instance.GetComponent<Volume>().sharedProfile.TryGet<LiftGammaGain>(out var g);
        GetComponent<Slider>().value = g.gamma.value.w;
    }

    public void LoadData(ConfigData data)
    {
        GetComponent<Slider>().value = data.gammaData.value;
    }

    public void SaveData(ref ConfigData data)
    {
        return;
    }
}

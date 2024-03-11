using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class GammaSetting : MonoBehaviour, IConfigSaveable
{
    private static GammaSetting instance;
    public static GammaSetting Instance { get { return instance; } }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("There is already a GammaSlider");
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(instance);
    }

    public void ChangeGammaTo(float value)
    {
        Instance.GetComponent<Volume>().sharedProfile.TryGet<LiftGammaGain>(out var liftGammaGain);
        liftGammaGain.gamma.value = new Vector4(0,0,0,value);
    }

    public void LoadData(ConfigData data)
    {
        ChangeGammaTo(data.gammaData.value);
    }

    public void SaveData(ref ConfigData data)
    {
        Instance.GetComponent<Volume>().sharedProfile.TryGet<LiftGammaGain>(out var liftGammaGain);
        data.gammaData.value = liftGammaGain.gamma.value.w;
    }

//If the Gamma doesn't get saved correctly when the game is exited it's probably because of this, therefore should work in Build
#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        //ChangeGammaTo(0);
    }
#endif
}

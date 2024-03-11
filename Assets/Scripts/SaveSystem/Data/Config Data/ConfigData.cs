[System.Serializable]
public class ConfigData
{
    public AudioData audioData;
    public GammaData gammaData;
    public float mouseSensitivity;
    public int lastSave;
    public bool isFullscreen;

    public ConfigData() 
    { 
        audioData = new AudioData();
        gammaData = new GammaData();
        mouseSensitivity = 5f;
        lastSave = 0;
        isFullscreen = true;
    }
}

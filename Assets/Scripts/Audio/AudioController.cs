using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using static UnityEngine.ParticleSystem;

public class AudioController : MonoBehaviour, IConfigSaveable
{
    [SerializeField]
    private bool audioOcclusionOn = true;
    [SerializeField]
    private float occlusionSpreadPlayer = 1.5f;
    [SerializeField]
    private float occlusionSpreadSoundSource = 1.5f;
    [SerializeField]
    private LayerMask occlusionLayer;
    private int occlusionHitCount = 0;
    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 0.2f;
    [Range(0, 1)]
    public float musicVolume = 0.2f;
    [Range(0, 1)]
    public float ambienceVolume = 0.2f;
    [Range(0, 1)]
    public float SFXVolume = 0.2f;

    [field: Header("Music")]
    [field: SerializeField] public EventReference music_ref { get; private set; }
    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience_ref { get; private set; }
    [field: SerializeField] public EventReference rain_ref { get; private set; }
    [field: Header("UI")]
    [field: SerializeField] public EventReference buttonClick_ref { get; private set; }
    [field: SerializeField] public EventReference buttonHover_ref { get; private set; }
    //[field: SerializeField] public EventReference sliderMoving_ref { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootsteps_ref { get; private set; }
    [field: SerializeField] public EventReference playerDeath_ref { get; private set; }
    [field: SerializeField] public EventReference playerCrouch_ref { get; private set; }
    [field: SerializeField] public EventReference distractionPickup_ref { get; private set; }
    [field: Header("Enemy SFX")]
    [field: SerializeField] public EventReference enemyAlert_ref { get; private set; }
    [field: SerializeField] public EventReference enemyNoise_ref { get; private set; }
    [field: SerializeField] public EventReference enemyDeath_ref { get; private set; }
    [field: Header("other SFX")]
    [field: SerializeField] public EventReference distractionEmission_ref { get; private set; }
    [field: SerializeField] public EventReference distractionImpact_ref { get; private set; }
    [field: SerializeField] public EventReference exitOpen_ref { get; private set; }
    [field: SerializeField] public EventReference exitClosed_ref { get; private set; }
    [field: SerializeField] public EventReference gameSaved_ref { get; private set; }
    [field: SerializeField] public EventReference takedown_ref { get; private set; }

    public static AudioController Instance { get; private set; }
    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;
    private List<EventInstance> playerEventInstances;
    private StudioListener listener;

    private Bus masterBus;
    private Bus musicBus;
    private Bus ambienceBus;
    private Bus SFXBus;

    private EventInstance ambience;
    private EventInstance rain;
    private EventInstance music;
    public EventInstance playerFootsteps { get; private set; }
    public EventInstance playerDeath { get; private set; }
    public EventInstance playerCrouch { get; private set; }
    public EventInstance distractionPickup { get; private set; }
    public EventInstance enemyNoise { get; private set; }
    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("Only 1 Instance of AudioController is allowed!");
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);

        Instance.eventInstances = new List<EventInstance>();
        Instance.eventEmitters = new List<StudioEventEmitter>();
        Instance.playerEventInstances = new List<EventInstance>();

        Instance.masterBus = RuntimeManager.GetBus("bus:/");
        Instance.musicBus = RuntimeManager.GetBus("bus:/Music");
        Instance.ambienceBus = RuntimeManager.GetBus("bus:/Ambient");
        Instance.SFXBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Start()
    {
        Instance.masterBus.setVolume(masterVolume);
        Instance.musicBus.setVolume(musicVolume);
        Instance.ambienceBus.setVolume(ambienceVolume);
        Instance.SFXBus.setVolume(SFXVolume);

        Instance.music = RuntimeManager.CreateInstance(music_ref);
        Instance.music.start();
        ambience = RuntimeManager.CreateInstance(ambience_ref);
        //ambience.start();
        //rain = Instance.CreateEventInstance(rain_ref);
    }

    private void FixedUpdate()
    {
        if (!Instance.audioOcclusionOn) return;
        if (Instance.listener == null) Instance.listener = FindObjectOfType<StudioListener>();
        foreach (EventInstance eventInstance in eventInstances)
        {
            Instance.CheckOcclusion(eventInstance);
        }
        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            Instance.CheckOcclusion(emitter);
        }
    }

#region Occlusion
    private void CheckOcclusion(EventInstance eventInstance)
    {
        if (Instance.listener == null) return;
        PLAYBACK_STATE pbs;
        eventInstance.getPlaybackState(out pbs);
        FMOD.ATTRIBUTES_3D attr;
        eventInstance.get3DAttributes(out attr);
        Vector3 eventPosition = new Vector3(attr.position.x, attr.position.y, attr.position.z);
        float listenerDistance = Vector3.Distance(eventPosition, Instance.listener.transform.position);
        EventDescription description;
        eventInstance.getDescription(out description);
        float minDistance, maxDistance;
        description.getMinMaxDistance(out minDistance, out maxDistance);
        if (pbs == PLAYBACK_STATE.PLAYING)
        {
            if(listenerDistance > maxDistance)
            {
                Instance.occlusionHitCount = 15;
            }
            else
            {
                Instance.OccludeBetween(eventPosition, Instance.listener.transform.position);
            }
        }
        eventInstance.setParameterByName("Occlusion", Instance.occlusionHitCount / 1.5f);
        Instance.occlusionHitCount = 0;
    }
    private void CheckOcclusion(StudioEventEmitter emitter)
    {
        if (emitter == null) return;
        Vector3 eventPosition = emitter.gameObject.transform.position;
        float listenerDistance = Vector3.Distance(eventPosition, Instance.listener.transform.position);
        float minDistance, maxDistance;
        emitter.EventInstance.getMinMaxDistance(out minDistance, out maxDistance);
        if (emitter.IsPlaying() && listenerDistance <= maxDistance)
        {
            Instance.OccludeBetween(eventPosition, Instance.listener.transform.position);
        }
        emitter.EventInstance.setParameterByName("Occlusion", Instance.occlusionHitCount / 1.5f);
        Instance.occlusionHitCount = 0;
    }
    private void OccludeBetween(Vector3 sound, Vector3 listener)
    {
        Vector3 soundLeft = FindPoint(sound, listener, occlusionSpreadSoundSource, true);
        Vector3 soundRight = FindPoint(sound, listener, occlusionSpreadSoundSource, false);
        Vector3 soundTop = new Vector3(sound.x, sound.y + occlusionSpreadSoundSource, sound.z);
        Vector3 soundBottom = new Vector3(sound.x, sound.y - occlusionSpreadSoundSource, sound.z);

        Vector3 listenerLeft = FindPoint(listener, sound, occlusionSpreadPlayer, true);
        Vector3 listenerRight = FindPoint(listener, sound, occlusionSpreadPlayer, false);

        Vector3 listenerTop = new Vector3(listener.x, listener.y + occlusionSpreadPlayer * 0.5f, listener.z);
        Vector3 listenerBottom = new Vector3(listener.x, listener.y - occlusionSpreadPlayer * 0.5f, listener.z);

        CastLine(soundLeft, listenerLeft);
        CastLine(soundLeft, listener);
        CastLine(soundLeft, listenerRight);

        CastLine(sound, listenerLeft);
        CastLine(sound, listener);
        CastLine(sound, listenerRight);

        CastLine(soundRight, listenerLeft);
        CastLine(soundRight, listener);
        CastLine(soundRight, listenerRight);

        CastLine(soundTop, listenerTop);
        CastLine(soundTop, listener);
        CastLine(soundTop, listenerBottom);

        CastLine(soundBottom, listenerBottom);
        CastLine(soundBottom, listener);
        CastLine(soundBottom, listenerTop);
    }

    private Vector3 FindPoint(Vector3 castSource, Vector3 castTarget, float castSpread, bool goLeft)
    {
        float x;
        float z;
        float n = Vector3.Distance(new Vector3(castSource.x, 0f, castSource.z), new Vector3(castTarget.x, 0f, castTarget.z));
        float mn = (castSpread / n);
        if (goLeft)
        {
            x = castSource.x + (mn * (castSource.z - castTarget.z));
            z = castSource.z - (mn * (castSource.x - castTarget.x));
        }
        else
        {
            x = castSource.x - (mn * (castSource.z - castTarget.z));
            z = castSource.z + (mn * (castSource.x - castTarget.x));
        }
        return new Vector3(x, castSource.y, z);
    }

    private void CastLine(Vector3 castSource, Vector3 castTarget)
    {
        RaycastHit hit;
        Physics.Linecast(castSource, castTarget, out hit, Instance.occlusionLayer);
        if (hit.collider)
        {
            Instance.occlusionHitCount++;
            UnityEngine.Debug.DrawLine(castSource, castTarget, Color.red);
        }
        else
        {
            UnityEngine.Debug.DrawLine(castSource, castTarget, Color.blue);
        }
    }
#endregion Occlusion

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference reference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(reference);
        Instance.eventInstances.Add(eventInstance);
        return eventInstance;
    }
    public void ClearEventInstance(EventInstance eventInstance)
    {
        RuntimeManager.DetachInstanceFromGameObject(eventInstance);
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
        Instance.eventInstances.Remove(eventInstance);
    }
    public StudioEventEmitter InitializeEventEmitter(EventReference reference, GameObject emitterObject)
    {
        StudioEventEmitter emitter = emitterObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = reference;
        Instance.eventEmitters.Add(emitter);
        return emitter;
    }

    public void CreatePlayerSounds(Transform player)
    {
        Instance.playerFootsteps = Instance.CreatePlayerEventInstance(playerFootsteps_ref);
        RuntimeManager.AttachInstanceToGameObject(playerFootsteps, player);
        Instance.playerDeath = Instance.CreatePlayerEventInstance(playerDeath_ref);
        RuntimeManager.AttachInstanceToGameObject(playerDeath, player);
        Instance.playerCrouch = Instance.CreatePlayerEventInstance(playerCrouch_ref);
        RuntimeManager.AttachInstanceToGameObject(playerCrouch, player);
        Instance.distractionPickup = Instance.CreatePlayerEventInstance(distractionPickup_ref);
    }
    public EventInstance CreatePlayerEventInstance(EventReference reference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(reference);
        Instance.playerEventInstances.Add(eventInstance);
        return eventInstance;
    }

    public void Pause(bool val)
    {
        foreach (EventInstance eventInstance in Instance.eventInstances)
        {
            eventInstance.setPaused(val);
        }
        foreach (StudioEventEmitter emitter in Instance.eventEmitters)
        {
            if (val)
            {
                emitter.Stop();
            }
            else
            {
                emitter.Play();
            }
        }
        Instance.playerFootsteps.setPaused(val);
    }

    public void CleanUp()
    {
        foreach(EventInstance eventInstance in Instance.eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        foreach(StudioEventEmitter emitter in Instance.eventEmitters)
        {
            emitter.Stop();
        }
        foreach(EventInstance eventInstance in Instance.playerEventInstances)
        Instance.eventInstances.Clear();
        Instance.eventEmitters.Clear();
        Instance.playerEventInstances.Clear();
    }

    public void SetAmbienceParameter(string parameterName, float value)
    {
        //Instance.rain.setParameterByName(parameterName, value);
    }

    public void SetMusicArea(MusicArea area)
    {
        Instance.music.setParameterByName("BackgroundMusic", (float)area);
    }
    public void SetSoundArea(SoundAreaType type, float value)
    {
        switch (type)
        {
            case SoundAreaType.PLAYER:
                Instance.playerFootsteps.setParameterByName("PlayerMovingState", value);
                break;

        }
    }

    public void UpdateVolume()
    {
        Instance.masterBus.setVolume(Instance.masterVolume);
        Instance.musicBus.setVolume(Instance.musicVolume);
        Instance.ambienceBus.setVolume(Instance.ambienceVolume);
        Instance.SFXBus.setVolume(Instance.SFXVolume);
    }

    public void LoadData(ConfigData data)
    {
        //Debug.Log("Sound Data imported: " + data.audioData.masterVolume + " || " + data.audioData.effectVolume + " || " + data.audioData.musicVolume + " || " + data.audioData.ambienceVolume);
        Instance.masterVolume = data.audioData.masterVolume;
        Instance.SFXVolume = data.audioData.effectVolume;
        Instance.musicVolume = data.audioData.musicVolume;
        Instance.ambienceVolume = data.audioData.ambienceVolume;
        Instance.UpdateVolume();
        //Debug.Log("Audio Loaded");
    }

    public void SaveData(ref ConfigData data)
    {
        Debug.Log("Sound Data Saved: " + masterVolume + " || " + SFXVolume + " || " + musicVolume + " || " + ambienceVolume);
        data.audioData = new AudioData(Instance.masterVolume, Instance.musicVolume, Instance.SFXVolume, Instance.ambienceVolume);
        Debug.Log("Saved Audio Settings");
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private sealed class EffectVoice
    {
        public AudioSource source;
        public AudioCue cue;
        public float baseVolume;
    }

    private const string MasterKey = "Audio.MasterVolume";
    private const string MusicKey = "Audio.MusicVolume";
    private const string EffectsKey = "Audio.EffectsVolume";
    private const string MuteKey = "Audio.Muted";

    private static AudioManager instance;
    private readonly List<EffectVoice> voices = new List<EffectVoice>();
    private readonly HashSet<Button> wiredButtons = new HashSet<Button>();
    private AudioCatalog catalog;
    private readonly AudioSource[] musicSources = new AudioSource[2];
    private readonly float[] musicLevels = new float[2];
    private readonly float[] fadeStartLevels = new float[2];
    private readonly float[] fadeTargetLevels = new float[2];
    private AudioClip requestedMusic;
    private float fadeElapsed;
    private bool musicIsFading;
    private float masterVolume;
    private float musicVolume;
    private float effectsVolume;
    private bool muted;

    public static AudioManager Instance => instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateManager()
    {
        if (instance == null)
            new GameObject("AudioManager").AddComponent<AudioManager>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        catalog = Resources.Load<AudioCatalog>("AudioCatalog");
        if (catalog == null)
            Debug.LogWarning("AudioCatalog is missing from Assets/Resources; sound cues will be silent.");

        masterVolume = PlayerPrefs.GetFloat(MasterKey, 1f);
        musicVolume = PlayerPrefs.GetFloat(MusicKey, 1f);
        effectsVolume = PlayerPrefs.GetFloat(EffectsKey, 0.8f);
        muted = PlayerPrefs.GetInt(MuteKey, 0) != 0;

        for (int i = 0; i < musicSources.Length; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            source.spatialBlend = 0f;
            musicSources[i] = source;
        }

        int count = Mathf.Max(1, catalog != null ? catalog.effectVoiceCount : 16);
        for (int i = 0; i < count; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            voices.Add(new EffectVoice { source = source });
        }

        RefreshVolumes();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        // Also covers projects that create this component in an already loaded scene.
        SetupScene(SceneManager.GetActiveScene());
    }

    private void Update()
    {
        if (!musicIsFading) return;
        AdvanceMusicFade(Time.unscaledDeltaTime);
    }

    private void AdvanceMusicFade(float deltaTime)
    {
        fadeElapsed += deltaTime;
        float duration = catalog != null ? catalog.musicFadeDuration : 0f;
        float progress = duration <= 0f ? 1f : Mathf.Clamp01(fadeElapsed / duration);
        for (int i = 0; i < musicSources.Length; i++)
        {
            musicLevels[i] = Mathf.Lerp(fadeStartLevels[i], fadeTargetLevels[i], progress);
            if (progress >= 1f && musicLevels[i] <= 0f)
            {
                musicSources[i].Stop();
                musicSources[i].clip = null;
            }
        }

        if (progress >= 1f) musicIsFading = false;
        RefreshMusicVolumes();
    }

    private void OnDestroy()
    {
        if (instance != this) return;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        wiredButtons.Clear();
        SetupScene(scene);
    }

    private void SetupScene(Scene scene)
    {
        if (catalog == null || !scene.IsValid()) return;
        PlayMusic(catalog.MusicForScene(scene.name));
        if (!catalog.autoWireSceneButtons) return;

        foreach (GameObject root in scene.GetRootGameObjects())
            foreach (Button button in root.GetComponentsInChildren<Button>(true))
                if (wiredButtons.Add(button)) button.onClick.AddListener(PlayUISelect);
    }

    private void PlayUISelect()
    {
        Play(AudioCue.UISelect);
    }

    public static bool Play(AudioCue cue, AudioCueEntry[] overrides = null)
    {
        return instance != null && instance.PlayEffect(cue, overrides);
    }

    private bool PlayEffect(AudioCue cue, AudioCueEntry[] overrides)
    {
        AudioCueEntry entry = null;
        if (overrides != null)
            foreach (AudioCueEntry candidate in overrides)
                if (candidate != null && candidate.cue == cue && candidate.HasClips)
                {
                    entry = candidate;
                    break;
                }

        if (entry == null) entry = catalog != null ? catalog.Find(cue) : null;
        if (entry == null || !entry.HasClips) return false;

        float now = Time.unscaledTime;
        if (entry.cooldown > 0f && entry.hasPlayed && now - entry.lastPlayedAt < entry.cooldown)
            return false;

        AudioClip clip = entry.ChooseClip();
        if (clip == null) return false;
        EffectVoice voice = FindVoice(cue);
        if (voice == null) return false;

        voice.source.Stop();
        voice.cue = cue;
        voice.baseVolume = Mathf.Clamp01(entry.volume);
        voice.source.clip = clip;
        voice.source.pitch = Random.Range(Mathf.Max(0.1f, entry.minPitch), Mathf.Max(0.1f, entry.maxPitch));
        voice.source.volume = muted ? 0f : voice.baseVolume * masterVolume * effectsVolume;
        voice.source.Play();
        entry.lastPlayedAt = now;
        entry.hasPlayed = true;
        return true;
    }

    private EffectVoice FindVoice(AudioCue cue)
    {
        bool enemyCue = IsEnemyCue(cue);
        bool lowPriority = IsLowPriority(cue);
        if (enemyCue && catalog != null)
        {
            int enemyCount = 0;
            foreach (EffectVoice voice in voices)
                if (voice.source.isPlaying && IsEnemyCue(voice.cue)) enemyCount++;
            if (enemyCount >= Mathf.Max(0, catalog.maxConcurrentEnemySounds))
            {
                if (lowPriority) return null;
                foreach (EffectVoice voice in voices)
                    if (voice.source.isPlaying && IsLowPriority(voice.cue)) return voice;
                return null;
            }
        }

        foreach (EffectVoice voice in voices)
            if (!voice.source.isPlaying) return voice;

        if (!lowPriority)
            foreach (EffectVoice voice in voices)
                if (IsLowPriority(voice.cue)) return voice;
        return null;
    }

    private static bool IsEnemyCue(AudioCue cue)
    {
        return cue >= AudioCue.EnemySpawn && cue <= AudioCue.EnemyDeath;
    }

    private static bool IsLowPriority(AudioCue cue)
    {
        return cue == AudioCue.EnemyWalk || cue == AudioCue.EnemyRandom;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (requestedMusic == clip) return;
        requestedMusic = clip;

        int incoming = -1;
        if (clip != null)
        {
            for (int i = 0; i < musicSources.Length; i++)
                if (musicSources[i].isPlaying && musicSources[i].clip == clip) incoming = i;

            if (incoming < 0)
            {
                // Reuse the quieter source if a previous crossfade is still running.
                incoming = musicLevels[0] <= musicLevels[1] ? 0 : 1;
                musicSources[incoming].Stop();
                musicSources[incoming].clip = clip;
                musicLevels[incoming] = 0f;
                musicSources[incoming].volume = 0f;
                musicSources[incoming].Play();
            }
        }

        fadeElapsed = 0f;
        musicIsFading = true;
        for (int i = 0; i < musicSources.Length; i++)
        {
            fadeStartLevels[i] = musicLevels[i];
            fadeTargetLevels[i] = i == incoming ? 1f : 0f;
        }

        if (catalog == null || catalog.musicFadeDuration <= 0f) AdvanceMusicFade(0f);
        else RefreshMusicVolumes();
    }

    public void StopMusic() => PlayMusic(null);

    public void StopAllEffects()
    {
        foreach (EffectVoice voice in voices) voice.source.Stop();
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MasterKey, masterVolume);
        RefreshVolumes();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(MusicKey, musicVolume);
        RefreshVolumes();
    }

    public void SetEffectsVolume(float value)
    {
        effectsVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(EffectsKey, effectsVolume);
        RefreshVolumes();
    }

    public void SetMuted(bool value)
    {
        muted = value;
        PlayerPrefs.SetInt(MuteKey, value ? 1 : 0);
        RefreshVolumes();
    }

    private void RefreshVolumes()
    {
        RefreshMusicVolumes();
        foreach (EffectVoice voice in voices)
            voice.source.volume = muted ? 0f : voice.baseVolume * masterVolume * effectsVolume;
    }

    private void RefreshMusicVolumes()
    {
        for (int i = 0; i < musicSources.Length; i++)
            musicSources[i].volume = muted ? 0f : musicLevels[i] * masterVolume * musicVolume;
    }
}

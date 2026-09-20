using System;
using System.Collections.Generic;
using UnityEngine;

// Keep these values stable: AudioCatalog.asset serializes cues by number.
public enum AudioCue
{
    UISelect = 0,
    TowerBuild = 1,
    TowerUpgrade = 2,
    TowerFire = 3,
    WaveStart = 4,
    ProjectileImpact = 5,
    EnemySpawn = 6,
    EnemyWalk = 7,
    EnemyRandom = 8,
    EnemyDamage = 9,
    EnemyDeath = 10,
    BaseDamage = 11,
    UIBack = 12,
    Pause = 13,
    Resume = 14
}

[Serializable]
public class AudioCueEntry
{
    public AudioCue cue;
    public AudioClip[] clips;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float minPitch = 1f;
    [Range(0.1f, 3f)] public float maxPitch = 1f;
    [Min(0f)] public float cooldown;
    [NonSerialized] internal float lastPlayedAt;
    [NonSerialized] internal bool hasPlayed;

    public bool HasClips
    {
        get
        {
            if (clips == null) return false;
            foreach (AudioClip clip in clips)
                if (clip != null) return true;
            return false;
        }
    }

    public AudioClip ChooseClip()
    {
        if (clips == null || clips.Length == 0) return null;
        int start = UnityEngine.Random.Range(0, clips.Length);
        for (int i = 0; i < clips.Length; i++)
        {
            AudioClip clip = clips[(start + i) % clips.Length];
            if (clip != null) return clip;
        }
        return null;
    }
}

[Serializable]
public class SceneMusicEntry
{
    public string sceneName;
    public AudioClip clip;
    [Range(0f, 1f), Tooltip("Multiplied by the master and music volume settings.")]
    public float volume = 1f;
}

[Serializable]
public class SoundEffectVolumeEntry
{
    public AudioClip clip;
    [Range(0f, 1f), Tooltip("Applied to this file everywhere it is used, including prefab overrides.")]
    public float volume = 1f;
}

[CreateAssetMenu(fileName = "AudioCatalog", menuName = "Audio/Audio Catalog")]
public class AudioCatalog : ScriptableObject
{
    public AudioClip defaultMusic;
    [Range(0f, 1f), Tooltip("Used when no scene music entry matches. Multiplied by the master and music volume settings.")]
    public float defaultMusicVolume = 1f;
    public List<SceneMusicEntry> sceneMusic = new List<SceneMusicEntry>();
    [Min(0f)] public float musicFadeDuration = 1f;
    [Min(1)] public int effectVoiceCount = 16;
    [Min(0)] public int maxConcurrentEnemySounds = 6;
    [Header("Sound Effect File Volumes")]
    public List<SoundEffectVolumeEntry> soundEffectVolumes = new List<SoundEffectVolumeEntry>();
    public List<AudioCueEntry> effects = new List<AudioCueEntry>();
    public bool autoWireSceneButtons = true;

    public AudioCueEntry Find(AudioCue cue)
    {
        if (effects == null) return null;
        foreach (AudioCueEntry entry in effects)
            if (entry != null && entry.cue == cue) return entry;
        return null;
    }

    public float EffectVolumeFor(AudioClip clip)
    {
        if (clip == null || soundEffectVolumes == null) return 1f;
        foreach (SoundEffectVolumeEntry entry in soundEffectVolumes)
            if (entry != null && entry.clip == clip) return Mathf.Clamp01(entry.volume);
        return 1f;
    }

    public SceneMusicEntry FindMusicForScene(string sceneName)
    {
        if (sceneMusic != null)
            foreach (SceneMusicEntry entry in sceneMusic)
                if (entry != null && entry.sceneName == sceneName) return entry;
        return null;
    }

    public AudioClip MusicForScene(string sceneName)
    {
        SceneMusicEntry entry = FindMusicForScene(sceneName);
        return entry != null ? entry.clip : defaultMusic;
    }
}

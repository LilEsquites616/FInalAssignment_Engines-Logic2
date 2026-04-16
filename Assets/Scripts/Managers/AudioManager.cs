using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private bool playMusicOnStart = true;
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.6f;

    [Header("SFX")]
    [SerializeField] private AudioClip playerShotClip;
    [SerializeField] private AudioClip enemyShotClip;
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;

    [Header("Optional Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureAudioSources();
        ApplyVolumes();

        if (playMusicOnStart)
        {
            PlayMusic();
        }
    }

    private void OnValidate()
    {
        ApplyVolumes();
    }

    public void PlayMusic()
    {
        if (musicClip == null)
        {
            return;
        }

        EnsureAudioSources();

        if (musicSource.clip != musicClip)
        {
            musicSource.clip = musicClip;
        }

        musicSource.loop = true;

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void PlayPlayerShot()
    {
        PlaySfx(playerShotClip);
    }

    public void PlayEnemyShot()
    {
        PlaySfx(enemyShotClip);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        EnsureAudioSources();
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    private void EnsureAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = GetOrCreateAudioSource("Music Source");
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }

        if (sfxSource == null)
        {
            sfxSource = GetOrCreateAudioSource("SFX Source");
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
        }
    }

    private AudioSource GetOrCreateAudioSource(string childName)
    {
        Transform child = transform.Find(childName);

        if (child == null)
        {
            GameObject childObject = new GameObject(childName);
            childObject.transform.SetParent(transform);
            childObject.transform.localPosition = Vector3.zero;
            child = childObject.transform;
        }

        AudioSource source = child.GetComponent<AudioSource>();

        if (source == null)
        {
            source = child.gameObject.AddComponent<AudioSource>();
        }

        return source;
    }

    private void ApplyVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = 1f;
        }
    }
}

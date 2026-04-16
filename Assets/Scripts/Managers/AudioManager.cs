using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private bool playMusicOnStart = true;
    [SerializeField] private bool keepMusicPlaying = true;
    [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.6f;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip playerShotClip;
    [SerializeField] private AudioClip enemyShotClip;
    [SerializeField] private AudioClip playerHurtClip;
    [SerializeField] private AudioClip enemyHurtClip;
    [SerializeField] private AudioClip pickupClip;

    [Header("SFX Volume")]
    [SerializeField] [Range(0f, 1f)] private float playerShotVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float enemyShotVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float playerHurtVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float enemyHurtVolume = 1f;
    [SerializeField] [Range(0f, 1f)] private float pickupVolume = 1f;

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

    private void Update()
    {
        if (!keepMusicPlaying || !playMusicOnStart || musicClip == null)
        {
            return;
        }

        EnsureAudioSources();

        if (!musicSource.isPlaying)
        {
            PlayMusic();
        }
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
        PlaySfx(playerShotClip, playerShotVolume);
    }

    public void PlayEnemyShot()
    {
        PlaySfx(enemyShotClip, enemyShotVolume);
    }

    public void PlayPlayerHurt()
    {
        PlaySfx(playerHurtClip, playerHurtVolume);
    }

    public void PlayEnemyHurt()
    {
        PlaySfx(enemyHurtClip, enemyHurtVolume);
    }

    public void PlayPickup()
    {
        PlaySfx(pickupClip, pickupVolume);
    }

    private void PlaySfx(AudioClip clip, float volume)
    {
        if (clip == null)
        {
            return;
        }

        EnsureAudioSources();
        sfxSource.PlayOneShot(clip, volume);
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

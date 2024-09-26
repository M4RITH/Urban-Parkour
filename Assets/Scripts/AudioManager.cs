using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource musicSource;

    [Header("Volume Control")]
    [Range(0.0f, 1.0f)]
    public float musicVolume = 1.0f;

    [Header("Audio Clip")]
    public AudioClip backgroundMusic;

    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();

                if (_instance == null)
                {
                    GameObject obj = new GameObject("AudioManager");
                    _instance = obj.AddComponent<AudioManager>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        // Set initial volume level
        musicSource.volume = musicVolume;

        // Play background music
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
    }
}

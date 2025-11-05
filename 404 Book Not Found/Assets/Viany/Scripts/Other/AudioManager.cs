using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public AudioClip attackSfx;
    public AudioClip damageSfx;
    public AudioClip doorOpen;
    public AudioClip footsteps;
    public AudioClip pickup;
    public AudioClip uiButtons;
    public AudioClip knoflookDeath;
    public AudioClip lose;
    public AudioClip winSound;
    public AudioClip ingameBg;
    public AudioClip mainMenuBg;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup musicGroup;
    public bool playMusicOnStart = true;

    public AudioSource[] sfxSources;
    public AudioSource musicSource;
    private const int SFX_POOL_SIZE = 12;

    private void Awake()
    {
        sfxSources = new AudioSource[SFX_POOL_SIZE];
        for (int i = 0; i < SFX_POOL_SIZE; i++)
        {
            sfxSources[i] = gameObject.AddComponent<AudioSource>();
            sfxSources[i].playOnAwake = false;
            sfxSources[i].spatialBlend = 0f;
            sfxSources[i].outputAudioMixerGroup = sfxGroup;
        }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.outputAudioMixerGroup = musicGroup;
    }

    private void Start()
    {

    }

    public void PlayAttack() => PlaySfx(attackSfx);
    public void PlayDamage() => PlaySfx(damageSfx);
    public void PlayDoorOpen() => PlaySfx(doorOpen);
    public void PlayFootsteps() => PlaySfx(footsteps);
    public void PlayPickup() => PlaySfx(pickup);
    public void PlayUIButton() => PlaySfx(uiButtons);
    public void PlayKnoflookDeath() => PlaySfx(knoflookDeath);
    public void PlayLose() => PlaySfx(lose);
    public void PlayWin() => PlaySfx(winSound);

    public void PlayIngameMusic()
    {
        if (ingameBg == null || musicGroup == null) return;
        musicSource.clip = ingameBg;
        musicSource.Play();
    }

    public void PlayMainMenuMusic()
    {
        if (mainMenuBg == null || musicGroup == null) return;
        musicSource.clip = mainMenuBg;
        musicSource.Play();
    }

    public void StopMusic() => musicSource.Stop();

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxGroup == null) return;
        AudioSource s = GetFreeSfxSource();
        if (s != null)
            s.PlayOneShot(clip);
    }

    private AudioSource GetFreeSfxSource()
    {
        foreach (var s in sfxSources)
            if (!s.isPlaying) return s;
        return sfxSources[0];
    }
}
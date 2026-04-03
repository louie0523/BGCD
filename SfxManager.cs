using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public static SfxManager instance;

    public Dictionary<string, AudioClip> Audios = new Dictionary<string, AudioClip>();

    public AudioSource Sfx;
    public AudioSource Bgm;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            Sfx = GetComponent<AudioSource>();
            Bgm = transform.GetChild(0).GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
            Load();
        } else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
    }

    private void Update()
    {
        Sfx.volume = DataManager.instance.SfxSlider.value;
        Bgm.volume = DataManager.instance.BgmSlider.value;
    }

    void Load()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sound");

        foreach(AudioClip clip in clips)
        {
            Audios.Add(clip.name, clip);
        }

        Debug.Log(Audios.Count + "개의 클립 로드");
    }


    public void Play3DMy(AudioSource audioSource,string name, float volume = 1f)
    {
        if (Audios.TryGetValue(name, out var audioClip))
        {
            audioSource.PlayOneShot(audioClip, volume);
        }
    }

    public void PlaySfx(string name, float volume = 1f)
    {
        if(Audios.TryGetValue(name, out var audioClip))
        {
            Sfx.PlayOneShot(audioClip, volume);
        }
    }

    public void PlayBgm(string name)
    {
        if (Audios.TryGetValue(name, out var audioClip))
        {
            Bgm.Stop();
            Bgm.clip = audioClip;
            Bgm.Play();
        }
    }

    public void StopBgm()
    {
        Bgm.Stop();
    }
}

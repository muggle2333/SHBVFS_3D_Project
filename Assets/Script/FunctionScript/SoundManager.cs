using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;

public enum Sound
{
    PlayCard,
    UseCard,
    DrawCard,
    DiscardCard,
    //
    RoundStart,
    ControlStart,
    ControlEnd,
    //
    MoveToPlain,
    MoveToMountain,
    MoveToLake,
    MoveOnLake,
    //
    Attack,
    Build,
    DestoryBuilding,
    Occupy,
    Search,

}
public enum Bgm
{
    GameplayNormalBgm,
    MainMenuBgm,
    DyingBgm,
    ForestBgm,
    SuccessBgm,
    LossBgm,
}
[Serializable]
public struct SoundAudioClip
{
    public Sound sound;
    public AudioClip audioClip;
}
[Serializable]
public struct BgmAudioClip
{
    public Bgm bgm;
    public AudioClip audioClip;
}
public class SoundManager : NetworkBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private List<SoundAudioClip> audioClipList;
    [SerializeField] private List<BgmAudioClip> bgmClipList;

    private Dictionary<Sound, AudioClip> effectDicts= new Dictionary<Sound, AudioClip>();
    private Dictionary<Bgm, AudioClip> bgmDicts= new Dictionary<Bgm, AudioClip>();
    [SerializeField] private AudioSource effectSource;
    [SerializeField] private AudioSource bgmSource;

    private bool canCountdown = true;
    private void Awake()
    {
        Instance= this;
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == Loader.Scene.GameplayScene.ToString())
        {
            bgmSource.Stop();
        }else if(scene.name == Loader.Scene.MainMenuScene.ToString())
        {
            PlayBgm(Bgm.MainMenuBgm);
        }
    }

    public void Start()
    {
        effectDicts = new Dictionary<Sound, AudioClip>();
        bgmDicts = new Dictionary<Bgm, AudioClip>();

        foreach (var soundAudioClip in audioClipList)
        {
            effectDicts.Add(soundAudioClip.sound, soundAudioClip.audioClip);
        }
        foreach(var bgmAudiClip in bgmClipList)
        {
            bgmDicts.Add(bgmAudiClip.bgm, bgmAudiClip.audioClip);
        }
    }
    public void PlaySound(Sound sound)
    {
        if(GetAudioClip(sound)==null) return;
        effectSource.PlayOneShot(GetAudioClip(sound));
    }

    [ClientRpc]
    public void PlaySoundClientRpc(Sound sound)
    {
        if (GetAudioClip(sound) == null) return;
        effectSource.PlayOneShot(GetAudioClip(sound));
    }

    [ClientRpc]
    public void PlayCountDownClientRpc(int times)
    {
        if(canCountdown)
        {
            StartCoroutine(PlayCountDownScene(1.0f,times));
        }
        
    }

    [ClientRpc]
    public void StopCountDownClientRpc()
    {
        StopAllCoroutines();
        canCountdown = true;
    }

    IEnumerator PlayCountDownScene(float duration,int times)
    {
        canCountdown= false;
        for(int i = 0;i<times;i++)
        {
            PlaySound(Sound.ControlEnd);
            yield return new WaitForSeconds(duration);
        }
        canCountdown= true;
    }
    private void StopLoopSound()
    {
        effectSource.loop = false;
    }
    public void PlayBgm(Bgm bgm)
    {
        if (GetAudioClip(bgm) == null) return;
        bgmSource.PlayOneShot(GetAudioClip(bgm));
    }
    private AudioClip GetAudioClip(Sound sound)
    {
        AudioClip audioClip = null;
        effectDicts.TryGetValue(sound, out audioClip);
        return audioClip;
    }
    private AudioClip GetAudioClip(Bgm bgm)
    {
        AudioClip audioClip = null;
        bgmDicts.TryGetValue(bgm, out audioClip);
        return audioClip;
    }

}

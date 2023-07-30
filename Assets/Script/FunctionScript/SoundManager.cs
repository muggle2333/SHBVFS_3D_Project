using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;

public enum Sound
{
    PlayCard,//0
    CardTakeEffect,//1
    DrawCard,//2
    DiscardCard,//3

    Attack,//4
    Build,//5
    DestoryBuilding,//6
    Occupy,//7
    Search,//8

    MoveToPlain,//9
    MoveToMountain,//10
    MoveToLake,//11
    MoveToForest,//12
    MoveOnLake,//13

    Button,//14
    ControlInput,//15
    CountDown,//16
    ControlStart,//17
    GameStart,//18

    CardSelect,//24
    CardCancel,//25
}
public enum Bgm
{
    LateBGM,//19
    LobbyBGM,//20
    DyingBGM,//21
    WinBGM,//22
    LoseBGM,//23
    EarlyBGM//24
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
        if(Instance !=null &&Instance!=this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TutorialScene" || scene.name.Contains("GameplayScene"))
        {
            PlayBgm(Bgm.EarlyBGM);
        }
        else if (scene.name == Loader.Scene.MainMenuScene.ToString()|| scene.name == Loader.Scene.WaitRoomScene.ToString())
        {
            PlayBgm(Bgm.LobbyBGM);
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

        PlayBgm(Bgm.LobbyBGM);
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
            PlaySound(Sound.CountDown);
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
        if (Instance == null) return;
        bgmSource.Stop();
        if (GetAudioClip(bgm) == null) return;
        bgmSource.clip = GetAudioClip(bgm);
        bgmSource.Play();
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
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void PlayMoveSound(GridObject start,GridObject end)
    {
        if (end.landType == LandType.Mountain)
        {
            PlaySound(Sound.MoveToMountain);
        }
        else if (end.landType == LandType.Lake)
        {
            if (start.landType == LandType.Lake)
            {
                PlaySound(Sound.MoveOnLake);
            }
            else
            {
                PlaySound(Sound.MoveToLake);
            }
        }
        else if ( end.landType == LandType.Forest)
        {
            PlaySound(Sound.MoveToForest);
        }
        else
        {
            PlaySound(Sound.MoveToPlain);
        }
    }
}

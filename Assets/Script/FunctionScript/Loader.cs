using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenuScene,
        GameplayScene,
        LoadingScene,
        LobbyScene,
        WaitRoomScene,
    }

    private static Scene targetScene;

    public static void Load(Scene scene)
    {
        targetScene = scene;
        if (scene == Scene.MainMenuScene)
        {
            MainMenuCleanUp.Instance.CleanUp();
        }
        SceneManager.LoadScene(scene.ToString());
    }

    public static void LoadNetwork(Scene scene)
    {
        targetScene = scene;
        if(scene == Scene.MainMenuScene)
        {
            MainMenuCleanUp.Instance.CleanUp();
        }
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }
    
    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}

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
        TutorialScene,
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
        //var activeScene = SceneManager.GetSceneByName(scene.ToString());
        //SceneManager.SetActiveScene(activeScene);
    }

    public static void LoadNetwork(Scene scene)
    {
        targetScene = scene;
        if(scene == Scene.MainMenuScene)
        {
            MainMenuCleanUp.Instance.CleanUp();
        }
        var activeScene = SceneManager.GetSceneByName(targetScene.ToString());
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }
    
    public static void LoadGameplayScene(int levelIndex)
    {
        NetworkManager.Singleton.SceneManager.LoadScene("GameplayScene_" + (levelIndex+1), LoadSceneMode.Single);
    }
    public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }

}

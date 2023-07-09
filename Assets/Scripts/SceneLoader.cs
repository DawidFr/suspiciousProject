using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class SceneLoader{
    public enum Scene{
        MainMenu,
        Lobby,
        LoadingScene,
        Game
    }
    private static Scene targetScene;



    public static void Load(Scene targetScene){
        SceneLoader.targetScene = targetScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }
    public static void LoadNetwork(Scene targetScene){
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }
    public static void SceneLoaderCallback(){
        SceneManager.LoadScene(targetScene.ToString());
    }

}

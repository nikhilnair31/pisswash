using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class Manager_Scene : MonoBehaviour 
{
    #region Vars
    public static Manager_Scene Instance { get; private set; }

    [SerializeField] private string currentSceneName;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public void LoadCurrentScene() {
        SceneManager.LoadScene(currentSceneName);
    }
    public void LoadSceneByName(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
    
    public string GetCurrSceneName() {
        return SceneManager.GetActiveScene().name;
    }
    public string GetNextSceneName(string currentSceneName) {
        var sceneNameList = GetSceneStrArr();
        var currentSceneIndex = System.Array.IndexOf(sceneNameList, currentSceneName);
        var nextSceneIndex = currentSceneIndex + 1;
        
        return sceneNameList[nextSceneIndex];
    }
    public string[] GetSceneStrArr() {
        // Get list of all scenes in build settings
        var sceneNumber = SceneManager.sceneCountInBuildSettings;
        var sceneNameList = new string[sceneNumber];
        for (int i = 0; i < sceneNumber; i++) {
            sceneNameList[i] = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }
        sceneNameList = sceneNameList[1..]; // Remove the first scene (main menu)
        Debug.Log($"sceneNameList: {string.Join(", ", sceneNameList)}");

        return sceneNameList;
    }
}
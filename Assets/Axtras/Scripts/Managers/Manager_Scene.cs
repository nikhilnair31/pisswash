using UnityEngine;
using UnityEngine.SceneManagement;

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
}
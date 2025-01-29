using UnityEngine;
using UnityEngine.Playables;

public class Manager_Timeline : MonoBehaviour 
{
    #region Vars
    public static Manager_Timeline Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private bool skipIntroCutscene;
    [SerializeField] private PlayableDirector startGameCutscene;
    [SerializeField] private PlayableDirector ventEnterCutscene;
    [SerializeField] private PlayableDirector ventExitCutscene;
    #endregion 

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start() {
        if (skipIntroCutscene) {
            Manager_UI.Instance.StartGame();            
        }
    }

    public void PlayCutscene_GameStart() {
        if (!skipIntroCutscene) {
            startGameCutscene.Play();
            startGameCutscene.playableGraph.GetRootPlayable(0).SetSpeed(1f);
        }
        else {
            startGameCutscene.time = startGameCutscene.duration;
            startGameCutscene.Evaluate();
        }
    }
    public void PlayCutscene_VentEnter() {
        ventEnterCutscene.Play();
    }
    public void PlayCutscene_VentExit() {
        ventExitCutscene.Play();
    }
}
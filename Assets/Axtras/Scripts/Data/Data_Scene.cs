using System;

[Serializable]
public class Data_Scene {
    public string sceneName;
    public string scoreRating;
    public bool playable;
    public bool unlocked;

    public Data_Scene() {
        sceneName = "";
        scoreRating = "";
        playable = true;
        unlocked = false;
    }
}
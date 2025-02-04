using System;

[Serializable]
public class Data_Scene {
    public string name;
    public string rating;
    public bool playable;
    public bool unlocked;

    public Data_Scene() {
        name = "";
        rating = "";
        playable = true;
        unlocked = false;
    }
}
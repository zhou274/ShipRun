using UnityEngine;
using UnityEngine.SocialPlatforms;

public class LogoScene : MonoBehaviour {
    //Start after this amount of time
    public float timeToLoad = 1.5f;
	// Use this for initialization
	void Start () {
        Application.targetFrameRate = 60;
        Invoke("LoadGame", timeToLoad);

     }
    //Load game
    void LoadGame() {
        Initiate.Fade("Game", Color.black, 2.0f);
    }
}

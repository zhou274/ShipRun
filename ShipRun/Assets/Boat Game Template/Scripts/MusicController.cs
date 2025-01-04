using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MusicController : MonoBehaviour {
    //get the source
    AudioSource myMusic;
    bool started = false;
	// Use this for initialization
	void Start () {
        
        if (transform.GetComponent<AudioSource>())
            myMusic = transform.GetComponent<AudioSource>();
        //don't destroy on load
        DontDestroyOnLoad(gameObject);
	}
    
    private void Update() {
        if (myMusic && SceneManager.GetActiveScene().name == "Game" && !started)
        {
            myMusic.Play();
            started = true;
        }
    }
}

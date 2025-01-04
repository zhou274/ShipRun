using UnityEngine;
using System.Collections;
using System.Drawing;

//limit power up spawns
public static class PowerUPCounterClass{
    static int powerUpCounter = 0;
    public static int powerUpSpawnIn = 5;

    public static bool CanSpawn() {
        powerUpCounter++;
        if (powerUpCounter >= powerUpSpawnIn)
        {
            powerUpCounter = 0;
            return true;
        }
        else {
            return false;
        }
    }

}

public class ScrollController : MonoBehaviour {
    //minimum speed
    public float minSpeed = 8.0f;
    public float speed = 0.0f;
    //max Speed
    public float maxSpeed = 18.0f;
    //Hold speed
    [HideInInspector]
    public float holdUpSpeed = 0.0f;
    //All the panels to scroll
    public PlatformController[] panels;
    //Last panel
    public PlatformController lastPanel;
    //Get all the challenges
    public GameObject[] challenges;
    //spawn powerups after ,the higher the number the rarely they will spawn 
    public int powerUpSpawnIn = 5;

    //Reference to water material
    public Renderer water;
    //Factor for water speed control
    public float waterSpeedFactor;
    //Default water flow speed
    float defaultWaterSpeed;


    // Use this for initialization
    void Start () {
        if (water){
            defaultWaterSpeed = water.material.GetFloat("_flowSpeed");
        }

        holdUpSpeed = speed;

        PowerUPCounterClass.powerUpSpawnIn = powerUpSpawnIn;

        foreach (PlatformController panel in panels)
        {
            if (panel)
            {
                panel.AssignChallengesArray(challenges);
                panel.CreateChallenge();
            }
        }

        speed = 0.0f;

    }
	//Scroll platforms and spawn challenges
	// Update is called once per frame
	void Update () {

        if (!lastPanel) {
            
            return;
        }

        if (water){
            water.material.SetFloat("_flowSpeed" , waterSpeedFactor);
        }

        foreach (PlatformController panel in panels) {
            if (panel) {

                panel.transform.position -= Vector3.forward * (Time.deltaTime * speed);

                if (panel.GetEndPoint().z <= -15.0f) {
                    panel.transform.position = lastPanel.GetEndPoint();
                    panel.CreateChallenge();
                    lastPanel = panel;
                }
                
            }
        }

	}
    //Set start speed
    public void StartSetup() {
        speed = minSpeed;
    }
    //Reset the flow Speed
    public void Crashed (){
        if (water){
            water.material.SetFloat("_flowSpeed" , defaultWaterSpeed);
        }
    }
}

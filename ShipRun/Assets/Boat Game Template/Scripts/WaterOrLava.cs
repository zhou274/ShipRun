using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterOrLava : MonoBehaviour
{
    //Water Materials
    public Material[] waterTypes;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.GetComponent<Renderer>() && waterTypes.Length >0){
            int matChosen = Random.Range(0,waterTypes.Length);

            if (waterTypes[matChosen]){
            transform.GetComponent<Renderer>().material = waterTypes[matChosen];
            }
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

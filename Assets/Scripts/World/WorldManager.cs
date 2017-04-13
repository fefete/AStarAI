using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    // Boolean varible to determin if the Blue Team Flag has been captured
    public bool BT_FlagCaptured { get; set; }

    // Boolean varible to determin if the Blue Team Flag has been captured
    public bool RT_FlagCaptured { get; set; }

    public bool testBT;
    public bool testRT;

    public GameObject blueFlag;
    public GameObject redFlag;

    public GameObject blueTeamFlagCarrier { get; set; }
    public GameObject redTeamFlagCarrier { get; set; }

    // Use this for initialization
    void Start () {

        // Set the flag captured values to false on game start
        BT_FlagCaptured = false;
        RT_FlagCaptured = false;

	}
	
	// Update is called once per frame
	void Update () {

        Debug.Log("Red: " + RT_FlagCaptured);
        Debug.Log("Blue: " + BT_FlagCaptured);

        if(RT_FlagCaptured == true)
        {
            redFlag.transform.position = blueTeamFlagCarrier.transform.position;
            blueTeamFlagCarrier.GetComponent<AIUnit>().hasFlag = true;
        }
        else if(BT_FlagCaptured == true)
        {
            blueFlag.transform.position = redTeamFlagCarrier.transform.position;
            redTeamFlagCarrier.GetComponent<AIUnit>().hasFlag = true;
        }


	}
}

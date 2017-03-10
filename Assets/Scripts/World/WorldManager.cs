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

    // Use this for initialization
    void Start () {

        // Set the flag captured values to false on game start
        BT_FlagCaptured = false;
        RT_FlagCaptured = false;

	}
	
	// Update is called once per frame
	void Update () {

        BT_FlagCaptured = testBT;
        RT_FlagCaptured = testRT;

	}
}

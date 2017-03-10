using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUnit : MonoBehaviour {

    //Team Enume
    private enum Team
    {
        Red, Blue
    }

    // Enum States
    public enum State
    {
        //
        // Capture - Go for enemy Flag
        // Defend - defend team Flag
        //
        Capture, Defend
    }

    enum SubState
    {
        //
        // Cover - Cover flag carrier
        // Retrieve - Kill enemy flag Carrier (get your team flag back)
        // Fighting - Currently in combat
        //
        Cover, Retrieve, Fighting
    }

    // Private Varibles
    private string teamTag_;
    private bool teamFlagCaptured_;
    private bool enemeyFlagCaptured_;
    private Team team_;
    private SubState subState_;

    //Public Variables
    public WorldManager worldManager_;
    public AISight aiSight_;
    public State state;

    // Use this for initialization
    void Start () {

        // Suppose this could be set with a public enum variable 
        teamTag_ = gameObject.tag; 
        
        // Gain Access to the world manager script
        worldManager_ = worldManager_.GetComponent<WorldManager>();

        teamFlagCaptured_ = false;
        enemeyFlagCaptured_ = false;

        if(teamTag_ == "BlueTeam")
        {
            team_ = Team.Blue;
        }else
        {
            team_ = Team.Red;
        }
    }
	
	// Update is called once per frame
	void Update () {

        //Check if your flag has been captured
        if(team_ == Team.Blue && worldManager_.BT_FlagCaptured == true)
        {
            subState_ = SubState.Retrieve;
        }


        switch (state)
        {
            case State.Capture:
                //TODO: Add Decision Making Here
                // Find path to flag 
            break;

            case State.Defend:
                //TODO: Add Decision Making Here
            break;
        }

        switch (subState_)
        {
            case SubState.Retrieve:
                //TODO: Add Decision Making Here
            break;

            case SubState.Cover:
                //TODO: Add Decision Making Here
            break;

            case SubState.Fighting:
                //TODO: Add Decision Making Here
            break;
        }

	}
}

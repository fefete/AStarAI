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


    // Private Varibles
    private string teamTag_;
    private bool teamFlagCaptured_;
    private bool enemeyFlagCaptured_;
    private Team team_;

    public Vector2[] path_;

    //Public Variables
    public WorldManager worldManager_;
    public AStarManager AStarManager_;
    private CaptureState captureState_;
    private DefendState defendState_;
    public AISight aiSight_;
    public State state;

    public float health;
    public float maxChaseDistance;

    public Node startNode;
    public Node targetNode;

    // Use this for initialization
    void Start () {

        // Suppose this could be set with a public enum variable 
        teamTag_ = gameObject.tag; 
        
        // Gain Access to the world manager script
        worldManager_ = worldManager_.GetComponent<WorldManager>();

        teamFlagCaptured_ = false;
        enemeyFlagCaptured_ = false;

        Renderer rend = GetComponent<Renderer>();

        if(teamTag_ == "BlueTeam")
        {
            team_ = Team.Blue;
            rend.material.color = Color.blue;
        }else
        {
            team_ = Team.Red;
            rend.material.color = Color.red;
        }

        captureState_ = this.GetComponent<CaptureState>();
        defendState_ = this.GetComponent<DefendState>();

        captureState_.enabled = false;
        defendState_.enabled = false;

    }
	
	// Update is called once per frame
	void Update () {

        switch(state)
        {
            case State.Capture:
                defendState_.enabled = false;
                captureState_.enabled = true;
                break;

            case State.Defend:
                captureState_.enabled = false;
                defendState_.enabled = true;
                break;
        }


        //Check if your flag has been captured
        if (team_ == Team.Blue && worldManager_.BT_FlagCaptured == true)
        {
            captureState_.subState = CaptureState.SubState.Retrieve;
        }


        //switch (state)
        //{
        //    case State.Capture:
        //        //TODO: Add Decision Making Here
        //        // Find path to flag 
        //        // Shoot closest enemy
        //        // If enemy flag was captured (by another unit), goto and provide cover for flag carrier
        //        // If carrying flag, 
        //            // get new path to base
        //            // call for back up
        //    break;

        //    case State.Defend:
        //        //TODO: Add Decision Making Here
        //        // Shoot closest enemy
        //        // Do patrol (coin flip based)
        //    break;
        //}

        //switch (subState_)
        //{
        //    case SubState.Retrieve:
        //        //TODO: Add Decision Making Here
        //        // If NOT NEAR enemy flag, goto and recover team flag
        //        // If NEAR go for flag
        //        // If seen flag carrier goto and kill
        //        // If alerted (messaged) goto and kill
        //        // If 'Defending' chase enemy flag carrier
        //        // Shoot closest enemy
        //    break;

        //    case SubState.Cover:
        //        //TODO: Add Decision Making Here
        //        // Get path to flag carrier
        //        // Shoot closest enemy
        //    break;

        //    case SubState.Fighting:
        //        //TODO: Add Decision Making Here
        //        // If health is low
        //            //run away or call for backup (if allies nearby)
        //        // If enemy ran away
        //            // Chase (how long / far) ? 
        //    break;
        //}

    }

    public void getNewPath(Node startNode, Node endNode)
    {
        AStarManager_.calculatePath(startNode, endNode, out path_);
    }

    public void shoot(GameObject target)
    {
        Debug.Log("Shooting at Target: " + target.name);
    }

    public void flee()
    {
        Debug.Log(this.gameObject.name + " is running away from a fight");
    }

    public void chase(GameObject target, Vector3 startPoint, float maxChaseDistance)
    {


    }

}

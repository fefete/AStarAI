using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUnit : MonoBehaviour {

    //Team Enume
    public enum Team
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
    private int IDXcounter_;
    public Team team_;

    //public Vector3[] path_;
    public List<Vector3> path_;
    public List<Node> Npath_;

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
    public Node deliveryNode;

    public bool hasFlag;

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

        //StartCoroutine("waitToStart");

        startNode = getClosestNode();

        IDXcounter_ = 0;
        hasFlag = false;

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

    IEnumerator waitToStart()
    {
        while(AStarManager_.ready == false)
        {
            yield return null;
        }
    }

    public void getNewPath(Node startNode, Node endNode)
    {
        AStarManager_.calculatePath(startNode, endNode, out path_, out Npath_);
    }

    public Node getClosestNode()
    {
        return AStarManager_.getNearestNode(this.transform);
    }

    public void moveAlongPath()
    {
        Vector3 targetNode = path_[IDXcounter_];

        float distToNode = Vector3.Distance(transform.position, new Vector3(targetNode.x, targetNode.y, targetNode.z));

        if (IDXcounter_ != path_.Count - 1 && distToNode < 0.1f)
        {
            IDXcounter_++;
        }


        transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetNode.x, targetNode.y, targetNode.z), 7.5f * Time.deltaTime);
    }

    public void shoot(GameObject target)
    {
        Debug.Log("Shooting at Target: " + target.name);
    }

    public void flee()
    {
        Debug.Log(this.gameObject.name + " is running away from a fight");
    }

    public IEnumerator deliverFlag(Node startNode)
    {
        path_.Clear();
        IDXcounter_ = 0;

        getNewPath(startNode, deliveryNode);

        Debug.Log("1");
        while (hasFlag)
        {
            Debug.Log("Delivering.. " + startNode + ", " + deliveryNode);
            moveAlongPath();
            yield return null;
        }
    }

    public IEnumerator chase(Vector3 startOfChasePoint, GameObject enemyUnit)
    {
        bool canChase = true;
        while (canChase == true)
        {
            float distFromBeginChase = Vector3.Distance(transform.position, startOfChasePoint);

            if (distFromBeginChase < maxChaseDistance)
            {
                float dist = Vector3.Distance(enemyUnit.transform.position, transform.position);

                //Stop moving/chasing when close enough to the enemy 
                if (dist > 5.0f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, enemyUnit.transform.position, Time.deltaTime);
                }
            }
            else
            {
                canChase = false;
            }

            yield return null;
        }
    }

    public IEnumerator retreat(Vector3 returnPoint)
    {
        bool done = true;
        while (done == true)
        {
            Debug.Log("Cant chase anymore, should go back to start point");
            transform.position = Vector3.MoveTowards(transform.position, returnPoint, 0.1f);

            if (transform.position == returnPoint)
            {
                done = false;
            }

            yield return null;
        }

    }

}

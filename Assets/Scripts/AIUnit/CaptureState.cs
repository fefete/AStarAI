using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureState : MonoBehaviour {

    public AISight aiSight_;
    private AIUnit aiUnit_;
    private GameObject enemyUnitTarget_;


    private Vector3 startOfChasePoint;
    private bool retreating_;

    public enum SubState
    {
        //
        // Cover - Cover flag carrier
        // Retrieve - Kill enemy flag Carrier (get your team flag back)
        // Fighting - Currently in combat
        //
        DeliverFlag, CoverFlagCarrier, Retrieve, Fighting, Chase, Retreat
    }

    //Set State Variables
    private SubState subState_;
    public SubState subState
    {
        get
        {
            return subState_;
        }
        set
        {
            //exitState(subState_)
            subState_ = value;
            changeState(subState_);
        }
    }

    // Use this for initialization
    void Start () {

        aiUnit_ = this.GetComponent<AIUnit>();
        aiSight_ = aiSight_.GetComponent<AISight>();

        retreating_ = false;

        //Get Path to Flag
        aiUnit_.getNewPath(aiUnit_.startNode, aiUnit_.targetNode);

	}
	
	// Update is called once per frame
	void Update () {

        //TODO: Fix this / move 
        if (aiUnit_.path_.Count != 0)
        {
            aiUnit_.moveAlongPath();
        }

        //if (aiSight_.hasTarget)
        //{
        //    enemyUnitTarget_ = aiSight_.GetTarget();

        //    float distToTarget = Vector3.Distance(enemyUnitTarget_.transform.position, transform.position);

        //    if (distToTarget < 45.0f)
        //    {
        //        //enemyUnitTarget_ = aiSight_.GetTarget();
        //        //aiUnit_.shoot(enemyUnitTarget_);
        //        //subState = SubState.Fighting;
        //    }
        //}
        //else
        //{
        //    enemyUnitTarget_ = null;
        //}
    }

    void changeState(SubState newState)
    {
        switch (newState)
        {
            case SubState.Retrieve:
                //TODO: Add Decision Making Here
                // If NOT NEAR enemy flag, goto and recover team flag
                // If NEAR go for flag
                // If seen flag carrier goto and kill
                // If alerted (messaged) goto and kill

                Debug.Log("Retrive Flag");

            break;

            case SubState.DeliverFlag:
                Debug.Log("Deliver State");
                Node startNode = aiUnit_.getClosestNode();
                StartCoroutine(aiUnit_.GetComponent<AIUnit>().deliverFlag(startNode));

            break;

            case SubState.CoverFlagCarrier:
                //TODO: Add Decision Making Here
                // Get path to flag carrier
                // Shoot closest enemy
                break;

            case SubState.Fighting:
                //TODO: Add Decision Making Here
                // If health is low
                //run away or call for backup (if allies nearby)
                // If enemy ran away
                // Chase (how long / far) ? 

                //if (aiUnit_.health < 20)
                //{
                //    aiUnit_.flee();
                //}

                //if (aiUnit_.health > 50)  // enemy out of sight / retreating
                //{
                //    Node closestNode = aiUnit_.getClosestNode();

                //    startOfChasePoint = GameObject.Find(closestNode.name).transform.position; // Find Nearest Node and use x,y,z coords as return point
                //    subState = SubState.Chase;
                //}
            break;

            case SubState.Chase:
                
                //StartCoroutine(aiUnit_.GetComponent<AIUnit>().chase(startOfChasePoint, enemyUnitTarget_));
             
            break;


        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "RedFlag" && aiUnit_.team_ != AIUnit.Team.Red)
        {
            subState = SubState.DeliverFlag;
        }
        if(other.gameObject.name == "BlueFlag" && aiUnit_.team_ != AIUnit.Team.Blue)
        {
            subState = SubState.DeliverFlag;
        }
    }

}

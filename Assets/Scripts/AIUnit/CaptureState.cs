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
        DeliverFlag, CoverFlagCarrier, Fighting, Chase, Capturing
    }

    //Set State Variables
    public SubState subState_;
    public SubState subState
    {
        get
        {
            return subState_;
        }
        set
        {
            bool done = false;
            while (done == false)
            {
                //if (exitState(subState_))
                //{
                    subState_ = value;
                    changeState(subState_);
                    done = true;
                //}
            }
        }
    }

    // Use this for initialization
    void Start () {

        aiUnit_ = this.GetComponent<AIUnit>();
        aiSight_ = aiSight_.GetComponent<AISight>();

        retreating_ = false;

        //Get Path to Flag
        aiUnit_.getNewPath(aiUnit_.startNode, aiUnit_.targetNode);

        subState = SubState.Capturing;
    }
	
	// Update is called once per frame
	void Update () {

        if (aiUnit_.isAlive)
        {
            if (aiUnit_.didRespawn)
            {
                aiUnit_.didRespawn = false;
                restartCaptureState();
            }
            if (aiSight_.hasTarget)
            {
                enemyUnitTarget_ = aiSight_.GetTarget();

                float distToTarget = Vector3.Distance(enemyUnitTarget_.transform.position, transform.position);

                if (distToTarget < aiUnit_.shootRange)
                {
                    enemyUnitTarget_ = aiSight_.GetTarget();
                    aiUnit_.shoot(enemyUnitTarget_);
                    subState = SubState.Fighting;
                }
            }
            else
            {
                enemyUnitTarget_ = null;
            }
        }
        else
        {
            StopAllCoroutines();
        }
    }

    // Changes from one state to another
    void changeState(SubState newState)
    {
        switch (newState)
        {
            case SubState.Capturing:
                StopAllCoroutines();
                StartCoroutine(aiUnit_.GetComponent<AIUnit>().capturing());

            break;

            case SubState.DeliverFlag:
                //StopAllCoroutines();
                Node startNode = aiUnit_.getClosestNode();
                StartCoroutine(aiUnit_.GetComponent<AIUnit>().deliverFlag(startNode));

            break;

            case SubState.Fighting:

                // If health is low
                //run away to recovery bay
                if (aiUnit_.health < 40)
                {
                    StartCoroutine(aiUnit_.GetComponent<AIUnit>().flee(aiUnit_.recoveryBay.transform));
                }
            break;

        }

    }

    bool exitState(SubState oldState)
    {
        switch (oldState)
        {
            case SubState.Capturing:
                //StopAllCoroutines();
                return true;
                //StopCoroutine("capturing");
                //aiUnit_.stopCaptureState();

            break;

            case SubState.DeliverFlag:
                //StopCoroutine("deliverFlag");


            break;
                
        }

        return true;
    }

    private void restartCaptureState()
    {
        aiUnit_.startNode = aiUnit_.getClosestNode();

        aiUnit_.getNewPath(aiUnit_.startNode, aiUnit_.targetNode);
        subState = SubState.Capturing;
    }

    public void OnTriggerEnter(Collider other)
    {
        //Check if the AIunit is within range of target flag
        if(other.gameObject.name == "RedFlag" && aiUnit_.team_ != AIUnit.Team.Red)
        {
            subState = SubState.DeliverFlag;
            aiUnit_.hasFlag = true;
        }
        if(other.gameObject.name == "BlueFlag" && aiUnit_.team_ != AIUnit.Team.Blue)
        {
            subState = SubState.DeliverFlag;
            aiUnit_.hasFlag = true;
        }

        //Check if the flag carrie has made it to the delivery point
        if (other.gameObject.name == "FlagDeliveryCollider(Red)" && aiUnit_.team_ == AIUnit.Team.Red && aiUnit_.hasFlag == true)
        {
            aiUnit_.worldManager_.BF_CapturedAndDelivered = true;
            aiUnit_.hasFlag = false;
            restartCaptureState();
        }
        if (other.gameObject.name == "FlagDeliveryCollider(Blue)" && aiUnit_.team_ == AIUnit.Team.Blue && aiUnit_.hasFlag == true)
        {
            aiUnit_.worldManager_.RF_CapturedAndDelivered = true;
            aiUnit_.hasFlag = false;
            restartCaptureState();
        }
    }

}

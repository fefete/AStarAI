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
        DeliverFlag, Empty, Fighting, Chase, Capturing
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
                //First exit the current state
                if (exitState(subState_))
                {
                    subState_ = value;
                    changeState(subState_);
                    done = true;
                }
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
            if(subState == SubState.Empty)
            {
                restartCaptureState();
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
                if (!aiUnit_.hasFlag)
                {
                    StopAllCoroutines();
                    StartCoroutine(aiUnit_.GetComponent<AIUnit>().capturing());
                }else
                {
                    subState = SubState.DeliverFlag;
                }

            break;

            case SubState.Empty:

                Debug.Log("Is in empty Capture");
                StopAllCoroutines();
                aiUnit_.path_.Clear();
                //restartCaptureState();

            break;


            case SubState.DeliverFlag:
               
                if (!aiUnit_.isDelivering)
                {
                    aiUnit_.path_.Clear();

                    Node startNode = aiUnit_.getClosestNode();
                    aiUnit_.getNewPath(startNode, aiUnit_.deliveryNode);

                    if (aiUnit_.path_.Count > 0)
                    {
                        aiUnit_.isDelivering = true;
                        StartCoroutine(aiUnit_.GetComponent<AIUnit>().deliverFlag());
                    }
                }

            break;

            case SubState.Fighting:

                if (!aiUnit_.hasFlag && aiUnit_.isFighting == false)
                {
                    //aiUnit_.isFighting = true;
                    StartCoroutine(aiUnit_.GetComponent<AIUnit>().fight());
                }

            break;

        }

    }

    //Exit the current state, resetting data that needs to be
    bool exitState(SubState oldState)
    {
        switch (oldState)
        {
            case SubState.Capturing:
                aiUnit_.path_.Clear();
                aiUnit_.isCapturing = false;

            break;

            case SubState.DeliverFlag:
                //aiUnit_.path_.Clear();
                aiUnit_.isDelivering = false;

            break;

            case SubState.Fighting:
                //aiUnit_.path_.Clear();
                aiUnit_.isFighting = false;

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
        if (other != null)
        {
            //Check if the AIunit is within range of target flag
            if (other.gameObject.name == "RedFlag" && aiUnit_.team_ != AIUnit.Team.Red)
            {
                subState = SubState.DeliverFlag;
                aiUnit_.hasFlag = true;
            }
            if (other.gameObject.name == "BlueFlag" && aiUnit_.team_ != AIUnit.Team.Blue)
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

}

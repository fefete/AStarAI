using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendState : MonoBehaviour {

    public AISight aiSight_;
    private AIUnit aiUnit_;
    public GameObject enemyUnitTarget_;

    public List<Node> patrolNodes;

    public enum SubState
    {
        //
        // Cover - Cover flag carrier
        // Retrieve - Kill enemy flag Carrier (get your team flag back)
        // Fighting - Currently in combat
        //
        Patrol, Retrieve, Fighting, Chase
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
            //exitState(subState_)
            subState_ = value;
            changeState(subState_);
        }
    }

    // Use this for initialization
    void Start () {

        aiUnit_ = this.GetComponent<AIUnit>();
        aiSight_ = aiSight_.GetComponent<AISight>();

        //Find the closest node to the unit, get all the other closer nodes
        Node startNode = aiUnit_.AStarManager_.getNearestNode(this.transform);
        GameObject startNodeObj = GameObject.Find(startNode.name);
        patrolNodes.Add(startNode);

        int numOfPatrolNodes = 4;
        if(startNode.accessibleNodes.Count < 4)
        {
            numOfPatrolNodes = startNode.accessibleNodes.Count;
        }

        // add the close node to the list
        for(int i = patrolNodes.Count; i < numOfPatrolNodes; i++)
        {
            GameObject nodeObj = GameObject.Find(startNode.accessibleNodes[i].name);
            float dist = Vector3.Distance(startNodeObj.transform.position, nodeObj.transform.position);
            if (dist < 50.0f)
            {
                patrolNodes.Add(startNode.accessibleNodes[i]);
            }

        }



        if(patrolNodes.Count != 0)
        {
            subState = SubState.Patrol;
        }

    }
	
	// Update is called once per frame
	void Update () {

        if (aiUnit_.isAlive)
        {
            if (aiUnit_.didRespawn)
            {
                aiUnit_.didRespawn = false;
            }
            if (aiSight_.hasTarget)
            {
                enemyUnitTarget_ = aiSight_.GetTarget();

                float distToTarget = Vector3.Distance(enemyUnitTarget_.transform.position, transform.position);

                if (distToTarget <= aiUnit_.shootRange)
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

    void changeState(SubState newState)
    {
        switch (newState)
        {
            case SubState.Patrol:

                StartCoroutine(aiUnit_.GetComponent<AIUnit>().patrol(patrolNodes));

            break;

            case SubState.Retrieve:
                //TODO: Add Decision Making Here
                // If NOT NEAR enemy flag, goto and recover team flag
                // If NEAR go for flag
                // If seen flag carrier goto and kill
                // If alerted (messaged) goto and kill

                Debug.Log("Retrive Flag");

            break;

            case SubState.Fighting:
                Debug.Log("Defend: Fighting");
                if (aiUnit_.health <= 40)
                {
                    StartCoroutine(aiUnit_.GetComponent<AIUnit>().flee(aiUnit_.recoveryBay.transform));
                }
                break;

            case SubState.Chase:

                //StartCoroutine(aiUnit_.GetComponent<AIUnit>().chase(startOfChasePoint, enemyUnitTarget_));

            break;


        }

    }

}

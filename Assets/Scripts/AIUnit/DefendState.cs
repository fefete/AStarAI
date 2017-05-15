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
        Patrol, Empty, Fighting, Chase
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
            if (exitState(subState_))
            {
                subState_ = value;
                changeState(subState_);
            }
        }
    }

    // Use this for initialization
    void Start () {

        aiUnit_ = this.GetComponent<AIUnit>();
        aiSight_ = aiSight_.GetComponent<AISight>();

        subState = SubState.Empty;

    }
	
	// Update is called once per frame
	void Update () {

        if (aiUnit_.isAlive)
        {
            if (aiUnit_.didRespawn)
            {
                aiUnit_.didRespawn = false;
                restartDefendState();
            }
            if (subState == SubState.Empty)
            {
                restartDefendState();
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

            case SubState.Empty:

                Debug.Log("Is in empty State: Defend");
                StopAllCoroutines();
                aiUnit_.path_.Clear();

            break;

            case SubState.Fighting:

                if (aiUnit_.isFighting == false)
                {
                    aiUnit_.path_.Clear();
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
            case SubState.Patrol:

            break;


            case SubState.Fighting:
                //aiUnit_.path_.Clear();
                aiUnit_.isFighting = false;

            break;

        }

        return true;
    }

    void restartDefendState()
    {
        patrolNodes.Clear();
        //Find the closest node to the unit, get all the other closer nodes
        Node startNode = aiUnit_.AStarManager_.getNearestNode(this.transform);
        GameObject startNodeObj = GameObject.Find(startNode.name);
        patrolNodes.Add(startNode);

        int numOfPatrolNodes = 4;
        if (startNode.accessibleNodes.Count < 4)
        {
            numOfPatrolNodes = startNode.accessibleNodes.Count;
        }

        // add the close node to the list
        for (int i = patrolNodes.Count; i < numOfPatrolNodes; i++)
        {
            GameObject nodeObj = GameObject.Find(startNode.accessibleNodes[i].name);
            float dist = Vector3.Distance(startNodeObj.transform.position, nodeObj.transform.position);
            if (dist < 50.0f)
            {
                patrolNodes.Add(startNode.accessibleNodes[i]);
            }

        }

        if (patrolNodes.Count != 0)
        {
            subState = SubState.Patrol;
        }
    }

}

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
        CoverFlagCarrier, Retrieve, Fighting, Chase, Retreat
    }

    //Set State Variables
    public SubState subState { get; set; }

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

        if (aiSight_.hasTarget)
        {
            enemyUnitTarget_ = aiSight_.GetTarget();

            float distToTarget = Vector3.Distance(enemyUnitTarget_.transform.position, transform.position);

            if (distToTarget < 45.0f)
            {
                //enemyUnitTarget_ = aiSight_.GetTarget();
                aiUnit_.shoot(enemyUnitTarget_);
                subState = SubState.Fighting;
            }
        }else
        {
            enemyUnitTarget_ = null;
        }

        switch (subState)
        {
            case SubState.Retrieve:
                //TODO: Add Decision Making Here
                // If NOT NEAR enemy flag, goto and recover team flag
                // If NEAR go for flag
                // If seen flag carrier goto and kill
                // If alerted (messaged) goto and kill

                Debug.Log("Retrive Flag");

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

                if(aiUnit_.health < 20)
                {
                    aiUnit_.flee();
                }

                if(aiUnit_.health > 50)  // enemy out of sight / retreating
                {
                    startOfChasePoint = new Vector3(-8.37f, 1.141f, 0.0f); // Find Nearest Node and use x,y,z coords as return point
                    subState = SubState.Chase;
                }

                switch (subState)
                {
                    case SubState.Chase:

                        float distFromBeginChase = Vector3.Distance(transform.position, startOfChasePoint);

                        if (distFromBeginChase < aiUnit_.maxChaseDistance && retreating_ == false)
                        {
                            float dist  = Vector3.Distance(enemyUnitTarget_.transform.position, transform.position);

                            //Stop moving/chasing when close enough to the enemy 
                            if (dist > 5.0f)
                            {
                                  transform.position = Vector3.MoveTowards(transform.position, enemyUnitTarget_.transform.position, 0.1f);
                            }
                        }
                        else
                        {
                            retreating_ = true;
                            Debug.Log("Cant chase anymore, should go back to start point");
                            transform.position = Vector3.MoveTowards(transform.position, startOfChasePoint, 0.1f);

                            if(transform.position == startOfChasePoint)
                            {
                                retreating_ = false;
                            }

                        }
                        break;
                }

            break;

        }

    }

}

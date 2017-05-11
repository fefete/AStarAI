using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour {

    public enum Team { Red, Blue };

    public GameObject enemyFlagCarrier;
    public Team team;
    
    public GameObject[] teamMembers;

	// Use this for initialization
	void Start () {

        enemyFlagCarrier = null;

        if(team == Team.Blue)
        {
            teamMembers = GameObject.FindGameObjectsWithTag("BlueTeam");
        }
        else
        {
            teamMembers = GameObject.FindGameObjectsWithTag("RedTeam");
        }

    }

    public GameObject findClosestAlly(GameObject unit)
    {
        GameObject closest = null;
        float minDist = Mathf.Infinity;
        foreach(GameObject a in teamMembers)
        {
            if(a.name != unit.name)
            {
                float dist = Vector3.Distance(unit.transform.position, a.transform.position);
                if(dist < minDist)
                {
                    closest = a;
                    minDist = dist;
                }
            }
        }
        return closest;
    }

}

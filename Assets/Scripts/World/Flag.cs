using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour {

    public WorldManager worldManager;

    public GameObject flagPole;
    public GameObject flag;
    public GameObject light;

    public bool isCaptured = false;

    public enum Team
    {
        Red, Blue
    }
    public Team team;
	// Use this for initialization
	void Start () {
		
        if(team == Team.Red)
        {
            Renderer rend = flag.GetComponent<Renderer>();
            rend.material.color = Color.red;

            rend = flagPole.GetComponent<Renderer>();
            rend.material.color = Color.red;

            Light l = light.GetComponent<Light>();
            l.color = Color.red;

        }
        if (team == Team.Blue)
        {
            Renderer rend = flag.GetComponent<Renderer>();
            rend.material.color = Color.blue;

            rend = flagPole.GetComponent<Renderer>();
            rend.material.color = Color.blue;

            Light l = light.GetComponent<Light>();
            l.color = Color.blue;

        }

    }
	
	// Update is called once per frame
	void Update () {
		
        if(isCaptured == true)
        {
            GetComponent<SphereCollider>().enabled = false;
        }
        else
        {
            GetComponent<SphereCollider>().enabled = true;
        }

	}


    private void OnTriggerEnter(Collider other)
    {
        if (isCaptured == false)
        {
            if (team == Team.Red)
            {
                if (other.gameObject.tag == "BlueTeam")
                {
                    worldManager.RT_FlagCaptured = true;
                    worldManager.blueTeamFlagCarrier = other.gameObject;
                    isCaptured = true;
                }

            }
            else
            {
                if (other.gameObject.tag == "RedTeam")
                {
                    worldManager.BT_FlagCaptured = true;
                    worldManager.redTeamFlagCarrier = other.gameObject;
                    isCaptured = true;
                }
            }
        }
    }

}

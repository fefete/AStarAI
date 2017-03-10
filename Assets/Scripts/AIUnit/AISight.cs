using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISight : MonoBehaviour {

    // Public Variables
    public GameObject[] enemies;
    public GameObject pointOfVision;

    public float fieldOfViewAngle;

    // Private Variables
    private List<GameObject> enemieInView;
    string enemyTag;
    private bool hasTarget;

    // Use this for initialization
    void Start()
    {

        enemieInView = new List<GameObject>();

        if (enemies.Length == 0)
        {
            //
            // Check to see which team this AI Unit is Affilated with and,
            // Fill the Enemies array with units from the other team
            //
            if (gameObject.tag == "BlueTeam")
            {
                enemies = GameObject.FindGameObjectsWithTag("RedTeam");
                enemyTag = "RedTeam";
            }
            else
            {
                enemies = GameObject.FindGameObjectsWithTag("BlueTeam");
                enemyTag = "BlueTeam";
            }
        }

    }
	
	// Update is called once per frame
	void Update () {

        //Debug.DrawLine(pointOfVision.transform.position, pointOfVision.transform.forward * 10, Color.red);
        if(enemieInView.Count > 0)
        {
            // Look at the closest target 
            GameObject target = FindClosestObject(enemieInView);
   
            gameObject.transform.LookAt(target.transform);
            hasTarget = true;
        }

        Debug.Log(enemieInView.Count);

        DetectEnemies();
	}

    void DetectEnemies()
    {

        foreach(GameObject enemy in enemies)
        {

            Vector3 direction = enemy.transform.position - pointOfVision.transform.position;

            // Determin if the Enemy is in view 
            if (Vector3.Dot(pointOfVision.transform.forward, direction) > Mathf.Cos(fieldOfViewAngle / 2))
            {
                // Now Send a ray to check for any obsticales
                RaycastHit rayHit;

                if(Physics.Raycast(pointOfVision.transform.position, direction, out rayHit, 500))
                {
                    if(rayHit.transform.tag == enemyTag)
                    {
                        GameObject enemyToAdd = GameObject.Find(enemy.name);
                        if (!enemieInView.Contains(enemyToAdd))
                        {
                            enemieInView.Add(enemyToAdd);
                        }

                    }
                    else
                    {
                        //
                        // Check to see in an enemy in the 'enemiesInView' List is still in view.
                        // if not remove it
                        //
                        GameObject enemyToRemove = GameObject.Find(enemy.name);
                        if (enemieInView.Contains(enemyToRemove))
                        {
                            enemieInView.Remove(enemyToRemove);
                        }
                    }
                }

                Debug.DrawRay(pointOfVision.transform.position, direction, Color.red);

            }else
            {
                //
                // Check to see in an enemy in the 'enemiesInView' List is still in view,
                // if not remove it
                //
                GameObject enemyToRemove = GameObject.Find(enemy.name);
                if (enemieInView.Contains(enemyToRemove))
                {
                    enemieInView.Remove(enemyToRemove);
                }
            }

        }

    }

    GameObject FindClosestObject(List<GameObject> objs)
    {
        GameObject closest = null;

        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach(GameObject obj in objs)
        {
            float dist = Vector3.Distance(obj.transform.position, currentPos);
            if(dist < minDist)
            {
                closest = obj;
                minDist = dist;
            }

        }

        return closest;
    }

}

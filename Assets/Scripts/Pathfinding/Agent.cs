using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    public Node ActualNode;
    public Node FinalNode;
    public Node origin;

    public Mind mind;
    public Body body;

    public Sensor sensor;

    private Vector2[] path;

    // Use this for initialization
    void Start () {
        //AStarManager.getInstance().calculatePath(ActualNode, FinalNode, out path);
	}
	
	// Update is called once per frame
	void Update () {
	}
}

/*
 * move with dotween
 * 
 transform.DOKill(false);
      transform.DOMove(pos, distance / speed).SetEase(Ease.Linear).SetUpdate(false).OnComplete(
        () =>
        {
          m_IsMoving = false;
          m_ReachedObjective = true;
          ThrowOnObjectiveChange(m_Objective);
        }
      );
     
     */

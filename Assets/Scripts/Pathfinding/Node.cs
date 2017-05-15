using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 Physical representation of the node in the world, when it
 starts it passes the data to the NodeData variable.
     
     */

public class Node : MonoBehaviour {

    public List<Node> accessibleNodes;
    public NodeData data;
    public float G;
    public float H;
    public float F;
    public float cost_to_arrive;

    public void Start()
    {
        data = new NodeData();
        data.G = G;
        data.H = H;
        data.F = F;
        data.name = gameObject.name;
        data.node = this;
        data.CameFrom = null;
    }

}

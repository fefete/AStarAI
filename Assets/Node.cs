using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    public Node[] accessibleNodes;
    public NodeData data;
    public float G;
    public float H;
    public float F;
    public float cost_to_arrive;

    public void Awake()
    {
        data = new NodeData();
        data.x = transform.position.x;
        data.y = transform.position.y;
        data.z = transform.position.z;
        data.G = G;
        data.H = H;
        data.F = F;
        data.parent = this;
    }

}

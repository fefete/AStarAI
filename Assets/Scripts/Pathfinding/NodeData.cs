using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData {

    public float G;
    public float H;
    public float F;
    //public float x;
    //public float y;
    //public float z;
    public float cost_to_arrive;
    public string name;
    public Node node;
    public NodeData CameFrom;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 
    Logical representation for a node, it stores the variables needed for 
    the A* path calculation
     
     */

public class NodeData {

    public float G;
    public float H;
    public float F;
    public float cost_to_arrive;
    public string name;
    public Node node;
    public NodeData CameFrom;
}

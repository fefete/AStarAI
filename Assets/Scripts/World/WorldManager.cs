using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour {

    // Boolean varible to determin if the Blue Team Flag has been captured
    public bool BT_FlagCaptured { get; set; }

    // Boolean varible to determin if the Blue Team Flag has been captured
    public bool RT_FlagCaptured { get; set; }

    // Boolean Variable to determin if the captured flag was successfully deliverd
    // **RF == RedFlag == Blue Successfully Captured and Deliverd Red Teams Flag**
    public bool RF_CapturedAndDelivered { get; set; }
    public bool BF_CapturedAndDelivered { get; set; }

    public GameObject blueFlag;
    public GameObject redFlag;

    public Flag bf_script;
    public Flag rf_script;

    private Vector3 bf_basePos;
    private Vector3 rf_basePos;

    public GameObject blueTeamFlagCarrier { get; set; }
    public GameObject redTeamFlagCarrier { get; set; }

    public Text RedScore;
    public Text BlueScore;

    //Team Score Variables
    private int RT_Score = 0;
    private int BT_Score = 0;

    // Use this for initialization
    void Start () {

        // Set the flag captured values to false on game start
        BT_FlagCaptured = false;
        RT_FlagCaptured = false;

        RF_CapturedAndDelivered = false;
        BF_CapturedAndDelivered = false;

        bf_basePos = blueFlag.transform.position;
        rf_basePos = redFlag.transform.position;

        bf_script = blueFlag.GetComponent<Flag>();
        rf_script = redFlag.GetComponent<Flag>();

    }
	
	// Update is called once per frame
	void Update () {

        if(RT_FlagCaptured == true && blueTeamFlagCarrier.GetComponent<AIUnit>().isAlive)
        {
            redFlag.transform.position = blueTeamFlagCarrier.transform.position;
        }
        else
        {
            redFlag.transform.position = rf_basePos;
            RT_FlagCaptured = false;
        }
        if(BT_FlagCaptured == true && redTeamFlagCarrier.GetComponent<AIUnit>().isAlive)
        {
            blueFlag.transform.position = redTeamFlagCarrier.transform.position;
        }
        else
        {
            blueFlag.transform.position = bf_basePos;
            BT_FlagCaptured = false;
        }

        if(RF_CapturedAndDelivered == true)
        {
            RT_FlagCaptured = false;
            rf_script.isCaptured = false;
            BT_Score++;
            redFlag.transform.position = rf_basePos;
            RF_CapturedAndDelivered = false;
        }
        if (BF_CapturedAndDelivered == true)
        {
            BT_FlagCaptured = false;
            bf_script.isCaptured = false;
            RT_Score++;
            blueFlag.transform.position = bf_basePos;
            BF_CapturedAndDelivered = false;
        }

        RedScore.text = RT_Score.ToString();
        BlueScore.text = BT_Score.ToString();


    }
}

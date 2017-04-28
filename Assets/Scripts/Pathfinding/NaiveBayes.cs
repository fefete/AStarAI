using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NaiveBayes : MonoBehaviour
{
    Dictionary<bool, List<List<int>>> classMap;
    List<string> listA = new List<string>();
    struct Data
    {
        //Win or lose
        bool outputClass { get; set; } //Win or loose
        //Three paths, mid, left and right
        int path { get; set; } //id of the path
        // + hp - speed ¦ + speed - hp
        int agent_class { get; set; } //class A or B
        //remaining hp when flag picked
        int hp_remaining { get; set; } // healthy or damaged 50% threshold

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Classify()
    {

    }

    void separateData(List<string> l)
    {
        bool win;
        foreach (string s in l)
        {
            string[] splitted = s.Split(',');
            List<int> data = new List<int>();
            //path number
            int pathID = int.Parse(splitted[1]);
            data.Add(pathID);
            //agent class 0 fast / 1 heavy
            int agentClass = int.Parse(splitted[2]);
            data.Add(agentClass);
            int hpRemaining = int.Parse(splitted[3]);
            if (agentClass == 0)
            {
                if (hpRemaining >= 50)
                {
                    data.Add(1);
                }
                else
                {
                    data.Add(0);
                }
            }
            else
            {
                if (hpRemaining*0.5 >= 50)
                {
                    data.Add(1);
                }
                else
                {
                    data.Add(0);
                }
            }

            if (splitted[0].Equals("win"))
            {
                win = true;
            }
            else
            {
                win = false;
            }
            classMap[win].Add(data);
        }
    }

    void readCSV(string path)
    {

        using (var fs = File.OpenRead(path))
        using (var reader = new StreamReader(fs))
        {

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                listA.Add(line);
            }
        }

    }

}

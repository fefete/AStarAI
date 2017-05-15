using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NaiveBayes : MonoBehaviour
{
    Dictionary<bool, List<List<int>>> classMap;
    List<string> listA = new List<string>();

    //data for class
    float NumberOfWins;
    float NumberOfLoses;

    //data for agent class
    float countClassA;
    float countClassB;
    float meanClassA;
    float meanClassB;

    float countHealty;
    float countUnhealthy;
    float meanHealty;
    float meanUnhealthy;

    float countPathA;
    float countPathB;
    float countPathC;
    float meanPathA;
    float meanPathB;
    float meanPathC;

    float finalValue;

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
        classMap = new Dictionary<bool, List<List<int>>>();
        classMap[true] = new List<List<int>>();
        classMap[false] = new List<List<int>>();
        string potato = Application.dataPath + "/datatest.csv";
        readCSV(potato);
        separateData(listA);
        getMeans();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public string Classify(int classtype)
    {
        float valuePathA = 0;
        float valuePathB = 0;
        float valuePathC = 0;

        if (classtype == 0)
        {
            //p(win ¦ pathA, class0, Healty)
            valuePathA = meanPathA + meanClassA + meanHealty;

            //p(win ¦ pathB, class0, Healty)
            valuePathB = meanPathB + meanClassA + meanHealty;

            //p(win ¦ pathC, class0, Healty)
            valuePathC = meanPathC + meanClassA + meanHealty; ;
        }
        else
        {
            //p(win ¦ pathA, class1, Healty)
            valuePathA = meanPathA + meanClassB + meanHealty;

            //p(win ¦ pathB, class1, Healty)
            valuePathB = meanPathB + meanClassB + meanHealty;

            //p(win ¦ pathC, class1, Healty)
            valuePathC = meanPathC + meanClassB + meanHealty;
        }

        string path = "A";
        finalValue = valuePathA;
        if (finalValue < valuePathB)
        {
            finalValue = valuePathB;
            path = "B";

        }
        if (finalValue < valuePathC)
        {
            finalValue = valuePathC;
            path = "C";

        }


        string final = "Best path is " + path + " with a probability of " + finalValue + "\n" + "The values where: \n A:" + valuePathA + "\n" + "B:" + valuePathB + "\n" + "C:" + valuePathC + "\n";
        return final;
    }

    void getMeans()
    {
        NumberOfWins = classMap[true].Count;
        NumberOfWins = classMap[false].Count;

        foreach (List<int> list in classMap[true])
        {
            //path count
            if (list[0] == 0)
            {
                countPathA++;
            }
            else if (list[0] == 1)
            {
                countPathB++;
            }
            else if (list[0] == 2)
            {
                countPathC++;
            }

            //class count
            if (list[1] == 0)
            {
                countClassA++;
            }
            else if (list[1] == 1)
            {
                countClassB++;
            }

            //healthy count
            if (list[2] == 1)
            {
                countHealty++;
            }
            else if (list[2] == 0)
            {
                countUnhealthy++;
            }
        }

        meanClassA = countClassA / (NumberOfWins);
        meanClassB = countClassB / (NumberOfWins);

        meanHealty = countHealty / (NumberOfWins);
        meanUnhealthy = countUnhealthy / (NumberOfWins);

        meanPathA = countPathA / (NumberOfWins);
        meanPathB = countPathB / (NumberOfWins);
        meanPathC = countPathC / (NumberOfWins);

    }
    void separateData(List<string> l)
    {
        bool win;
        foreach (string s in l)
        {
            string[] splitted = s.Split(';');
            List<int> data = new List<int>();
            //path number
            int pathID = int.Parse(splitted[1]);
            data.Add(pathID);
            //agent class 0 fast / 1 heavy
            int agentClass = int.Parse(splitted[2]);
            data.Add(agentClass);
            int hpRemaining = int.Parse(splitted[3]);
            // 1 = healthy
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
                if (hpRemaining * 0.5 >= 50)
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

        FileStream fs = File.OpenRead(path);
        StreamReader reader = new StreamReader(fs);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            listA.Add(line);
        }


    }

}
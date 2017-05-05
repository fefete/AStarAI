using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NaiveBayes : MonoBehaviour
{
    Dictionary<bool, List<List<int>>> classMap;
    List<string> listA = new List<string>();

    //data for class
    int NumberOfWins;
    int NumberOfLoses;

    //data for agent class
    int countClassA;
    int countClassB;
    float meanClassA;
    float meanClassB;

    int countHealty;
    int countUnhealthy;
    float meanHealty;
    float meanUnhealthy;

    int countPathA;
    int countPathB;
    int countPathC;
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

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Classify(int classtype)
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
        else {
            //p(win ¦ pathA, class1, Healty)
            valuePathA = meanPathA + meanClassB + meanHealty;

            //p(win ¦ pathB, class1, Healty)
            valuePathB = meanPathB + meanClassB + meanHealty;

            //p(win ¦ pathC, class1, Healty)
            valuePathC = meanPathC + meanClassB + meanHealty;
        }

        finalValue = valuePathA;
        if (finalValue < valuePathB) finalValue = valuePathB;
        if (finalValue < valuePathC) finalValue = valuePathC;

    }

    void getMeans()
    {
        NumberOfWins = classMap[true].Count();
        NumberOfWins = classMap[false].Count();

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
            if (list[2] == 0)
            {
                countHealty++;
            }
            else if (list[2] == 1)
            {
                countUnhealthy++;
            }
        }

        meanClassA = countClassA / (NumberOfWins);
        meanClassB = countClassB / (NumberOfWins);

        meanHealty = countHealty / (NumberOfWins);
        meanUnhealthy = countUnHealty / (NumberOfWins);

        meanPathA = countPathA / (NumberOfWins);
        meanPathB = countPathB / (NumberOfWins);
        meanPathC = countPathC / (NumberOfWins);

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

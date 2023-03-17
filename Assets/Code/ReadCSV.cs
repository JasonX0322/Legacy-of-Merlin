using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ReadCSV : MonoBehaviour
{
    List<string[]> card;
    List<string[]> skill;

    void Awake()
    {
        card = Read("card");
        //skill = Read("skill");
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(card[0][0]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    List<string[]> Read(string name)
    {
        string path = Application.streamingAssetsPath + "/" + name + ".csv";
        List<string[]> csvData = new List<string[]>();
        StreamReader sr = new StreamReader(path);
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            string[] rowData = line.Split(',');
            csvData.Add(rowData);
        }
        sr.Close();
        return csvData;
    }
}

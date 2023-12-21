using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SatelliteManager : MonoBehaviour
{
    public GameObject satellitePrefab;
    public int csvToLoad;
    private TextAsset[] allCsvs;
    // Start is called before the first frame update
    void Start()
    {
        allCsvs = Resources.LoadAll<TextAsset>("Csv");
        DisplayFromCsv(csvToLoad);
    }


    private void DisplayFromCsv(int number){

        string csv = allCsvs[number].text;

        string[] lines = csv.Split('\n');

        lines = lines.Skip(1).ToArray();    

        foreach(string l in lines){
            string[] positions = l.Split(',');

            GameObject satellite = Instantiate(satellitePrefab,transform);
            
            satellite.transform.position = new Vector3(float.Parse(positions[1])/5000,float.Parse(positions[2])/5000,float.Parse(positions[3])/5000);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

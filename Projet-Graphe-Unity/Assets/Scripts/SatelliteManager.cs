using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SatelliteManager : MonoBehaviour
{
    public GameObject satellitePrefab;
    public int csvToLoad;
    public int ratio;
    private TextAsset[] allCsvs;
    [SerializeField] private bool parseWithComma;


    private List<GameObject> satellites;

    private Graph<GameObject> graph;


    public float treshold;
    public LineRenderer satelliteLink;

    public bool calculate = false;
    public bool hideEdges = false;
    public bool showEdges = false;

    [SerializeField] private Gradient degreeGradient;

    private Vector3 averagePosition;


    // DEBUG
    public bool clearall = false;

    
    private void Awake(){

    }

    // Start is called before the first frame update
    void Start()
    {
        satellites = new List<GameObject>();
        allCsvs = Resources.LoadAll<TextAsset>("Csv");
        csvToLoad = 0;
        treshold = 20;
        DisplayFromCsv(csvToLoad);
        ConstructGraph();
    }

    private void Update()
    {
        // if (calculate)
        // {
        //     ConstructGraph();
        //     calculate = false;
        // }
        // if (hideEdges)
        // {
        //     graph.HideLinks();
        //     hideEdges = false;
        // }
        // if (showEdges)
        // {
        //     graph.ShowLinks();
        //     showEdges = false;
        // }
    }

    public void SetTreshold(int dropDownValue){
        switch(dropDownValue){
            case 0 : 
                treshold = 20;
                break;
            case 1 :
                treshold = 40;
                break;
            case 2 : 
                treshold = 60;
                break;
        }
    }


    public void ClearAll()
    {
        foreach (GameObject s in satellites) Destroy(s);
        satellites.Clear();
    }

    public void DisplayFromCsv(int number)
    {
        CameraManager camera = Camera.main.GetComponent<CameraManager>();
        camera.transform.position = new Vector3(0,0,0);
        camera.transform.rotation = Quaternion.identity;
        averagePosition = new Vector3(0,0,0);

        string csv = allCsvs[number].text;

        string[] lines = csv.Split('\n');

        lines = lines.Skip(1).ToArray();

        foreach (string l in lines)
        {
            string[] positions = l.Split(',');

            GameObject satellite = Instantiate(satellitePrefab, transform);
            satellites.Add(satellite);

            if (parseWithComma) {
                for (int i = 1; i <= 3; i++)
                {
                    string[] subs = positions[i].Split(".");
                    positions[i] = subs[0] + "," + subs[1];
                }
            }

            Vector3 pos = ParsePositions(positions);
            averagePosition += pos;
            satellite.transform.position  = pos;
        }
        averagePosition /= lines.Length;
        camera.SetAveragePos(averagePosition);
    }

    private bool CompareWithTreshHold(GameObject s1, GameObject s2)
    {
        return Vector3.Distance(s1.transform.position, s2.transform.position) < treshold * 1000 / ratio; 
    }


    public void ConstructGraph()
    {
        Graph<GameObject>.GetGraph(
            ref graph, CompareWithTreshHold, new ColoredVertexCreator(degreeGradient), new LineRendererEdgeCreator(satelliteLink), satellites.ToArray());
    }

    private Vector3 ParsePositions(string[] line){
        string[] positions = line.Skip(1).ToArray();
        Vector3 pos = new();

        for(int i=0; i < positions.Length; i++){
            pos[i] = float.Parse(positions[i])/ratio;
        }
        return pos;
    }
}




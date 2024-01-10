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


    private List<GameObject> satellites;

    private Graph<GameObject> graph;


    public float treshold;
    public LineRenderer satelliteLink;

    public bool calculate = false;
    public bool hideEdges = false;
    public bool showEdges = false;

    [SerializeField] private Gradient degreeGradient;

    private Vector3 averagePosition;
  


    private void Awake(){
        
    }

    // Start is called before the first frame update
    void Start()
    {
        allCsvs = Resources.LoadAll<TextAsset>("Csv");
        satellites = new List<GameObject>();
        DisplayFromCsv(csvToLoad);
    }

    private void Update()
    {
        if (calculate)
        {
            ConstructGraph();
            calculate = false;
        }
        if (hideEdges)
        {
            graph.HideLinks();
            hideEdges = false;
        }
        if (showEdges)
        {
            graph.ShowLinks();
            showEdges = false;
        }
    }


    public void ClearAll()
    {
        foreach (GameObject s in satellites) Destroy(s);
        satellites.Clear();
    }

    public void DisplayFromCsv(int number)
    {

        string csv = allCsvs[number].text;

        string[] lines = csv.Split('\n');

        lines = lines.Skip(1).ToArray();

        foreach (string l in lines)
        {
            string[] positions = l.Split(',');

            GameObject satellite = Instantiate(satellitePrefab, transform);
            satellites.Add(satellite);

            // for (int i = 1; i <= 3; i++)
            // {
            //     string[] subs = positions[i].Split(".");
            //     positions[i] = subs[0] + "," + subs[1];
            // }

            Vector3 pos = ParsePositions(positions);
            averagePosition += pos;
            satellite.transform.position  = pos;
        }
        averagePosition /= transform.childCount;
        Camera.main.GetComponent<CameraManager>().SetAveragePos(averagePosition);
    }

    private bool CompareWithTreshHold(GameObject s1, GameObject s2)
    {
        return Vector3.Distance(s1.transform.position, s2.transform.position) < treshold; 
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




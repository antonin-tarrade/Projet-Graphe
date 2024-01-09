using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SatelliteManager : MonoBehaviour
{
    public GameObject satellitePrefab;
    public int csvToLoad;
    private TextAsset[] allCsvs;


    private List<GameObject> satellites;

    private Graph<GameObject> graph;


    public float treshold;
    public LineRenderer satelliteLink;

    public bool calculate = false;
    public bool hideEdges = false;
    public bool showEdges = false;

    [SerializeField] private Gradient degreeGradient;
    private float minDegree;
    private float maxDegree;
  

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

            for (int i = 1; i <= 3; i++)
            {
                string[] subs = positions[i].Split(".");
                positions[i] = subs[0] + "," + subs[1];
            }


            satellite.transform.position = new Vector3(float.Parse(positions[1]) / 1000, float.Parse(positions[2]) / 1000, float.Parse(positions[3]) / 1000);
        }
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

}




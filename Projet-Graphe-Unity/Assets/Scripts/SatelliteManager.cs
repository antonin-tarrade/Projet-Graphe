using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ListWrapper<T>
{
    public ListWrapper(List<T> list)
    {
        myList = list;
    }
    public List<T> myList;
}

public class SatelliteManager : MonoBehaviour
{
    public GameObject satellitePrefab;
    public int csvToLoad;
    public int ratio;
    private TextAsset[] allCsvs;
    [SerializeField] private bool parseWithComma;

    public GameObject selectedSatellite;


    private List<GameObject> satellites;

    private Graph<GameObject> graph;
    public Action OnGraphChanged;


    public float treshold;
    public LineRenderer satelliteLink;
    public float edgeSelectionThickness;

    public bool calculate = false;
    public bool destroyComputer = false;

    [SerializeField] private Gradient degreeGradient;

    private Vector3 averagePosition;
    private CameraManager mainCamera;

    public static SatelliteManager instance;

    //DEBUG
    public List<ListWrapper<float>> distances;
    public List<ListWrapper<bool>> adjacence;

    // DEBUG
    public bool clearall = false;

    private void Awake(){
        instance = this;
        mainCamera = Camera.main.GetComponent<CameraManager>();
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
        
        if (selectedSatellite != null)
        {
            selectedSatellite.GetComponent<Satellite>().AddOutlines();
        }

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
        mainCamera.transform.SetPositionAndRotation(new Vector3(0,0,0), Quaternion.identity);
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
            satellite.transform.SetPositionAndRotation(pos, Quaternion.Euler(satellite.GetComponent<Satellite>().satelliteRotation));
        }
        averagePosition /= lines.Length;
        mainCamera.SetAveragePos(averagePosition);
    }

    private bool CompareWithTreshHold(GameObject s1, GameObject s2)
        => Vector3.Distance(s1.transform.position, s2.transform.position) < treshold * 1000 / ratio;

    private float Distance(GameObject s1, GameObject s2)
        => Vector3.Distance(s1.transform.position, s2.transform.position) * 1000 / ratio;

    private float SquaredDistance(GameObject s1, GameObject s2)
        => Mathf.Pow(Distance(s1,s2), 2);
 


    public void ConstructGraph()
    {
        Graph<GameObject>.GetGraph(
            ref graph, 
            CompareWithTreshHold, 
            new ColoredVertexCreator(degreeGradient),
            new WeightedEdgeCreator(Distance, new LineRendererEdgeCreator(satelliteLink)),
            satellites.ToArray());
        distances = new();
        adjacence = new();
        foreach (float[] tab in graph.shortestDistanceMatrix)
            distances.Add(new(new(tab)));
        foreach (bool[] tab in graph.adjacenceMatrix)
            adjacence.Add(new(new(tab)));
        OnGraphChanged?.Invoke();
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




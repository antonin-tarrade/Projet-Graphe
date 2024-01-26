using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class UIManager : MonoBehaviour
{

    public GameObject satteliteUIPrefab;
    public GameObject goBackButton;
    public GameObject infoButton;
    public GameObject squareButton;
    public TextMeshProUGUI distanceButton;


    public static UIManager instance;


    public GameObject graphInfo;

    public GameObject histogramValuePrefab;


    [SerializeField] private Vector2 uiPosition;


    private SatelliteManager satelliteManager;

    private GameObject displayedSatelliteUI;
    private GameObject displayedGraphUI;

    public bool isGraphUIDisplayed;





    private void Awake()
    {
        instance = this; 
    }


    // Start is called before the first frame update
    void Start()
    {
        satelliteManager = SatelliteManager.instance;
        satelliteManager.OnGraphChanged += UpdateUI;
        satelliteManager.OnGraphChanged += UpdateDistanceUI;
        goBackButton.SetActive(false);
        isGraphUIDisplayed = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateUI()
    {
        if (displayedSatelliteUI != null)
        {
            RemoveSatelliteUI();
            if (satelliteManager.selectedSatellite != null) DisplaySatelliteUI(satelliteManager.selectedSatellite.GetComponent<Satellite>());
        }
    }

    public void UpdateDistanceUI()
    {
       if (satelliteManager.selectedSatellites.Item1 != null && satelliteManager.selectedSatellites.Item2)
        {
            string n1 = satelliteManager.selectedSatellites.Item1.GetComponent<Satellite>().satelliteName;
            string n2 = satelliteManager.selectedSatellites.Item2.GetComponent<Satellite>().satelliteName;
            string dist = satelliteManager.distance.ToString();
            distanceButton.text = n1 + " -> " + n2 + " : " + dist;
        }
       else
        {
            distanceButton.text = "";
        }
    }

    public void ClearDistanceUI()
    {
        distanceButton.text = "";
    }

    public void DisplaySatelliteUI(Satellite sattelite)
    {
        GameObject UI = Instantiate(satteliteUIPrefab);
        UI.name = "Sattelite " +  sattelite.satelliteName;
        Transform UIBox = UI.transform.GetChild(1);
        Transform infoBox = UIBox.transform.GetChild(1);
        Transform descriptionBox = UIBox.transform.GetChild(2);

        infoBox.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = sattelite.satelliteName;
        infoBox.GetChild(1).GetComponentInChildren<RawImage>().texture = sattelite.img;

        descriptionBox.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "Degree : " + sattelite.degree;
        descriptionBox.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "Connex component size : " + sattelite.tailleComposanteConnexe;
        UI.transform.SetParent(goBackButton.transform.parent);
        UI.GetComponent<RectTransform>().anchoredPosition = uiPosition;
        displayedSatelliteUI = UI;
    }

    public void RemoveSatelliteUI()
    {
        Destroy(displayedSatelliteUI);
        displayedSatelliteUI = null;
    }

    public void ActivateReturnButton()
    {
        goBackButton.SetActive(true);
    }

    public void DesactivateReturnButton()
    {
        goBackButton.SetActive(false);
    }


    public void DisplayGraphUI()
    {
        GameObject graphUI = Instantiate(graphInfo);
        graphUI.transform.SetParent(goBackButton.transform.parent);

        graphUI.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 0f, 0f);
        Transform infoBox = graphUI.transform.GetChild(1).GetChild(1);
        Transform histBox = infoBox.transform.GetChild(0);
        Transform hist1 = histBox.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0);
        Transform hist2 = histBox.transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0);
        Transform hist3 = histBox.transform.GetChild(2).GetChild(1).GetChild(0).GetChild(0);
        Transform hist4 = histBox.transform.GetChild(3).GetChild(1).GetChild(0).GetChild(0);
        Transform meanBox = infoBox.transform.GetChild(1);

        Transform hist1s = histBox.transform.GetChild(0).GetChild(1);
        Transform hist2s = histBox.transform.GetChild(1).GetChild(1);
        Transform hist3s = histBox.transform.GetChild(2).GetChild(1);
        Transform hist4s = histBox.transform.GetChild(3).GetChild(1);


        meanBox.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Mean Degree : " + satelliteManager.graph.meanDegree;
        meanBox.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Mean Clustering Degree : " + satelliteManager.graph.meanClusterDegree;

        List<int> abs = new List<int>();
        for (int i = 0; i< 10; i++)
        {
            abs.Add(i*10);
        }

        Histogram h1 = new(abs);
        Dictionary<int, int> hdeg = h1.GenerateHistogram(satelliteManager.graph.degreeDistribution);
        Dictionary<int, int> hcon = h1.GenerateHistogram(satelliteManager.graph.connexComponentsDistribution);
        Dictionary<int, int> hclu = h1.GenerateHistogram(satelliteManager.graph.clusteringDegreeDistribution);

        List<int> abs2 = new();

        int multnumber = satelliteManager.squaredDistance ? 5000 : 100;

        for (int i = 0; i<= 10; i++)
        {
            abs2.Add(i*multnumber);
        }

        Histogram h2 = new(abs2);
        Transform absic = hist1s.GetChild(1);
        Transform ordc = hist1s.GetChild(0).GetChild(1);



        for(int i = 0; i < absic.childCount; i++){
            absic.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = abs2[i].ToString();
        }
        int n = satelliteManager.graph.order;
        List<float> distrib = new List<float>();
        for (int i = 0; i<n; i++){
            for (int j = i+1; j<n; j++){
                float dist = satelliteManager.graph.shortestDistanceMatrix[i][j];
                if ((dist != float.PositiveInfinity)) {
                    distrib.Add(dist);
                }
                
            } 
        }

        Dictionary<float, int> hsp = h2.GenerateHistogram(distrib.ToArray());

        List<Dictionary<int,int>> allDict = new()
        {
            hdeg,
            hcon,
            hclu
        };
        List<List<float>> allOrd = new ();

        List<int> allMax = new();

        List<Transform> allTrans = new(){
            hist3s.GetChild(0).GetChild(1),
            hist2s.GetChild(0).GetChild(1),
            hist4s.GetChild(0).GetChild(1)
        };

        foreach (Dictionary<int,int> d in allDict){
            List<float> l = new();
            int max = d.Values.Max();

            for (float i = 1; i <= 5; i++)
            {
                l.Add(i * max/5);
            }
            allMax.Add(max);
            allOrd.Add(l);
        }

        for(int i = 0 ; i< allTrans.Count; i++ ){
            for(int j = 0; j < allTrans[i].childCount; j++){
                allTrans[i].GetChild(j).GetComponentInChildren<TextMeshProUGUI>().text = allOrd[i][j].ToString();
            }
        }

        int maxOrd = hsp.Values.Max();
        List<int> ord = new();
        for (int i = 0; i<= 10; i++)
        {
            ord.Add(i * maxOrd/10);
        }

        for(int i = 0; i < ordc.childCount; i++){
            ordc.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = ord[i].ToString();
        }


        for (int i = 0;i< 10;i++)
        {
            GameObject hd = Instantiate(histogramValuePrefab,hist3);
            hd.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, hdeg.TryGetValue(i,out int value1) ? (float) value1 * 250 / allMax[0] : 0);

            GameObject hco = Instantiate(histogramValuePrefab, hist2);
            hco.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, hcon.TryGetValue(i, out int value2) ? (float) value2 * 250 / allMax[1] : 0);

            GameObject hs = Instantiate(histogramValuePrefab, hist1);
            hs.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, hsp.TryGetValue(i, out int value3) ? (float) value3 * 250 / maxOrd : 0);


            GameObject hcl = Instantiate(histogramValuePrefab, hist4);
            hcl.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, hclu.TryGetValue(i, out int value4) ? (float) value4 * 250 / allMax[2] : 0);


        }

        displayedGraphUI = graphUI;

    }


    public void RemoveGraphUI()
    {
        Destroy(displayedGraphUI);
        displayedGraphUI = null;
    }

    public void UpdateGraphUI()
    {
        RemoveGraphUI();
        DisplayGraphUI();
    }


    public void DisplayOrHIdeInfo()
    {
        if (!isGraphUIDisplayed)
        {
            isGraphUIDisplayed = true;
            DisplayGraphUI();
            infoButton.GetComponentInChildren<TextMeshProUGUI>().text = "Hide Infos";
        } else
        {
            isGraphUIDisplayed = false;
            RemoveGraphUI();
            infoButton.GetComponentInChildren<TextMeshProUGUI>().text = "Display Infos";
        }
    }



    public void ChangeSquare(){
        if (!satelliteManager.squaredDistance){
            satelliteManager.squaredDistance = true;
            squareButton.GetComponentInChildren<TextMeshProUGUI>().text = "Apply normal";
        } else {
            satelliteManager.squaredDistance = false;
            squareButton.GetComponentInChildren<TextMeshProUGUI>().text = "Apply Square";
        }
        satelliteManager.ConstructGraph();
    }


}

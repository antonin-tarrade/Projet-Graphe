using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UIManager : MonoBehaviour
{

    public GameObject satteliteUIPrefab;
    public GameObject goBackButton;
    public GameObject infoButton;


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
            DisplaySatelliteUI(satelliteManager.selectedSatellite.GetComponent<Satellite>());
        }
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
        Transform mainUI = graphUI.transform.GetChild(1);
        Transform infoBox = mainUI.transform.GetChild(1);
        Transform hist1 = infoBox.transform.GetChild(0).GetChild(1);
        Transform hist2 = infoBox.transform.GetChild(1).GetChild(1);
        Transform meanBox = infoBox.transform.GetChild(2);
        Transform hist3 = infoBox.transform.GetChild(3).GetChild(1);

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


        for (int i = 0;i< 10;i++)
        {
            GameObject hd = Instantiate(histogramValuePrefab,hist1);
            hd.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, (hdeg.TryGetValue(i,out int value1) ? value1*3 : 0));

            GameObject hco = Instantiate(histogramValuePrefab, hist2);
            hco.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, (hcon.TryGetValue(i, out int value2) ? value2 * 3 : 0));

            GameObject hcl = Instantiate(histogramValuePrefab, hist3);
            hcl.GetComponent<RectTransform>().sizeDelta = new Vector2(50f, (hclu.TryGetValue(i, out int value3) ? value3 * 3 : 0));
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


}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public GameObject satteliteUIPrefab;
    public GameObject goBackButton;

    public static UIManager instance;


    public GameObject graphInfo;

    public GameObject histogramPrefab;


    [SerializeField] private Vector2 uiPosition;


    private SatelliteManager satelliteManager;

    private GameObject displayedUI;




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
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void UpdateUI()
    {
        if (displayedUI != null)
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
        displayedUI = UI;
    }

    public void RemoveSatelliteUI()
    {
        Destroy(displayedUI);
        displayedUI = null;
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
        Transform hist1 = infoBox.transform.GetChild(0);
        Transform hist2 = infoBox.transform.GetChild(1);
        Transform meanBox = infoBox.transform.GetChild(2);
        Transform hist3 = infoBox.transform.GetChild(3);

        meanBox.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Mean Degree : " + satelliteManager.graph.meanDegree;
        meanBox.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Mean Clustering Degree : " + satelliteManager.graph.meanClusterDegree;

        Histogram h1 = new Histogram();

    }



}

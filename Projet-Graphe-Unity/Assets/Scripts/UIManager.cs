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
            RemoveUI();
            Display(satelliteManager.selectedSatellite.GetComponent<Satellite>());
        }
    }


    public void Display(Satellite sattelite)
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

    public void RemoveUI()
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

}

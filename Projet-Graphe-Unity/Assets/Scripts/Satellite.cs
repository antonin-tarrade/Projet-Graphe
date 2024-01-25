using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static System.Random;
using Unity.VisualScripting;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Satellite : MonoBehaviour
{

    public string satelliteName;
    public int degree;
    public int tailleComposanteConnexe;
    public Texture2D img;
    [SerializeField] private int nameLength;
    [SerializeField] private float zoomDistance;
    private CameraManager mainCamera;

    public static float cameraDistanceFactor = 0.01f;
    public static float offset = 0.85f;
    public static float minSize = 0.6f;
    public static float maxSize = 2.5f;

    public Vector3 satelliteRotation;


    private void Awake()
    {
       satelliteRotation = RandomRotation();
    }

    private string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789---";
        return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[(int) Random.Range(0f, s.Length)]).ToArray());
    }


    private Texture2D RandomImage(int num)
    {
        return Resources.Load<Texture2D>("Satellites/Satellite" + num);
    }

    private Vector3 RandomRotation() {
        return new Vector3(Random.Range(0f, 45f), Random.Range(0f, 45f), Random.Range(0f, 45f));
    }

    // Start is called before the first frame update
    void Start()
    {   mainCamera = Camera.main.GetComponent<CameraManager>();
        satelliteName = RandomString(nameLength);
        img = RandomImage((int)Random.Range(1f, 7f));
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = offset * Mathf.Clamp((Vector3.Distance(transform.position, Camera.main.transform.position)) * cameraDistanceFactor, minSize, maxSize) * Vector3.one ;
    }

    private void OnMouseEnter() {
        AddOutlines();

    }

    private void OnMouseExit() {
        RemoveOutlines();
    }


    private void OnMouseDown() {
        SatelliteManager.instance.selectedSatellite = gameObject;
        mainCamera.GoTo(transform);
    }

    public void AddOutlines()
    {
        GetComponent<MeshRenderer>().materials[1].SetFloat("_Float", SatelliteManager.instance.edgeSelectionThickness);
    }

    public void RemoveOutlines()
    {
        GetComponent<MeshRenderer>().materials[1].SetFloat("_Float", 0f);
    }
}

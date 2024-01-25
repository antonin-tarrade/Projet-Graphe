using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static System.Random;
using UnityEditor.ShaderGraph.Internal;
using Unity.VisualScripting;

public class Satellite : MonoBehaviour
{

    public string satelliteName;
    [SerializeField] private int nameLength;
    public int degree;
    [SerializeField] private float zoomDistance;
    private CameraManager mainCamera;

    public static float cameraDistanceFactor = 0.01f;
    public static float offset = 0.75f;
    public static float minSize = 0.5f;
    public static float maxSize = 2f;
    



    private string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[(int) Random.Range(0f, s.Length)]).ToArray());
    }

    // Start is called before the first frame update
    void Start()
    {   mainCamera = Camera.main.GetComponent<CameraManager>();
        satelliteName = RandomString(nameLength);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = offset * Mathf.Clamp((Vector3.Distance(transform.position, Camera.main.transform.position)) * cameraDistanceFactor, minSize, maxSize) * Vector3.one ;
    }

    private void OnMouseEnter() {
        Debug.Log(satelliteName);
        gameObject.GetComponent<MeshRenderer>().materials[1].SetFloat("_Float", SatelliteManager.instance.edgeSelectionThickness);

    }

    private void OnMouseExit() {
        gameObject.GetComponent<MeshRenderer>().materials[1].SetFloat("_Float",0f);
    }


    private void OnMouseDown() {
       mainCamera.GoTo(transform);
    }
}

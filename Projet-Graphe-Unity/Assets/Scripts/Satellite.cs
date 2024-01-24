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
        
    }

    private void OnMouseEnter() {
        Debug.Log(satelliteName);
        gameObject.GetComponent<MeshRenderer>().materials[1].SetFloat("_Float",1.2f);

    }

    private void OnMouseExit() {
        gameObject.GetComponent<MeshRenderer>().materials[1].SetFloat("_Float",1f);
    }


    private void OnMouseDown() {
       mainCamera.GoTo(transform);
    }
}

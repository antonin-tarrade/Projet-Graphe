using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static System.Random;
using UnityEditor.ShaderGraph.Internal;
using Unity.VisualScripting;
using UnityEngine.UI;

public class Satellite : MonoBehaviour
{

    public string satelliteName;
    public int degree;
    public int tailleComposanteConnexe;
    public Texture2D img;
    [SerializeField] private int nameLength;
    [SerializeField] private float zoomDistance;
    private CameraManager mainCamera;




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

    // Start is called before the first frame update
    void Start()
    {   mainCamera = Camera.main.GetComponent<CameraManager>();
        satelliteName = RandomString(nameLength);
        img = RandomImage((int)Random.Range(1f, 7f));
        Debug.Log(img);
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

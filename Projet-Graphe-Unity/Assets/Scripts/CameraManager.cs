using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraManager : MonoBehaviour
{
    private Vector3 center;
    public float speed;
    public float zoomRate;
    public float zoomDistance;
    public float rotationRate;
    private Vector3 spawnPosition;
    private SatelliteManager satelliteManager;
    private UIManager uiManager;

    private bool canMove;
    private bool canZoom;



    // Start is called before the first frame update
    void Start()
    {
        center = new();
        satelliteManager = SatelliteManager.instance;
        uiManager = UIManager.instance;
        spawnPosition = new();
        canMove = true;
        canZoom = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {


            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.RotateAround(center, transform.up, -Time.deltaTime * speed);
            }

            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.RotateAround(center, transform.right, Time.deltaTime * speed);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.RotateAround(center, transform.up, Time.deltaTime * speed);
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.RotateAround(center, transform.right, -Time.deltaTime * speed);
            }

            if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W))
            {
                transform.position += speed * Time.deltaTime * (transform.position - center).normalized;
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.position -= speed * Time.deltaTime * (transform.position - center).normalized;
            }

        }
    }

    public void SetAveragePos(Vector3 position){
        center = position;
        transform.position = center - new Vector3(0,0,100);
        spawnPosition = transform.position;

    }

    public void GoTo(Transform target){
        if (!canZoom) { return; }
        uiManager.DesactivateReturnButton();
        uiManager.RemoveUI();
        canMove = false;
        StartCoroutine(ZoomTo(target.position,target.rotation,true));
        satelliteManager.selectedSatellite = target.gameObject;
        
    }


    private IEnumerator ZoomTo(Vector3 targetPosition, Quaternion targetRotation,bool zoom){
        canZoom = false;
         while (Vector3.Distance(transform.position, targetPosition) > zoomDistance)
        {
            transform.SetPositionAndRotation(Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * zoomRate), Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationRate));
            yield return null;
        }
        if (zoom){
            uiManager.Display(satelliteManager.selectedSatellite.GetComponent<Satellite>());
            uiManager.ActivateReturnButton();
        } else {
            satelliteManager.selectedSatellite = null;
            uiManager.RemoveUI();
            uiManager.DesactivateReturnButton();
            canMove = true;

        }
        canZoom = true;
    }


    public void UnZoom(){
        canZoom = false;
        StartCoroutine(ZoomTo(spawnPosition,Quaternion.identity,false));
    }

}

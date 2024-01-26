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

            if (Input.GetKey(KeyCode.S))
            {
                transform.position += speed * Time.deltaTime * (transform.position - center).normalized;
            }
            if ((Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W)) && Vector3.Distance(transform.position, center) > 10f)
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
        uiManager.RemoveSatelliteUI();
        canMove = false;
        StartCoroutine(ZoomTo(target.position,target.position,true));
        
    }


    private IEnumerator ZoomTo(Vector3 targetPosition, Vector3 targetView, bool zoom){
        canZoom = false;
         while (Vector3.Distance(transform.position, targetPosition) > zoomDistance || ((targetView - transform.position).normalized - transform.forward.normalized).sqrMagnitude > 0.01)
        {
            Quaternion lookAt = Quaternion.LookRotation(targetView - transform.position);
            transform.SetPositionAndRotation(Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * zoomRate), Quaternion.Slerp(transform.rotation, lookAt, Time.deltaTime * rotationRate));
            yield return null;
        }
        if (zoom){
            uiManager.DisplaySatelliteUI(satelliteManager.selectedSatellite.GetComponent<Satellite>());
            uiManager.ActivateReturnButton();
        } else {
            satelliteManager.selectedSatellite.GetComponent<Satellite>().RemoveOutlines();
            satelliteManager.selectedSatellite = null;
            
            canMove = true;

        }
        canZoom = true;
    }


    public void UnZoom(){
        uiManager.RemoveSatelliteUI();
        uiManager.DesactivateReturnButton();
        canZoom = false;
        StartCoroutine(ZoomTo(spawnPosition, center ,false));
    }

}

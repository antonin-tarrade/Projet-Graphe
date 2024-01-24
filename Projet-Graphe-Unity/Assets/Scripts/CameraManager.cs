using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Vector3 center;
    public float speed;
    public float zoomRate;
    public float zoomDistance;
    public float rotationRate;


    // Start is called before the first frame update
    void Start()
    {
        center = new();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow)){
            transform.RotateAround(center,transform.up,-Time.deltaTime * speed);
        }

        if (Input.GetKey(KeyCode.UpArrow)){
            transform.RotateAround(center,transform.right,Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.LeftArrow)){
            transform.RotateAround(center,transform.up,Time.deltaTime * speed);
        }

        if (Input.GetKey(KeyCode.DownArrow)){
            transform.RotateAround(center,transform.right,-Time.deltaTime * speed);
        }

        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W) ){
            transform.position += speed * Time.deltaTime * (transform.position - center).normalized;
        }
        if (Input.GetKey(KeyCode.S)){
            transform.position -= speed * Time.deltaTime * (transform.position - center).normalized;
        }
    }

    public void SetAveragePos(Vector3 position){
        center = position;
        transform.position = center - new Vector3(0,0,100);
    }

    public void GoTo(Transform target){
        StartCoroutine(ZoomTo(target));
    }


    private IEnumerator ZoomTo(Transform target){
         while (Vector3.Distance(transform.position, target.position) > zoomDistance)
        {
            transform.SetPositionAndRotation(Vector3.Lerp(transform.position, target.position, Time.deltaTime * zoomRate), Quaternion.Slerp(transform.rotation, target.rotation, Time.deltaTime * rotationRate));
            yield return null;
        }
    }

}

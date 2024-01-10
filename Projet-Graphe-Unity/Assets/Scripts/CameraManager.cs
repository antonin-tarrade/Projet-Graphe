using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.GameCenter;

public class CameraManager : MonoBehaviour
{
    private Vector3 center;
    public float speed;

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

        if (Input.GetKey(KeyCode.Z) || Input.GetKeyDown(KeyCode.W) ){
            transform.position += speed * Time.deltaTime * (transform.position - center).normalized;
            
        }
        if (Input.GetKey(KeyCode.S)){
            transform.position -= speed * Time.deltaTime * (transform.position - center).normalized;
            
        }

        // transform.LookAt(center);
    }

    public void SetAveragePos(Vector3 position){
        center = position;
        transform.position = center - new Vector3(0,0,100);
    }


}

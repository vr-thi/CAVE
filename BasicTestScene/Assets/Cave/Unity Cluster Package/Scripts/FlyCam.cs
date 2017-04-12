﻿using System.Collections;
using System.Collections.Generic;
using UnityClusterPackage;
using UnityEngine;
using UnityEngine.Networking;

public class FlyCam : MonoBehaviour
{

    public float speed = 50.0f; // max speed of camera
    public float sensitivity = 0.25f;       // keep it from 0..1
    public bool inverted = false;


    private Vector3 lastMouse = new Vector3(255, 255, 255);

    // Smoothing
    public bool smooth = true;
    public float acceleration = 0.05f;
    private float actSpeed = 0.0f;  // keep it from 0..1
    private Vector3 lastDir = new Vector3();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (NodeInformation.type.Equals("master"))
        {
            // Mouse Look
            if (Input.GetKey(KeyCode.Mouse0))
            {
                lastMouse = Input.mousePosition - lastMouse;
                if (!inverted) lastMouse.y = -lastMouse.y;
                lastMouse *= sensitivity;
                lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.y, transform.eulerAngles.y + lastMouse.x, 0);
                transform.eulerAngles = lastMouse;

                lastMouse = Input.mousePosition;
            }



            // Movement of Camera

            Vector3 dir = new Vector3();    // create (0,0,0)

            if (Input.GetKey(KeyCode.UpArrow)) dir.z += 1.0f;
            if (Input.GetKey(KeyCode.DownArrow)) dir.z -= 1.0f;
            if (Input.GetKey(KeyCode.LeftArrow)) dir.x -= 1.0f;
            if (Input.GetKey(KeyCode.RightArrow)) dir.x += 1.0f;
            dir.Normalize();

            if (dir != Vector3.zero)
            {
                // some movement
                if (actSpeed < 0.01)
                    actSpeed += acceleration * Time.deltaTime;
                else
                    actSpeed = 0.01f;

                lastDir = dir;
            }
            else
            {
                // should stop
                if (actSpeed > 0)
                    actSpeed -= acceleration * Time.deltaTime;
                else
                    actSpeed = 0.0f;
            }


            if (smooth)
                transform.Translate(lastDir * actSpeed * speed * Time.deltaTime * 40);
            else
                transform.Translate(dir * speed * Time.deltaTime * 20);
        }
    }
}
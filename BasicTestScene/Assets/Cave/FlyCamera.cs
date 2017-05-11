﻿using UnityEngine;
using UnityClusterPackage;

public class FlyCamera : MonoBehaviour {

    private float translateVelocity = 0.125f;
    private Vector3 directionVector;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update()
    {
        if (NodeInformation.type.Equals("master"))
        {
            directionVector = new Vector3(InputSynchronizer.GetAxis("Horizontal"), 0, InputSynchronizer.GetAxis("Vertical"));
            transform.position += directionVector * translateVelocity;
        }
    }
}

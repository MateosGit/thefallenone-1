﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float speed = 5.0f;
    public float jumpSpeed = 2.5f;

    private Rigidbody _body;

    public Vector3 Position
    {
        get { return _body.transform.position; }
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

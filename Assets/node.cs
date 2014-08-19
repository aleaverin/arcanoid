using UnityEngine;
using System;
using System.Collections;

public class node : MonoBehaviour 
{ 
	Vector3 p;
	// Use this for initialization
	void Start () 
	{
		p = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	private Action destroyHandler = ()=>{};
	public void AddDestroyHandler(Action handler)
	{
		destroyHandler += handler;
	}
	void OnDestroy()
	{
		destroyHandler();

	}
}


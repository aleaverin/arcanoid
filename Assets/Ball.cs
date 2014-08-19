using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Ball : MonoBehaviour 
{
	private Transform Transform;
	public Vector3 Position { get {return Transform.position;}}

	[SerializeField]
	ParticleSystem boomParticleSystem;


	float bonusSpeedMultiplicator = 2.0f;
	float bonusScaleMultiplicator = 1.5f;
	float currentSpeedModificator = 1.0f;
	float currentScaleModificator = 1.0f;
	bool ignoreNodeCollide = false;



	public void Scale(float multiplicator)
	{
		currentScaleModificator *= multiplicator;
		Transform.localScale *= multiplicator;
	}
	public void Accelerate(float multiplicator)
	{
		currentSpeedModificator *= multiplicator;
		force *= multiplicator;
	}
	public void SetIgnoreNodeCollide(bool ignore)
	{
		ignoreNodeCollide = ignore;
	}
	void Awake()
	{
		Transform = transform;
	}
	
	public float DefaultBottomPosition { get; set; }
	public void SetPositionToDefault()
	{
		Transform.position = new Vector3(0, DefaultBottomPosition + Transform.localScale.y/2, 0);
	}
	
	bool waitingApproveStart = true;
	public void Reset()
	{
		waitingApproveStart = true;
	}

	private Action nodeDestroyHandler;
	public void SetNodeDestroyHandler(Action handler)
	{
		nodeDestroyHandler = handler;
	}

	[SerializeField]
	Vector3 forceDirection;
	[SerializeField]
	float force = 1;
	
	void FixedUpdate()
	{
		AddAngularCompensation();
		if (sumCollisionNormals != Vector3.zero)
		{
			forceDirection = Vector3.Reflect(forceDirection, sumCollisionNormals);
			forceDirection.Normalize();
			sumCollisionNormals = Vector3.zero;
		}
		rigidbody.velocity = forceDirection * force * Time.fixedDeltaTime * (waitingApproveStart?0:1);
	}
	
	float maxAngularWeightTechRatio = 1.5f;
	[SerializeField]
	float maxAngularWeight = 0.75f;
	[SerializeField]
	float maxAngular = 360.0f;
	void AddAngularCompensation()
	{
		float acc = rigidbody.angularVelocity.z * Time.fixedDeltaTime;
		acc = Mathf.Sign(acc) 
			* Mathf.Min (maxAngular, Mathf.Abs(acc)) 
				* maxAngularWeight 
				* maxAngularWeightTechRatio;
		forceDirection += new Vector3 (acc, 0, 0);
		forceDirection.Normalize ();
	}
	//--------------------------------
	Vector3 sumCollisionNormals = Vector3.zero;

	int maxLengthHitOfPointList = 6;
	List<ContactPoint> hitOfPoint = new List<ContactPoint> ();
	void OnCollisionEnter(Collision collision) 
	{
		foreach (ContactPoint contact in collision.contacts) 
		{

			if (isPlatform(contact.otherCollider))
			{
				Platform p = contact.otherCollider.gameObject.GetComponent<Platform>();
				rigidbody.angularVelocity = new Vector3(0,0,p.GetNormalizedAccelerate()*maxAngular);
			}
			else
			{
				rigidbody.angularVelocity = Vector3.zero;
			}
			if (isNode(contact.otherCollider))
			{
				if (!ignoreNodeCollide)
				{
					sumCollisionNormals += contact.normal;
				}
				BonusManager.Gen (contact.otherCollider.gameObject.transform.position);
				boomParticleSystem.transform.position = contact.otherCollider.gameObject.transform.position;
				boomParticleSystem.Play();
				nodeDestroyHandler();
				Destroy(contact.otherCollider.gameObject);
				hitOfPoint.Clear();
			}
			else
			{
				sumCollisionNormals += contact.normal;
				if (hitOfPoint.Contains(contact))
				{
					Debug.Log("ploho");
				}
				else
				{
					hitOfPoint.Add(contact);
					if (hitOfPoint.Count > maxLengthHitOfPointList)
					{
						hitOfPoint.RemoveAt(0);
					}
				}
			}
		}
	}

	private bool isNode(Collider collider)
	{
		return collider.name == "node";
	}
	private bool isPlatform(Collider collider)
	{
		return collider.name == "platform";
	}

	void Update () 
	{
		if (Input.GetMouseButtonDown(0))
		{
			waitingApproveStart = false;
		}
	}

	void OnCollisionStay(Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts) 
		{
			sumNormals += contact.normal;
		}
	}
	Vector3 sumNormals = Vector3.zero;
	void OnCollisionExit()
	{

	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Platform : MonoBehaviour 
{
	private Vector3 mousePosition;
	private bool inputCatched = false;
	private List<float> accelerate = new List<float>();

	[SerializeField]
	private int accelerateLevel = 10; // default = 1;
	[SerializeField]
	float accelerateLevelRatio = 0.5f;

	[SerializeField]
	private GameObject[] childs;
	private List<Vector3> childDefaultScales;
	private float bonusScale = 1.5f;
	private float currentScale = 1;

	private Transform Transform;
	public Vector3 Position { get {return Transform.position;}}


	
	void Awake()
	{
		Transform = transform;
		childDefaultScales = new List<Vector3> ();
		foreach (var child in childs)
		{
			childDefaultScales.Add(child.transform.localScale);
		}
	}
	private void Scale(Transform transform, float multiplicator)
	{
		Vector3 result = transform.localScale;
		result.x *= multiplicator;
		transform.localScale = result;
	}
	//[SerializeField]
	public void Scale(float multiplicator)
	{
		currentScale *= multiplicator;
		Scale(Transform, multiplicator);
		foreach (var child in childs)
		{
			Scale(child.transform, 1/multiplicator);
		}
	}

	public float DefaultUpPosition { get { return DefaultBottomPosition + Transform.localScale.y; }}
	public float DefaultBottomPosition { get; set; }
	public void SetPositionToDefault()
	{
		Transform.position = new Vector3(0, DefaultBottomPosition + Transform.localScale.y/2, 0);
	}
	public void Reset()
	{
		inputCatched = false;
	}

	private bool TryCatchInput()
	{
		if (Input.GetMouseButtonDown(0))
		{
			mousePosition = Input.mousePosition;
			Ray ray = DeviceSettings.Camera.ScreenPointToRay(mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if (HitTest(hit))
				{
					return true;
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
		{
			return true;
		}
		return false;
	}
	private bool TryDropCatchedInput()
	{
		if (Input.GetMouseButtonUp(0))
		{
			return true;
		}
		if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
		{
			return true;
		}
		return false;
	}
	private Vector3 GetInputDelta()
	{
		if (Input.GetMouseButton(0))
		{
			Vector3 delta = Input.mousePosition - mousePosition;
			mousePosition = Input.mousePosition;
			return delta;
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			return -Vector3.right * Time.deltaTime * 800.0f;
		} 
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			return Vector3.right * Time.deltaTime * 800.0f;
		}
		return Vector3.zero;
	}

	void Update () 
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			Scale (bonusScale);
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			Scale (1/bonusScale);
		}
		inputCatched |= TryCatchInput();

		if (TryDropCatchedInput())
		{
			inputCatched = false;
			accelerate.Clear();
		}
		if (inputCatched)
		{
			Vector3 delta = GetInputDelta();
			Transform.Translate(Vector3.Project(delta, Vector3.right) * DeviceSettings.UnitOnPixel);
			if (accelerate.Count >= accelerateLevel)
			{
				accelerate.RemoveAt(0);
			}
			accelerate.Add(delta.x);
		}
	}

	private bool HitTest(RaycastHit hit)
	{
		return hit.transform.name == "platform";
	}


	public float GetNormalizedAccelerate()
	{
		return GetAccelerate () / Screen.width;
	}

	public float GetAccelerate()
	{
		float accel = 0;
		float ratio = 1;
		int count = accelerate.Count;
		for (int i = count - 1 ; i >= 0; i--)
		{
			accel += accelerate[i] * ratio;
			ratio *= accelerateLevelRatio;
		}
		return accel;
	}

	void OnTriggerEnter(Collider other) 
	{
		Destroy(other.gameObject);
	}
	
}

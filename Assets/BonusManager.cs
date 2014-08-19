using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BonusManager : MonoBehaviour 
{
	[SerializeField]
	private Ball ball;
	[SerializeField]
	private Platform platform;
	[SerializeField]
	private Shader shader;

	private Material material;

	private Ball getBall() { return ball; }
	private Platform getPlatform(){ return platform; }

	private List<Func<GameObject>> actions;

	static BonusManager inc = null;

	void Awake()
	{
		inc = this;
		material = new Material(shader);

		actions = new List<Func<GameObject>>()
		{
			GenerateBonus(getPlatform().Scale , 1.3f, "ps+", Color.blue),
			GenerateBonus(getPlatform().Scale , 1.0f/1.3f, "ps-", Color.red),
			GenerateBonus(getBall().Accelerate, 2.0f, "ba+", Color.yellow),
			GenerateBonus(getBall().Accelerate, 1.0f/2.0f, "ba-", Color.white),
			GenerateBonus(getBall().Scale, 1.5f, "bs+", Color.magenta),
			GenerateBonus(getBall().Scale, 1.0f/1.5f, "bs-",Color.grey),
			GenerateBonus(getBall().SetIgnoreNodeCollide, true, "ic+", Color.cyan)
		};
	}

	void Update()
	{
		int key1 = (int)KeyCode.Alpha1;
		for (int keymod = 0; keymod < 10; keymod++ )
		{
			if (Input.GetKeyDown((KeyCode)(key1+keymod)))
			{
				Debug.Log((KeyCode)(key1+keymod));
				actions[keymod]();
			}
		}
	}

	public static void Gen(Vector3 pos)
	{
		if (UnityEngine.Random.value > 0.8f)
		{
			inc.actions[UnityEngine.Random.Range(0,inc.actions.Count)]().transform.position = pos;
		}
	}

	private Func<GameObject> GenerateObject(Action action, string shortBonusName, Color bonusColor)
	{
		return () => {
			GameObject go = GameObject.CreatePrimitive (PrimitiveType.Sphere);
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit rh;
			if (Physics.Raycast (ray,out rh))
			{
				go.transform.position = rh.point;
			}

			go.AddComponent<bonus>().SetAction(action);
			go.renderer.material.SetColor("_MainTex", bonusColor);
			go.collider.isTrigger = true;
			Rigidbody rb = go.AddComponent<Rigidbody>();
			rb.mass = 3.0f;
			//rb.isKinematic = true;
			GUIText gt = go.AddComponent<GUIText>(); 
			gt.text = shortBonusName;
			gt.fontSize = 50;
			gt.color = Color.red;
			gt.anchor = TextAnchor.MiddleCenter;
			return go;
		};
	}

	private Func<GameObject> GenerateBonus(Action<float> action, float modificator, string shortBonusName, Color bonusColor)
	{
		return GenerateObject(() => 
			{ 
				action (modificator);
				SheduleDeactivate (() => action (1 / modificator));
			}, shortBonusName, bonusColor);
	}
	private Func<GameObject> GenerateBonus(Action<bool> action, bool modificator, string shortBonusName, Color bonusColor)
	{
		return GenerateObject (() => 
			{ 
				action (modificator);
				SheduleDeactivate (() => action (!modificator));
			}, shortBonusName, bonusColor);
	}
	private void SheduleDeactivate(Action action)
	{
		StartCoroutine(Wait(3.0f, action));
	}
	private IEnumerator Wait(float seconds, Action postAction)
	{
		yield return new WaitForSeconds(seconds);
		postAction();
	}

}

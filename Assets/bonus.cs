using UnityEngine;
using System;
using System.Collections;

public class bonus : MonoBehaviour 
{
	private Action bonusAction = ()=>{};
	public void SetAction(Action action)
	{
		bonusAction += action;
	}
	void OnColliderEnter(Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts) 
		{
			if (contact.otherCollider.name == "platform")
			{

				Destroy(this);
			}
		}
	}
	void LateUpdate()
	{



		Vector2 center = new Vector2( 
		                             Screen.width * transform.position.x,
		                            Screen.height * transform.position.y);
		Vector2 objectOffset = new Vector2 (
			DeviceSettings.Screen.x / 2 + transform.position.x,
			DeviceSettings.Screen.y / 2 + transform.position.y);
		guiText.pixelOffset = -center + objectOffset * (1.0f / DeviceSettings.UnitOnPixel);
		
	}
	void OnDestroy()
	{
		bonusAction();
	}
}

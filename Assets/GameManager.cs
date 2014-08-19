using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	[SerializeField]
	private AreaSettings AreaSettings;

	[SerializeField]
	private Ball ball;
	[SerializeField]
	private Platform platform;

	[SerializeField]
	private LevelController LevelController;

	[SerializeField]
	private bool buildSupportBorder = false;

	private int nodeCountOnLevel = 0;

	private float bottomBallPosition = float.NegativeInfinity;

	int score = 0;
	float time = 600.0f;
	int lifeCount = 3;

	private bool gameLocked = true;

	private bool start = true;

	void Awake()
	{
		DeviceSettings.Reset();
		LevelController.Initialize();
	}

	void Start()
	{
		AreaSettings.CreateBorders (buildSupportBorder);

		platform.DefaultBottomPosition = 
			-DeviceSettings.Screen.y/2 
			+ GameSettings.PlatformOffsetOnBottomInUnit;
		
		ball.DefaultBottomPosition = platform.DefaultUpPosition + 0.1f;
		
		bottomBallPosition = -DeviceSettings.Screen.y/2;

		platform.SetPositionToDefault();
		ball.SetPositionToDefault();
		backQuad.transform.localScale = DeviceSettings.Screen;
		backQuad.renderer.material.SetTextureScale ("_MainTex", DeviceSettings.Screen);

		ball.SetNodeDestroyHandler(NodeDestroyHandler);

		LoadNextLevel ();

		StartCoroutine(setStr());
	}

	private void LoadNextLevel()
	{
		Lock ();
		score += Mathf.Max (0, 600 - (int)time);
		GUIScore.text = "Score: " + score.ToString ();
		time = 0.0f;
		StartCoroutine(LevelController.LoadNextLevel(SetNodeCountOnLevel));
	}
	
	private void SetNodeCountOnLevel(int nodeCountOnLevel)
	{
		this.nodeCountOnLevel = nodeCountOnLevel;
	}
	private void NodeDestroyHandler()
	{
		nodeCountOnLevel--;
		score += 100;
		GUIScore.text = "Score: " + score.ToString ();
		if (nodeCountOnLevel == 0)
		{
			LoadNextLevel();
		}
	}
	[SerializeField]
	public GUIText GUIScore;
	
	private bool FallenBall()
	{
		return ball.Position.y < bottomBallPosition;
	}
	
	void Update () 
	{
		if (!gameLocked)
		{
			time += Time.deltaTime;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (gameLocked)
			{
				Unlock();
			}
		}
		
		if (FallenBall())
		{
			lifeCount--;
			if (!gameLocked)
			{
				Lock();
			}
		}
	}
	void Lock()
	{
		gameLocked = true;
		platform.SetPositionToDefault();
		platform.Reset();
		ball.SetPositionToDefault();
		ball.Reset ();
	}
	void Unlock()
	{
		gameLocked = false;
		start = false;
	}



	[SerializeField]
	private GUIStyle tap4StartStyle;
	[SerializeField]
	private Texture backTexture;
	[SerializeField]
	private GameObject backQuad;
	string starts ="Tap 4 start game";
	string startstring = "";
	IEnumerator setStr()
	{
		char[] s = starts.ToCharArray ();
		foreach (char c in s)
		{
			startstring += c;
			yield return new WaitForSeconds(0.1f);
		}
	}
	void OnGUI()
	{
		Rect fullRect = new Rect (0, 0, Screen.width, Screen.height); 
		if (start)
		{
			GUI.Label (fullRect, startstring , tap4StartStyle);
		}
		else if (gameLocked && lifeCount > 0)
		{
			GUI.Label (fullRect, "Tap 2 continue", tap4StartStyle);
		} 
		else if (lifeCount < 0)
		{
			GUI.Label (fullRect, "Game Over", tap4StartStyle);
		}
	}		
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStuff : MonoBehaviour
{
	bool arrowSlotted = false;
	[SerializeField]
	GameObject arrowPrefab;
	[SerializeField]
	GameObject arrow; //last spawned
	public GameObject ArrowVisual; //show hide placeholder, actual arrow spawns at behind camera
	AudioSource ac;
	public GameObject menuObject;
	FloatingMenuController fmc;
	int currentLevel = 0; //todo

	//ui
	public TextMeshProUGUI arrowsRemainingText;
	public TextMeshProUGUI starsText;
	public TextMeshProUGUI levelText;
	public int arrowsRemaining = 10;
	public int starsScore = 0;
	public GameObject inGameStats;
	public Camera ARCam;

	//mano
	float lastMenuTime;
	bool menuOpen = false;

	public Text DebugT;

	Vector3 sc = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
	public GameObject LevelTextObj;
	public Texture2D[] levels;
	Material levelMat;

	//level configs
	int[] levelShootForce = new int[]{4000,3500,3000,2500,1500};
	int[] starsRequiredPerLevelUP = new int[] { 40, 40, 35, 35, 40};
	int[] arrowsPerLevel = new int[] {10, 8, 7, 6, 5 };

	bool practiceMode = false;
	//levelShootForce starsRequiredPerLevelUP arrowsPerLevel

	public GameObject ArrowTmp; //tmp parent, remove on newgame

	public TextMeshProUGUI LevelInfo;

	public enum GameState {
		PlacingTarget,
		Playing,
		MenuOpen
	}

	public GameState _currentGameState = GameState.PlacingTarget;

	// Start is called before the first frame update
	void Start()
	{
		ac = GetComponent<AudioSource>();
		fmc = menuObject.GetComponent<FloatingMenuController>();
		//Debug.Log("Starting...");
		levelMat = LevelTextObj.GetComponent<MeshRenderer>().material;

		//PlayerPrefs.SetInt("currentLevel", 2);
		//int currentScore = PlayerPrefs.GetInt("currentLevel");
		//Debug.Log(currentLevel);
		//newGame(0);
	}

	public void newGame(int level) {
		foreach (Transform child in ArrowTmp.transform)
		{
			GameObject.Destroy(child.gameObject);
		}
		currentLevel = level;
		arrowsRemaining = arrowsPerLevel[level];
		starsScore = 0;
		arrowsRemainingText.text = arrowsRemaining.ToString();
		starsText.text = "0" + "/" + starsRequiredPerLevelUP[level].ToString();
		int cl = level + 1;
		levelText.text = "Level " + cl.ToString();
		LevelInfo.text = "Collect " + starsRequiredPerLevelUP[level].ToString() + " Stars";
		ArrowVisual.GetComponent<ArrowPullBack>().maxShootForce = levelShootForce[currentLevel];
		StartCoroutine("ShowLevelText");
	}

	IEnumerator ShowLevelText() {
		LevelTextObj.SetActive(true);
		levelMat.mainTexture = levels[currentLevel];
		yield return new WaitForSeconds(5);
		LevelTextObj.SetActive(false);
	}

	// Update is called once per frame
	void Update()
	{
		handleManoEvents();
	}

	void handleManoEvents() {
		//string dt = "";
		if (ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info.mano_gesture_trigger != ManoGestureTrigger.NO_GESTURE)
		{
			switch (ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info.mano_gesture_trigger)
			{
				case ManoGestureTrigger.CLICK:
					/**
					if (menuOpen)
					{
						DebugT.text = "CLICK: " + fmc.currentItemSelected.ToString();
						//selectMenuButton(fmc.currentItemSelected);
					}
					*/
					break;
				case ManoGestureTrigger.DROP:
					if (arrowSlotted)
					{
						ShootArrow();
					}
					break;
				case ManoGestureTrigger.GRAB_GESTURE:
					break;
				case ManoGestureTrigger.PICK:
					if (!arrowSlotted)
					{
						SpawnArrow();
					}
					break;
				case ManoGestureTrigger.RELEASE_GESTURE:
					/**
					if (menuOpen)
					{
						if (Time.time > lastMenuTime+1f) {
							DebugT.text = "CLICKR: " + fmc.currentItemSelected.ToString();
						}
						
						//selectMenuButton(fmc.currentItemSelected);
					}
					*/
					break;
				default:
					break;
			}
		}
		/** 
		//PAUSE menu
		if (ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info.hand_side == HandSide.Palmside && ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info.mano_gesture_continuous == ManoGestureContinuous.CLOSED_HAND_GESTURE) {
			if (!menuOpen) {
				if (Time.time > lastMenuTime+2f) {
					//menuObject.transform.parent = ARCam.transform;
					menuObject.SetActive(true);
					menuObject.transform.position = new Vector3(ARCam.transform.position.x, ARCam.transform.position.y, ARCam.transform.position.z + 4f);
					//menuObject.transform.rotation = ARCam.transform.rotation;
					//menuObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - menuObject.transform.position);
					//menuObject.transform.parent = null;
					Vector3 menuz = ARCam.ScreenToWorldPoint(sc);
					menuObject.transform.position = new Vector3(menuz.x, menuz.y, menuz.z + 3f);
					menuObject.transform.rotation = Quaternion.LookRotation(menuObject.transform.position - ARCam.transform.position);
					menuOpen = true;
					lastMenuTime = Time.time;
				}
			}
			else {
				if (Time.time > lastMenuTime + 2f)
				{
					menuObject.SetActive(false);
					menuOpen = false;
					lastMenuTime = Time.time;
				}
			}
			
		}
		
		if (menuOpen) {
			//if (ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info.mano_class == ManoClass.POINTER_GESTURE) {
				//Vector3 pointerPosition = ManoUtils.Instance.CalculateNewPosition(ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.bounding_box.top_left, 0f);
				//dt +=  ": " + pointerPosition.ToString();
				//Vector3 clickpos = ARCam.ViewportToWorldPoint(ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.palm_center);
				RaycastHit hitx;
				Vector3 fwd = ARCam.transform.TransformDirection(Vector3.forward) * 200;
				Debug.DrawRay(ARCam.transform.position, fwd, Color.green);
				if (Physics.Raycast(sc, fwd, out hitx))
				{
					Debug.Log(hitx.transform.gameObject.name);
					switch (hitx.transform.gameObject.name)
					{
						case "PlayBtn":
							fmc.selectItem(0);
							DebugT.text = "PlayBtn high";
							break;
						case "ReplayBtn":
							fmc.selectItem(1);
							DebugT.text = "ReplayBtn high";
							break;
						case "HomeBtn":
							fmc.selectItem(2);
							DebugT.text = "HomeBtn high";
							break;
						default:
							break;
					};
				}
				//dt += "-" + clickpos.ToString();
				//DebugT.text = dt;
			//}
		}
		*/
		//labelGesture.text = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.depth_estimation.ToString();
	}

	public void ShootArrow()
	{
		Rigidbody _arrowRigidB = arrow.GetComponent<Rigidbody>();

		//ArrowVisual shootforce calc
		float asf = ArrowVisual.GetComponent<ArrowPullBack>().getosf();
		//unparent before shooting
		arrow.transform.parent = ArrowTmp.transform;

		_arrowRigidB.isKinematic = false;
		_arrowRigidB.AddRelativeForce(Vector3.forward * asf); //arrowShootForc
		ArrowHit _arrowHit = arrow.GetComponent<ArrowHit>();
		_arrowHit.enabled = true;

		//hide placeholder arrow
		arrowSlotted = false;
		ac.Play();
		ArrowVisual.SetActive(false);
		arrowsRemaining -= 1;
		arrowsRemainingText.SetText(arrowsRemaining.ToString());
	}

	public void SpawnArrow()
	{
		if (arrowsRemaining > 0)
		{
			arrow = Instantiate(arrowPrefab, ARCam.transform.position, ARCam.transform.rotation) as GameObject;
			arrow.transform.parent = ARCam.transform;
			arrowSlotted = true;
			arrow.transform.position = new Vector3(ArrowVisual.transform.position.x, ArrowVisual.transform.position.y, arrow.transform.position.z);
			ArrowVisual.SetActive(true);
		}
		else {
			//Level over
			if (practiceMode) {
				arrow = Instantiate(arrowPrefab, ARCam.transform.position, ARCam.transform.rotation) as GameObject;
				arrow.transform.parent = ARCam.transform;
				arrowSlotted = true;
				arrow.transform.position = new Vector3(ArrowVisual.transform.position.x, ArrowVisual.transform.position.y, arrow.transform.position.z);
				ArrowVisual.SetActive(true);
			} else {
				//check if level pass
				//levelShootForce starsRequiredPerLevelUP arrowsPerLevel
				if (starsScore >= starsRequiredPerLevelUP[currentLevel])
				{
					//level up
					if (currentLevel + 1 < 5)
					{
						currentLevel += 1;
						newGame(currentLevel);
						//show win dialog
					}
					else
					{
						//practice mode
					}
				}
				else {
					//level not passed, show message
					newGame(currentLevel);
				}
			}
		}
	}

	public void setTargetPlaced() {
		
	}

	public void addStars(int st) {
		starsScore += st;
		string starlab = starsScore.ToString() + "/" + starsRequiredPerLevelUP[currentLevel].ToString();
		starsText.SetText(starlab);
		if (arrowsRemaining == 0) {
			if (starsScore >= starsRequiredPerLevelUP[currentLevel])
			{
				//level up
				if (currentLevel + 1 < 5)
				{
					currentLevel += 1;
					newGame(currentLevel);
					//show win dialog
				}
				else
				{
					//practice mode
				}
			}
			else
			{
				//level not passed, show message
				newGame(currentLevel);
			}
		}
	}

	void selectMenuButton(int x) {
		switch (x) {
			case 0: //play, close menu
				menuObject.SetActive(false);
				menuOpen = false;
				lastMenuTime = Time.time;
				break;
			case 1: //restart, newgame
				newGame(10);
				break;
			case 2: //home, wot
				break;
			default:
				break;
		}
	}
}

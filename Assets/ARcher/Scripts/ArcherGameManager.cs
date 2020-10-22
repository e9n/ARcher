using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ArcherGameManager : MonoBehaviour
{
	private static ArcherGameManager _instance;

	public static ArcherGameManager Instance {
		get {
			return _instance;
		}
		set {
			_instance = value;
		}
	}

	public Camera ARCam;

	//ShootArrowMaybe
	bool isArrowSlotted;

	//SpawnArrow
	int arrowsLeft = 10; //current game
	GameObject arrow; //last spawned arrow
	public GameObject arrowPrefab;
	public GameObject ArrowVisual; //to show pulling animation and stuff, original placed right behind it

	//ShootArrow
	public GameObject arrowsRoot; //all arrows shot on level parented to this
	AudioSource ac; //shoot sound
	public TextMeshProUGUI arrowsLeftText;

	//ArrowDie
	int GameScore; //stars
	public TextMeshProUGUI gameScoreText; //stars
	
	//Levels
	int currentLevel = 0;
	int[] levelShootForce = new int[] { 4000, 3500, 3000, 2500, 1500 };
	int[] starsRequired = new int[] { 55, 50, 45, 40, 25 };
	int[] arrowsPerLevel = new int[] { 10, 8, 7, 6, 5 };
	public TextMeshProUGUI levelText;
	public TextMeshProUGUI level_text; //new game splash
	public TextMeshProUGUI level_instruction; //new game splash

	private void Awake()
	{
		if (!_instance)
		{
			_instance = this;
			//ManomotionManager.OnManoMotionFrameProcessed += HandleManoMotionFrameProcessed;
			
		}
		else
		{
			Debug.LogError("More than 1 CubeManagers in the scene");
			Destroy(this.gameObject);
		}
	}

	private void Start()
	{
		ac = GetComponent<AudioSource>();
	}

	private void Update()
	{
		HandleManoMotionFrameProcessed();
	}

	public void HandleManoMotionFrameProcessed() {
		GestureInfo gesture = ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info;
		if (gesture.mano_gesture_trigger != ManoGestureTrigger.NO_GESTURE) {
			switch (gesture.mano_gesture_trigger) {
				case ManoGestureTrigger.DROP:
					if (isArrowSlotted)
					{
						ShootArrow();
					}
					break;
				case ManoGestureTrigger.PICK:
					if (!isArrowSlotted)
					{
						SpawnArrow();
					}
					break;
				default:
					break;
			}
		}
		/**
		
		//TrackingInfo trackingInfo = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info;
		Warning warning = ManomotionManager.Instance.Hand_infos[0].hand_info.warning;

		if (warning != Warning.WARNING_HAND_NOT_FOUND) {
			return;
		}
		
		ShootArrowMaybe(gesture);
		*/
	}

	//moved up
	public void ShootArrowMaybe(GestureInfo g) {
		switch(g.mano_gesture_trigger) {
			case ManoGestureTrigger.DROP:
				if (isArrowSlotted)
				{
					ShootArrow();
				}
				break;
			case ManoGestureTrigger.PICK:
				if (!isArrowSlotted)
				{
					SpawnArrow();
				}
				break;
			default:
				break;
		}
	}

	private void ShootArrow() {
		Rigidbody _arrowRigidB = arrow.GetComponent<Rigidbody>();

		//ArrowVisual shootforce calc
		float asf = ArrowVisual.GetComponent<ArrowPullBack>().getosf();
		//unparent before shooting
		arrow.transform.parent = arrowsRoot.transform;

		_arrowRigidB.isKinematic = false;
		_arrowRigidB.AddRelativeForce(Vector3.forward * asf); //arrowShootForc
		ArrowHit _arrowHit = arrow.GetComponent<ArrowHit>();
		_arrowHit.enabled = true;

		//hide placeholder arrow
		isArrowSlotted = false;
		ac.Play();
		ArrowVisual.SetActive(false);
		arrowsLeft -= 1;
		arrowsLeftText.SetText(arrowsLeft.ToString());
	}

	private void SpawnArrow()
	{
		if (arrowsLeft > 0) {
			//spawn
			arrow = Instantiate(arrowPrefab, ARCam.transform.position, ARCam.transform.rotation) as GameObject;
			arrow.transform.parent = ARCam.transform;
			isArrowSlotted = true;
			arrow.transform.position = ArrowVisual.transform.position; //keep the original arrow behind the screen
			ArrowVisual.SetActive(true); //show the placeholder arrow
		} else {
			//ignore?
		}
	}

	public void ArrowDie(int hitScore) {
		//update score
		GameScore += hitScore;
		ArrowVisual.GetComponent<ArrowPullBack>().strengthP.fillAmount = 0; //reset pull visualizer
		UpdateGameScore();
		if (arrowsLeft == 0) {
			//GameOver
			StartCoroutine("ShowGameOverDelayed");
		}
	}

	IEnumerator ShowGameOverDelayed() {
		yield return new WaitForSeconds(1);
		ShowGameOver();
	}

	void UpdateGameScore() {
		gameScoreText.SetText(GameScore.ToString() + "/" + starsRequired[currentLevel].ToString());
	}

	void ShowGameOver() {
		//show game score
		//if pass, change level
		if (GameScore >= starsRequired[currentLevel]) {
			if (currentLevel + 1 < levelShootForce.Length) {
				//next level
				currentLevel += 1;
				newGame(currentLevel);
			} else {
				//no more levels, VICTORY
				level_text.text = "Congrats!";
				level_instruction.text = "You have successfully completed all the levels in the game!";
				StartCoroutine("ShowLevelGoals");
			}
		}
	}

	public void newGame(int level)
	{
		//clear any arrows in scene
		foreach (Transform child in arrowsRoot.transform)
		{
			GameObject.Destroy(child.gameObject);
		}
		//set level
		currentLevel = level;
		string lt = "Level " + (level + 1).ToString();
		levelText.text = lt;
		//reset to default
		isArrowSlotted = false;
		arrowsLeft = arrowsPerLevel[level];
		arrowsLeftText.text = arrowsLeft.ToString();
		GameScore = 0;
		string gs = "0" + "/" + starsRequired[level].ToString();
		gameScoreText.text = gs;

		//set arrow shoot force for this level
		ArrowVisual.GetComponent<ArrowPullBack>().maxShootForce = levelShootForce[currentLevel];

		//show new level targets
		level_text.text = lt;
		string li = "Collect " + starsRequired[level].ToString() + " Stars";
		level_instruction.text = li;
		StartCoroutine("ShowLevelGoals");
	}

	IEnumerator ShowLevelGoals() {
		level_text.transform.gameObject.SetActive(true);
		level_instruction.transform.gameObject.SetActive(true);
		yield return new WaitForSeconds(3);
		level_text.transform.gameObject.SetActive(false);
		level_instruction.transform.gameObject.SetActive(false);

	}

}

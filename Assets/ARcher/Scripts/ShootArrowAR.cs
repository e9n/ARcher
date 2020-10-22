using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootArrowAR : MonoBehaviour
{
	[SerializeField]
	int numberOfArrows = 10;
	bool arrowSlotted = false;
	float pullAmount = 10f;
	[SerializeField]
	GameObject arrowPrefab;
	[SerializeField]
	GameObject arrow;
	[SerializeField]
	float arrowShootForce = 2000;

	//Mano
	private ManoGestureTrigger click;
	public Text labelGesture;
	AudioSource ac;
	public GameObject ArrowVisual; //show hide placeholder, actual arrow spawns at camera z

	// Start is called before the first frame update
	void Start()
	{
		//SpawnArrow();
		//ShootLogic();
		click = ManoGestureTrigger.CLICK;
		ac = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
		/**
		if (Input.GetMouseButtonDown(0))
		{
			//ShootLogic();
			SpawnArrow();
		}
		if (Input.GetMouseButtonUp(0))
		{
			ShootLogic();
		}
		*/

		if (ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info.mano_gesture_trigger != ManoGestureTrigger.NO_GESTURE) {
			switch (ManomotionManager.Instance.Hand_infos[0].hand_info.gesture_info.mano_gesture_trigger) {
				case ManoGestureTrigger.CLICK:
					labelGesture.text = "CLICK";
					break;
				case ManoGestureTrigger.DROP:
					labelGesture.text = "DROP";
					if (arrowSlotted) {
						ShootLogic();
						//arrowSlotted = false;
						//ac.Play();
						//ArrowVisual.SetActive(false);
					}
					break;
				case ManoGestureTrigger.GRAB_GESTURE:
					labelGesture.text = "GRAB";
					break;
				case ManoGestureTrigger.PICK:
					labelGesture.text = "PICK";
					if (!arrowSlotted) {
						SpawnArrow();
						
					}
					break;
				case ManoGestureTrigger.RELEASE_GESTURE:
					labelGesture.text = "RELEASE";
					break;
				default:
					break;
			}
		}
		//labelGesture.text = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.depth_estimation.ToString();
	}

	public void ShootLogic()
	{
		
		
		/**
		if (numberOfArrows > 0) {
			arrowSlotted = true;
			arrow = Instantiate(arrowPrefab, transform.position, transform.rotation) as GameObject;
			arrow.transform.parent = transform;
		}
		

		if (numberOfArrows > 0) {
			if (pullAmount < 100) {
				pullAmount = 100;
			}
		}
		*/
		Rigidbody _arrowRigidB = arrow.GetComponent<Rigidbody>();
		//ProjectileArrow _arrowProjectile = arrow.GetComponent<ProjectileArrow>();

		//arrowProjectile.shootForce = _arrowProjectile.shootForce * (pullAmount / 100) + 0.1f;
		//_arrowProjectile.enabled = true;

		//ArrowVisual shootforce calc
		float asf = ArrowVisual.GetComponent<ArrowPullBack>().getosf();
		//Debug.Log(asf);

		//leave parent
		arrow.transform.parent = null;

		_arrowRigidB.isKinematic = false;
		_arrowRigidB.AddRelativeForce(Vector3.forward * 2000); //arrowShootForc

		ArrowHit _arrowHit = arrow.GetComponent<ArrowHit>();
		_arrowHit.enabled = true;

		arrowSlotted = false;
		ac.Play();
		ArrowVisual.SetActive(false);
	}

	public void SpawnArrow()
	{
		if (numberOfArrows > 0)
		{
			arrow = Instantiate(arrowPrefab, transform.position, transform.rotation) as GameObject;
			arrow.transform.parent = transform;
			arrowSlotted = true;
			//ArrowVisual.GetComponent<ArrowPullBack>().resetPos();
			//ArrowVisual.GetComponent<ArrowPullBack>().StartPulling();
			arrow.transform.position = new Vector3(ArrowVisual.transform.position.x, ArrowVisual.transform.position.y, arrow.transform.position.z);
			ArrowVisual.SetActive(true);
			//GameObject.Find("ArrowDummyZ").GetComponent<Text>().text = ArrowVisual.transform.position.z.ToString();
			//StartCoroutine("Pullbackdummy");
		}
	}

	IEnumerator Pullbackdummy()
	{
		yield return new WaitForSeconds(2);
		Vector3 op = ArrowVisual.transform.position;
		GameObject aac = GameObject.Find("ArrowARCenter");
		aac.transform.position = new Vector3(aac.transform.position.x, aac.transform.position.y, aac.transform.position.z + 0.01f);
		yield return new WaitForSeconds(2);
		ArrowVisual.transform.position = op;
	}
}

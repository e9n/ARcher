using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowHit : MonoBehaviour
{
	AudioSource ac;
	Rigidbody rigidB;
	bool collided = false;

	private void Start()
	{
		ac = GetComponent<AudioSource>();
		rigidB = GetComponent<Rigidbody>();
		//rigidB.velocity = Vector3.zero;
		StartCoroutine("DestroyAfterFive");
	}

	IEnumerator DestroyAfterFive() {
		yield return new WaitForSeconds(5);
		ArcherGameManager.Instance.ArrowDie(0);
	}


	private void OnCollisionEnter(Collision collision)
	{
		collided = true;
		GetComponent<CapsuleCollider>().enabled = false;
		GetComponent<Rigidbody>().isKinematic = true;
		
		//transform.parent = null;
		ac.Play();
		/**
		if (collision.collider.gameObject.name == "target") {
			float dis = Vector3.Distance(GameObject.Find("IndicatorFINDME").gameObject.transform.position, collision.contacts[0].point);
			calculateScore(dis);
		}
		*/
		if (collision.collider.gameObject.CompareTag("Target")) {
			float dis = Vector3.Distance(collision.collider.gameObject.transform.position, collision.contacts[0].point);
			float _score = 10 - ((dis * 10) / 2.5f);
			int score = (int)_score;
			ArcherGameManager.Instance.ArrowDie(score);

		}
		//nothing done when no target hit
	}

	void calculateScore(float dist) {
		float score = 10 - ((dist * 10) / 2.5f);
		int sco = (int)score;
		//GameObject.Find("ArrowDist").gameObject.GetComponent<Text>().text = score.ToString() + " : " + dist.ToString();
		GameObject.Find("GameManager").gameObject.GetComponent<GameStuff>().addStars(sco);
	}

	void SpinObjectInAir()
	{
		float _yVelocity = rigidB.velocity.y;
		float _xVelocity = rigidB.velocity.x;
		float _zVelocity = rigidB.velocity.z;
		float _combineVelocity = Mathf.Sqrt(_xVelocity * _xVelocity + _zVelocity * _zVelocity);
		float _fallAngle = -1 * Mathf.Atan2(_yVelocity, _combineVelocity) * 180 / Mathf.PI;

		//transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, _fallAngle);
		transform.eulerAngles = new Vector3(_fallAngle, transform.eulerAngles.y, transform.eulerAngles.z);
	}

	private void Update()
	{
		if (collided) return;
		SpinObjectInAir();
	}
}

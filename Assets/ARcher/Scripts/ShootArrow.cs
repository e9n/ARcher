using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootArrow : MonoBehaviour
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
	float arrowShootForce = 10;

	// Start is called before the first frame update
	void Start()
	{
		//SpawnArrow();
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0)) {
			//ShootLogic();
			SpawnArrow();
		}
		if (Input.GetMouseButtonUp(0))
		{
			ShootLogic();
		}
	}

	void ShootLogic() {
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
		_arrowRigidB.isKinematic = false;
		_arrowRigidB.AddRelativeForce(Vector3.up * arrowShootForce);
	}

	void SpawnArrow() {
		if (numberOfArrows > 0) {
			arrowSlotted = true;
			arrow = Instantiate(arrowPrefab, transform.position, transform.rotation) as GameObject;
			arrow.transform.parent = transform;
		}
	}
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Samples
{
	/// <summary>
	/// Listens for touch events and performs an AR raycast from the screen touch point.
	/// AR raycasts will only hit detected trackables like feature points and planes.
	///
	/// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
	/// and moved to the hit position.
	/// </summary>
	[RequireComponent(typeof(ARRaycastManager))]
	public class PlaceOnPlane : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Instantiates this prefab on a plane at the touch location.")]
		GameObject m_PlacedPrefab; //indicator
		private bool tapped = false;
		//public Transform indicator;

		public GameObject findplanetext;
		public GameObject ingamestats;
		Vector3 sc = new Vector3(Screen.width / 2, Screen.height / 2, 0f);

		/// <summary>
		/// The prefab to instantiate on touch.
		/// </summary>
		public GameObject placedPrefab
		{
			get { return m_PlacedPrefab; }
			set { m_PlacedPrefab = value; }
		}

		/// <summary>
		/// The object instantiated as a result of a successful raycast intersection with a plane.
		/// </summary>
		public GameObject spawnedObject { get; private set; }

		void Awake()
		{
			m_RaycastManager = GetComponent<ARRaycastManager>();
			spawnedObject = placedPrefab;

		}

		bool TryGetTouchPosition(out Vector2 touchPosition)
		{
	#if UNITY_EDITOR
			if (Input.GetMouseButton(0))
			{
				var mousePosition = Input.mousePosition;
				touchPosition = new Vector2(mousePosition.x, mousePosition.y);
				return true;
			}
#else
			if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
			{
				touchPosition = Input.GetTouch(0).position;
				return true;
			}
#endif

			touchPosition = default;
			return false;
		}

		void Update()
		{
			if (tapped) return;
			if (m_RaycastManager.Raycast(sc, s_Hits, TrackableType.PlaneWithinPolygon)) {
				var hitPose = s_Hits[0].pose;
				spawnedObject.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
			}
			if (!TryGetTouchPosition(out Vector2 touchPosition))
				return;
			if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
			{
				// Raycast hits are sorted by distance, so the first one
				// will be the closest hit.
				var hitPose = s_Hits[0].pose;
				//spawnedObject.transform.SetPositionAndRotation(hitPose.position, hitPose.rotation);
				spawnedObject.transform.Find("Ring").gameObject.SetActive(false);
				spawnedObject.transform.Find("target").gameObject.SetActive(true);
				tapped = true;
				foreach (var plane in GetComponent<ARPlaneManager>().trackables)
				{
					plane.gameObject.SetActive(false);
				}
				GetComponent<ARPlaneManager>().requestedDetectionMode = PlaneDetectionMode.None;
				findplanetext.SetActive(false);
				ingamestats.SetActive(true);
				//GameObject.Find("GameManager").gameObject.GetComponent<GameStuff>().newGame(0);
				ArcherGameManager.Instance.newGame(0);
			}
		}

		static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

		ARRaycastManager m_RaycastManager;
	}
}
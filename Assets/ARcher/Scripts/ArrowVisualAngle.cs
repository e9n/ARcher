using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowVisualAngle : MonoBehaviour
{
    public GameObject ARC;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(360f-ARC.transform.rotation.x, transform.localRotation.y, transform.localRotation.z);
    }
}

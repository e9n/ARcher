using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowPullBack : MonoBehaviour
{
    float minZ = 0.044f;
    float maxZ = 0.035f;
    float currentZ; //store z at spawn
    bool pullBack = false;
    Text tx;
    float osf;

    Animator anim;
    bool nomoanim;

    //strengthbar
    public Image strengthP;
    TrackingInfo tracking;

    public int maxShootForce = 2000;

    private void Start()
    {
        currentZ = transform.position.z;
        //tx = GameObject.Find("ArrowDummyZ").GetComponent<Text>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        nomoanim = false;
        transform.position = new Vector3(transform.position.x, transform.position.y, currentZ);
    }

    public void StartPulling() {
        pullBack = true;
    }

    public void resetPos() {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        /**
        if (pullBack && currentZ > minZ) {
            currentZ -= 0.0001f;
            if (currentZ > maxZ) {
                pullBack = false;
            }
        }
        */
        /**
        if (ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.depth_estimation != 1){
            float d = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info.depth_estimation;
            float z = ((1f - d) / 0.04f) * 0.00692307692f;
            transform.position = new Vector3(transform.position.x, transform.position.y, currentZ-z);
        }
        */
        if (nomoanim) {
            return;
        }
        float sfx = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (sfx >= 1f) {
            GetComponent<Animator>().StopPlayback();
            nomoanim = true;
        }
        osf = (sfx / 1) * maxShootForce; //current percentage / total * maxShootForce
        //tx.text = osf.ToString();
        strengthP.fillAmount = sfx;
        //tracking = ManomotionManager.Instance.Hand_infos[0].hand_info.tracking_info;
        //transform.position = Camera.main.ViewportToWorldPoint(new Vector3(tracking.palm_center.x, tracking.palm_center.y, 0.044f));
    }

    public float getosf() {
        return osf;
    }
}

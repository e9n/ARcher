using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingMenuController : MonoBehaviour
{
    public GameObject[] highlights;
    public int currentItemSelected; //return on click or hwaterv

    public void selectItem(int x) {
        if (x == currentItemSelected) {
            return;
        }
        currentItemSelected = x;
        hideAllHighlights();
        highlights[x].SetActive(true);
    }

    void hideAllHighlights() {
        for (int i = 0; i < highlights.Length; i++) {
            highlights[i].SetActive(false);
        }
    }
}

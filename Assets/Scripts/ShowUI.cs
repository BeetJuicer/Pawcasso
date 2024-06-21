using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowUI : MonoBehaviour
{
    public GameObject uiObject;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            uiObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        uiObject.SetActive(false);
    }

}

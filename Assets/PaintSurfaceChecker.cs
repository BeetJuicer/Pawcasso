using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintSurfaceChecker : MonoBehaviour
{
    [SerializeField] private Transform[] rayPosTransforms;
    private float detectionInterval = 0.1f; //how long in between checks?
    private float lastDetectTime;
    private float rayDistance = 1f;

    public GameObject testSphere;
    public static bool IsOnColoredGround { get; private set; }

    private void Update()
    {
        if(Time.time > lastDetectTime + detectionInterval)
        {
            Color color = IsOnColoredGround ? Color.white : Color.black;
            testSphere.GetComponent<Renderer>().material.SetColor("_Color", color);

            RaycastHit hit;   
            lastDetectTime = Time.time;

            foreach(Transform rayPos in rayPosTransforms)
            {
                Ray ray = new(rayPos.position, Vector3.down);

                if (Physics.Raycast(ray, out hit, rayDistance))
                {
                    if (hit.collider.TryGetComponent(out PaintTarget target)){
                        //check the channel of the texture, if at least one of the rays detects color, check IsOnColoredGround
                        if (PaintTarget.RayChannel(ray) != -1)
                        {
                            IsOnColoredGround = true;
                            return;
                        }
                    }           
                }
            }

            //none of the rays detected color
            IsOnColoredGround = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection : MonoBehaviour
{
    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
    }
    void Update()
    {
        DetectSelectObjectByRaycast();
    }

    private void DetectSelectObjectByRaycast() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.tag=="Chess") {
                    Debug.Log(hit.collider.gameObject.tag);
                }
            }
        }
    
    }
}

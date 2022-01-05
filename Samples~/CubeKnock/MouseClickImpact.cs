using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sainna.Onomatopoeia;

public class MouseClickImpact : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown (0)){
            RaycastHit hit; 
            Camera cam = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();
            Ray ray = cam.ScreenPointToRay(Input.mousePosition); 
            if ( Physics.Raycast (ray,out hit,100.0f)) {
                ColliderImpact csound = hit.rigidbody?.gameObject.GetComponentInChildren<ColliderImpact>();
                csound?.SpawnAnimation(hit.point, hit.normal);
            }
        }
    }
}

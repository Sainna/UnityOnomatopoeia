using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnomatopoeiaGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject _DefaultOnomatopoeiaPrefab;

    [SerializeField]
    Transform _TransformToFace;

    [SerializeField, Tooltip("Have the effect face the player")]
    Onomatopoeia.FacePlayerType _FacePlayerInfo = Onomatopoeia.FacePlayerType.AllDirection;

    private void Start() {
        if(_TransformToFace == null)
        {
            _TransformToFace = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }
    }

    // private Quaternion GetOnoRotation(Vector3 pos)
    // {
    //     Vector3 forward = Vector3.forward;
    //     Vector3 up = Vector3.up;
    //     if(_FacePlayerInfo == Onomatopoeia.FacePlayerType.None)
    //     {
    //         throw new System.Exception("not implemented");
    //     }
    //     else
    //     {
    //         forward = pos - _TransformToFace.position;
    //         if(_FacePlayerInfo == Onomatopoeia.FacePlayerType.AllDirection)
    //         {
    //             up = _TransformToFace.up;
    //         }
    //     }


    //     Quaternion rot = Quaternion.LookRotation(forward, up);
    //     return rot;
    // }

    public Onomatopoeia OnomatopoeiaAt(Vector3 pos, Quaternion rot, GameObject onoPrefab) {
        Onomatopoeia ono = Instantiate(onoPrefab, pos, rot).GetComponentInChildren<Onomatopoeia>();

        ono.TextFacePlayer(_TransformToFace, _FacePlayerInfo);

        ono.StartAnimation();
        return ono;
    }


    public Onomatopoeia OnomatopoeiaAt(Vector3 pos) {
        return OnomatopoeiaAt(pos, Quaternion.identity, _DefaultOnomatopoeiaPrefab);
    }

    public Onomatopoeia OnomatopoeiaAt(Vector3 pos, Quaternion rot) {
        return OnomatopoeiaAt(pos, rot, _DefaultOnomatopoeiaPrefab);
    }

    public Onomatopoeia OnomatopoeiaAt(Vector3 pos, GameObject onoPrefab) {
        return OnomatopoeiaAt(pos, Quaternion.identity, onoPrefab);
    }

    public Onomatopoeia OnomatopoeiaAtTransform() {
        return OnomatopoeiaAt(transform.position, Quaternion.identity, _DefaultOnomatopoeiaPrefab);
    }
}

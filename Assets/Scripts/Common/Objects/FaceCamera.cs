using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour {


    private void Start() {
        FaceTheCamera();
    }
    private void Update() {
        FaceTheCamera();
    }
    

    private void FaceTheCamera() {
        // Face the music, I mean camera haha!
        this.transform.LookAt (this.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }



}

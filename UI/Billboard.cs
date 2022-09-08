using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform MyCamera;
    // Update is called once per frame

    private void Start()
    {
        MyCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + MyCamera.forward);
    }
}

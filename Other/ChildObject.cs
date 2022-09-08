using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildObject : MonoBehaviour
{
    public MeshRenderer Parent;
    public MeshRenderer Renderer;


    public void SetColor()
    {
        Renderer.material.color = Parent.material.color;
    }


}

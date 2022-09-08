using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseOver : MonoBehaviour
{
    public bool mouseHere = false;

    private void OnMouseOver()
    {
        mouseHere = true;
    }
    private void OnMouseExit()
    {
        mouseHere = false;
    }
}

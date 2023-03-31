using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public bool interactable;
    public bool clickable;
    public bool isSoldier;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public virtual void SetClickable(bool b)
    {
        clickable = b;
    }

    public virtual void SetInteractable(bool b)
    {
        interactable = b;
    }

    public void SetAsSoldier()
    {
        isSoldier = true;
    }
}

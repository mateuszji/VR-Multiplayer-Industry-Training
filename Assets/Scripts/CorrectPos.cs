using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrectPos : MonoBehaviour
{
    [HideInInspector]
    public Replaceable replaceable;

    private void Start()
    {
        replaceable = null;
    }
}

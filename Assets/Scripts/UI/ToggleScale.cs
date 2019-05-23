using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScale : MonoBehaviour
{
    public Transform ObjToAnimate;
    public List<Vector3> Positions;
    
    [SerializeField]
    private int _index;

    public void SetIndex(int i)
    {
        _index = i;
        ObjToAnimate.localPosition = Positions[i];
    }

}

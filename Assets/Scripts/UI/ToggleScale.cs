using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScale : MonoBehaviour
{
    public int StartingIndex;
    public Transform ObjToAnimate;
    public List<Vector3> Positions;
    
    private int _index;

    private void Start()
    {
        _index = StartingIndex;
    }

    public void SetIndex(int i)
    {
        _index = i;
        ObjToAnimate.localPosition = Positions[i];
    }

}

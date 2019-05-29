using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeminoleReferenceController : MonoBehaviour
{

    public GameObject SeminoleReference;
    public Material LightMat;
    public Material DarkMat;

    private MarkerWrapper _enginePowerOutputLeft;
    private MarkerWrapper _enginePowerOutputRight;

    // Start is called before the first frame update
    void Start()
    {
        var FPALeft = GameObject.Find("FPALeft");
        var FPARight = GameObject.Find("FPARight");
        _enginePowerOutputLeft = new MarkerWrapper(FPALeft, LightMat, DarkMat);
        _enginePowerOutputRight = new MarkerWrapper(FPARight, LightMat, DarkMat);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncEngineOutputLeft(float value) => _enginePowerOutputLeft.Inc(value);
    public void DecEngineOutputLeft(float value) => _enginePowerOutputLeft.Dec(value);
    public void IncEngineOutputRight(float value) => _enginePowerOutputRight.Inc(value);
    public void DecEngineOutputRight(float value) => _enginePowerOutputRight.Dec(value);
}

public class MarkerWrapper
{
    private List<GameObject> _children;
    private int _numChildren => _children.Count;
    private float _percent = 0.0f;
    private Material _lightMat;
    private Material _darkMat;

    public MarkerWrapper(GameObject markerContainer, Material lightMat, Material darkMat)
    {
        var trans = markerContainer.GetComponent<Transform>();
        _children = new List<GameObject>();

        foreach (Transform child in trans)
            _children.Add(child.gameObject);

        _lightMat = lightMat;
        _darkMat = darkMat;
    }

    public void Inc(float value)
    {
        if (_percent + value > 100f) _percent = 100f;
        _percent += value;

    }

    public void Dec(float value)
    {
        if (_percent - value < 100f) _percent = 0f;
        _percent -= value;
    }

    public void ColorMarkers()
    {
        var numColored = Mathf.RoundToInt(_percent / 100f);
        for(int num = 0; num < numColored; num++)
        {
            _children[num].GetComponent<MeshRenderer>().material = _lightMat;
        }
    }
}

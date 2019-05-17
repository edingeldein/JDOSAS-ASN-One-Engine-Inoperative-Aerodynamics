using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ToggleGroupConfig : MonoBehaviour
{
    public int NumberOfButtons;
    public GameObject TogglePrefab;

    private List<GameObject> _toggleButtons;
    private ToggleGroup _toggleGroup;
    private GameObject _grouper;

    // Start is called before the first frame update
    void Start()
    {
        _grouper = transform.Find("Grouper").gameObject;
        _toggleGroup = _grouper.GetComponent<ToggleGroup>();

        _toggleButtons = GetToggleButtons(_grouper);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateButtons()
    {
        DestroyButtons();
        CreateButtons(NumberOfButtons);
    }

    private List<GameObject> GetToggleButtons(GameObject grouper)
    {
        var list = new List<GameObject>();
        foreach(Transform trans in grouper.transform)
        {
            list.Add(trans.gameObject);
        }
        return list;
    }

    private void DestroyButtons()
    {
        foreach (var toggle in _toggleButtons)
        {
            DestroyImmediate(toggle);
        }
        _toggleButtons = new List<GameObject>();
    }

    private void CreateButtons(int num)
    {
        for(int i = 0; i < NumberOfButtons; i++)
        {
            var toggle = Instantiate(TogglePrefab, _toggleGroup.transform);
            toggle.GetComponent<Toggle>().group = _toggleGroup.GetComponent<ToggleGroup>();

            _toggleButtons.Add(toggle);
        }
    }

}

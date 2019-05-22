using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeminoleController : MonoBehaviour
{

    /// <summary>
    /// Seminole Game Object houseing the .fbx model
    /// </summary>
    public GameObject Seminole;
    // Wrapper class for the seminole game object
    private SeminoleWrapper _seminoleWrap;
    
    /// <summary>
    /// Reference to the base .fbx seminole model
    /// </summary>
    private GameObject _seminoleModel;

    /// <summary>
    /// Seminole reference markers
    /// </summary>
    public GameObject SeminoleReference;

    // Inspector config variables
    public Side InopEngine;
    public PropMethod PropMethod;

    // private fields holding the state of the model
    private bool _updated;
    private int _power;
    private int _densityAlt;
    private int _airspeed;
    private int _weight;
    private int _cog;
    private int _flaps;
    private Gear _landingGear;

    private void Awake()
    {
        _seminoleWrap = new SeminoleWrapper(Seminole);
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (InopEngine == Side.Left) _seminoleWrap.SpinRight();
        else _seminoleWrap.SpinLeft();

        _seminoleWrap.PlayAnimation("WindmillProps");
        if (PropMethod == PropMethod.Feather) _seminoleWrap.SetAnimationSpeed(0);
        else _seminoleWrap.SetAnimationSpeed(1);

    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void SwitchEngine() => _seminoleWrap.SwitchEngine();
    public void ToggleAnimation() => _seminoleWrap.ToggleAnimation();

}

public class SeminoleWrapper
{
    private Transform _transform;
    private Animator _anim;
    private Propeller _left;
    private Propeller _right;
    private bool _animating => _anim.speed > 0;

    public SeminoleWrapper(GameObject seminole)
    {
        _transform = seminole.GetComponent<Transform>();
        _anim = seminole.GetComponent<Animator>();        
        ConfigureProps();
    }

    public void ToggleAnimation()
    {
        if (_animating) SetAnimationSpeed(0);
        else SetAnimationSpeed(1);
    }

    public void PlayAnimation(string name)
    {
        _anim.Play(name);
    }

    public void SetAnimationSpeed(int speed)
    {
        _anim.speed = speed;
    }

    public void SwitchEngine()
    {
        _left.Toggle();
        _right.Toggle();
    }

    public void SpinLeft()
    {
        _left.Spin();
        _right.Stop();
    }

    public void SpinRight()
    {
        _left.Stop();
        _right.Spin();
    }

    private void ConfigureProps()
    {
        var props = new List<GameObject>(GameObject.FindGameObjectsWithTag("Propellers"));
        var leftSpinningProp = props.Find(go => go.name.Equals("body_wing-propeller-left"));
        var rightSpinningProp = props.Find(go => go.name.Equals("body_wing-propeller-right"));
        var leftPropBlades = props.Find(go => go.name.Equals("body_wing-propeller-blades-left"));
        var rightPropBlades = props.Find(go => go.name.Equals("body_wing-propeller-blades-right"));

        _left = new Propeller(leftPropBlades, leftSpinningProp);
        _right = new Propeller(rightPropBlades, rightSpinningProp);
    }
}

/// <summary>
/// Contains propeller state info and manipulation
/// </summary>
public class Propeller
{
    private bool _running;
    private GameObject _blades;
    private GameObject _spin;

    public Propeller(GameObject blades, GameObject spin)
    {
        _running = true;
        _blades = blades;
        _spin = spin;
    }

    public void Toggle()
    {
        if (_running)
            Stop();
        else
            Spin();
    }

    public void Spin()
    {
        _blades.SetActive(false);
        _spin.SetActive(true);
        _running = true;
    }

    public void Stop()
    {
        _blades.SetActive(true);
        _spin.SetActive(false);
        _running = false;
    }
}

public enum Side
{ Left, Right }
public enum PropMethod
{ Windmill, Feather }
public enum Gear
{ Up, Down }
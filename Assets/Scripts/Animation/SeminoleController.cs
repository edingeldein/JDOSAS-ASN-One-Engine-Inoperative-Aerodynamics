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
    public GearState GearState;
    public CtrlTechnique ControlTechnique;
    [Range(0.1f,5f)] public float BankAngle;
    [Range(0.1f,10f)] public float YawAngle;

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
        _seminoleWrap = new SeminoleWrapper(Seminole, BankAngle, YawAngle);
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (InopEngine == Side.Left) _seminoleWrap.SpinRight();
        else _seminoleWrap.SpinLeft();

        _seminoleWrap.PlayAnimation("WindmillProps");
        if (PropMethod == PropMethod.Feather) _seminoleWrap.SetAnimationSpeed(0);
        else _seminoleWrap.SetAnimationSpeed(1);

        if (GearState == GearState.Up) _seminoleWrap.GearUp();
        else _seminoleWrap.GearDown();

        _seminoleWrap.SetOrientation(ControlTechnique, InopEngine);

    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void SwitchEngine() => _seminoleWrap.SwitchEngine();
    public void ToggleOrientation() => _seminoleWrap.ToggleOrientation();
    public void ToggleCtrlTech() => _seminoleWrap.ToggleCtrlTech();
    public void ToggleAnimation() => _seminoleWrap.ToggleAnimation();
    public void ToggleGear() => _seminoleWrap.ToggleGear();

}

public class SeminoleWrapper
{    
    private Transform _transform;
    private Transform _centerline;
    private Animator _anim;
    private Propeller _left;
    private Propeller _right;
    private Gear _landingGear;
    private float _bankAngle;
    private float _yawAngle;
    private Side _inopEngine;
    private OrientDir _direction;
    private CtrlTechnique _ctrlTech;
    private bool _animating => _anim.speed > 0;

    public SeminoleWrapper(GameObject seminole, float bankAngle, float yawAngle)
    {
        _transform = seminole.GetComponent<Transform>();
        _centerline = GameObject.FindGameObjectWithTag("Centerline")?.transform;
        _anim = seminole.GetComponent<Animator>();
        _bankAngle = bankAngle;
        _yawAngle = yawAngle;
        ConfigureProps();
        ConfigureGear();
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

    public void ToggleOrientation()
    {
        if(_ctrlTech == CtrlTechnique.WingsLevel)
        {
            Level();
            ToggleYaw();
        }
        else
        {
            NoSlip();
            ToggleBank();
        }
    }

    public void SetOrientation(CtrlTechnique ctrlTech, Side inopEngine)
    {
        _ctrlTech = ctrlTech;
        _direction = (inopEngine == Side.Left) ? OrientDir.Right : OrientDir.Left;
        Reorient();
    }

    public void Reorient()
    {
        if(_ctrlTech == CtrlTechnique.WingsLevel)
        {
            Level();
            if (_direction == OrientDir.Left) YawLeft(_yawAngle);
            else YawRight(_yawAngle);
        }
        else
        {
            NoSlip();
            if (_direction == OrientDir.Left) BankLeft(_bankAngle);
            else BankRight(_bankAngle);
        }
    }

    public void ToggleCtrlTech()
    {
        _ctrlTech = (_ctrlTech == CtrlTechnique.WingsLevel) 
            ? CtrlTechnique.ZeroSideSlip : CtrlTechnique.WingsLevel;
        Reorient();
    }

    public void ToggleBank()
    {
        if (_direction == OrientDir.Left) BankRight(_bankAngle);
        else BankLeft(_bankAngle);
    }

    public void BankLeft(float angle)
    {
        Bank(angle);
        _direction = OrientDir.Left;
    }

    public void BankRight(float angle)
    {
        Bank(-1f * angle);
        _direction = OrientDir.Right;
    }

    public void Level()
    {
        Bank(0f);
    }

    private void Bank(float angle)
    {
        _transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void ToggleYaw()
    {
        if (_direction == OrientDir.Right) YawLeft(_bankAngle);
        else YawRight(_bankAngle);
    }

    public void YawLeft(float angle)
    {
        Yaw(-1f * angle);
        _direction = OrientDir.Left;
    }

    public void YawRight(float angle)
    {
        Yaw(angle);
        _direction = OrientDir.Right;
    }

    public void NoSlip() => Yaw(0f);

    public void Yaw(float angle)
    {
        _transform.rotation = Quaternion.Euler(0f, angle, 0f);
        _centerline.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public void ToggleGear() => _landingGear.Toggle();

    public void GearUp() => _landingGear.SetUp();

    public void GearDown() => _landingGear.SetDown();

    private void ConfigureGear()
    {
        var gearAssm = new List<GameObject>(GameObject.FindGameObjectsWithTag("Gear"));
        _landingGear = new Gear(gearAssm);
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

public class Gear
{
    private List<GameObject> _gearParts;
    private bool _down;

    public Gear(List<GameObject> gearParts)
    {
        _gearParts = gearParts;
        _down = true;
    }

    public void Toggle()
    {
        if (_down) SetUp();
        else SetDown();
    }

    public void SetUp()
    {
        Set(false);
        _down = false;
    }

    public void SetDown()
    {
        Set(true);
        _down = true;
    }

    private void Set(bool val)
    {
        foreach (var part in _gearParts)
            part.SetActive(val);
    }
}

public enum Side
{ Left, Right }
public enum OrientDir
{ Left,Right }
public enum PropMethod
{ Windmill, Feather }
public enum GearState
{ Up, Down }
public enum CtrlTechnique
{ WingsLevel, ZeroSideSlip }
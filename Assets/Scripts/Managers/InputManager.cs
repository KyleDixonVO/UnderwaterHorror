using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager inputManager;

    private bool _escapePressed;
    public bool escapePressed;

    private bool _spacedPressed;
    public bool spacePressed;

    private bool _wPressed;
    public bool wPressed;

    private bool _sPressed;
    public bool sPressed;

    private bool _aPressed;
    public bool aPressed;

    private bool _dPressed;
    public bool dPressed;

    private bool _ePressed;
    public bool ePressed;
    public bool eCycled;

    private bool _capsPressed;
    public bool capsPressed;

    private void Awake()
    {
        if (inputManager == null)
        {
            inputManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (inputManager != null && inputManager != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _escapePressed = false;
        eCycled = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInputs();
    }

    public void ResetEscape()
    {
        _escapePressed = false;
    }

    void UpdateInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _escapePressed = !_escapePressed;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _spacedPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            _spacedPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            _wPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            _wPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _sPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            _sPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _aPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            _aPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            _dPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            _dPressed = false;
        }

        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            _capsPressed = true;
        }
        else if (Input.GetKeyUp(KeyCode.CapsLock))
        {
            _capsPressed = false;
        }

        if (Input.GetKey(KeyCode.E))
        {
            _ePressed = true;
            //Debug.Log(_ePressed);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            _ePressed = false;
            //Debug.Log(_ePressed);
            eCycled = true;
        }

        escapePressed = _escapePressed;
        spacePressed = _spacedPressed;
        wPressed = _wPressed;
        sPressed = _sPressed;
        dPressed = _dPressed;
        aPressed = _aPressed;
        capsPressed = _capsPressed;
        ePressed = _ePressed;
    }


}
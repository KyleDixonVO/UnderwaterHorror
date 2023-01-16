using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats playerStats;

    [Header("Suit Settings")]
    public float maxSuitPower = 100.0f;
    public float suitPower;
    [SerializeField] public float drainPerSecond = 0.67f;
    [SerializeField] public float rechargePerSecond = 2.0f;
    [SerializeField] public float sprintDrainPerSecond = 1.0f;
    [SerializeField] public float drainPerDash = 5.0f;
    [SerializeField] public float suffocationDamage = 1.0f;

    [Header("Player Stats")]
    public float maxPlayerHealth = 100.0f;
    public float playerHealth = 100.0f;

    [Header("Spotlight Settings")]
    [SerializeField] public float poweredRange = 60.0f;
    [SerializeField] public float unpoweredRange = 20.0f;
    [SerializeField] public Color poweredColor;
    [SerializeField] public Color unpoweredColor;
    [SerializeField] private Light suitSpotlight;


    private void Awake()
    {
        if (playerStats == null)
        {
            playerStats = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (playerStats != this && playerStats != null)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        suitPower = maxSuitPower;
        playerHealth = maxPlayerHealth;
        
    }

    // Update is called once per frame
    void Update()
    {
        FindSpotlightRef();
        ToggleSuitSpotlight();
    }

    public void FindSpotlightRef()
    {
        if (suitSpotlight != null) return;
        suitSpotlight = GameObject.Find("SuitSpotLight").GetComponent<Light>();
    }

    public void SuitDashDrain()
    {
        suitPower -= drainPerDash;
    }

    public void SuitSprintDrain()
    {
        suitPower -= sprintDrainPerSecond * Time.deltaTime;
    }

    public void SuitPowerLogic()
    {
        if (suitPower <= 0)
        {
            suitPower = 0;
            Debug.Log("Suit Power Depleted");
            TakeDamage(suffocationDamage * Time.deltaTime);
            this.gameObject.transform.GetComponentInChildren<Light>().color = unpoweredColor;
            this.gameObject.transform.GetComponentInChildren<Light>().range = unpoweredRange;
        }
        else
        {
            this.gameObject.transform.GetComponentInChildren<Light>().color = poweredColor;
            this.gameObject.transform.GetComponentInChildren<Light>().range = poweredRange;
        }
        suitPower -= drainPerSecond * Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Taking Damage");
        if (playerHealth <= 0)
        {
            Debug.Log("Player Health Depleted");
            playerHealth = 0;
            return;
        }

        if (damage <= 0) return;
        playerHealth -= damage;
    }

    public void RechargeSuit()
    {
        if (FirstPersonController_Sam.fpsSam.inWater || suitPower == maxSuitPower) return;
        suitPower += rechargePerSecond * Time.deltaTime;
        if (suitPower > maxSuitPower) suitPower = maxSuitPower;
    }

    public void ToggleSuitSpotlight()
    {
        if (FirstPersonController_Sam.fpsSam.inWater)
        {
            suitSpotlight.enabled = true;
        }
        else
        {
            suitSpotlight.enabled = false;
        }
    }
}
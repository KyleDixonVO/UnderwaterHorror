using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnderwaterHorror
{
    public class BatteryPack : Item
    {
        [Header("stats")]
        [SerializeField]
        float powerGain = 30f;

        void Start()
        {
            usageTime = 2;
            usageProgress = usageTime;
            typeName = "BatteryPack";

            // Tobias is nuts
            itemAudioSource = this.gameObject.GetComponent<AudioSource>();
            _outline = GetComponent<Outline>();
            _outline.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (GameManager.gameManager.gameState != GameManager.gameStates.gameplay || UI_Manager.ui_Manager.PDAOpen())
            {
                AudioManager.audioManager.PauseSound(itemAudioSource); 
                return;
            }

            if (beingUsed)
            {               
                TimeToEffect();
            }

            if (FirstPersonController_Sam.fpsSam.carryingHeavyObj)
            {
                return;
            }

            if (isEquiped && Input.GetKeyDown(KeyCode.Mouse0) && beingUsed == false && !isUsed)
            {
                AudioManager.audioManager.PlaySound(itemAudioSource, AudioManager.audioManager.batteryUsed);
                beingUsed = true;
                //TimeToEffect();
            }

            if (gameObject.activeSelf == false)
            {
                AudioManager.audioManager.StopSound(itemAudioSource);
            }
        }

        protected override void ApplyEffect()
        {      
            PlayerStats.playerStats.suitPower += powerGain;
            Debug.LogWarning("works");

            if (PlayerStats.playerStats.suitPower > PlayerStats.playerStats.maxSuitPower)
            {
                PlayerStats.playerStats.suitPower = PlayerStats.playerStats.maxSuitPower;
            }

            // deactivates the object not allowing it to be used
            gameObject.SetActive(false);
            //isUsed = true;

        }

        public override void ResetForNewRun()
        {
            base.ResetForNewRun();
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnderwaterHorror
{
    public class ElectroProd : Weapon
    {
        // Start is called before the first frame update
        void Start()
        {
            // set weapon stats
            damage = 8;
            range = 4;
            maxAmmo = 3;
            reserves = maxAmmo;
            currentAmmo = 1;
            reloadTime = 2;
            typeName = "ElectroProd";

            // set bools
            canShoot = true;
            isEquiped = false;

            // Tobias 0<0
            weaponAudioSource = this.gameObject.GetComponent<AudioSource>();
            _outline = GetComponent<Outline>();
            _outline.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (GameManager.gameManager.gameState != GameManager.gameStates.gameplay || UI_Manager.ui_Manager.PDAOpen())
            {
                AudioManager.audioManager.PauseSound(weaponAudioSource);
                return;
            }

            // RELOAD COUTER
            if (reloadProgress <= 0 && !canShoot && reserves > 0)
            {
                reloadProgress = 0;
                AudioManager.audioManager.StopSound(weaponAudioSource);
                canShoot = true;
                currentAmmo++;
                reserves--;
            }
            else if (reloadProgress > 0 && isEquiped)
            {
                AudioManager.audioManager.PlaySound(weaponAudioSource, AudioManager.audioManager.electricProdRecharge);
                reloadProgress -= Time.deltaTime;
            }

            // SHOOTING
            if (Input.GetKeyDown(KeyCode.Mouse0) && currentAmmo > 0 && canShoot && isEquiped)
            {
                AudioManager.audioManager.PlaySound(weaponAudioSource, AudioManager.audioManager.electricProdShock);
                ShootWeapon(damage);
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0) && currentAmmo <= 0 && isEquiped)
            {
                // no ammo sound
                AudioManager.audioManager.PlaySound(weaponAudioSource, AudioManager.audioManager.electricProdNoCharge);
            }

            if (FirstPersonController_Sam.fpsSam.carryingHeavyObj)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                //EquipUnequip();
            }
        }

        public override void ResetForNewRun()
        {
            base.ResetForNewRun();
            gameObject.GetComponent<BoxCollider>().enabled = true;
        }
    }

}

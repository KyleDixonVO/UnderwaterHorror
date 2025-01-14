using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnderwaterHorror
{
    public class HarpoonGun : Weapon
    {
        // We might want to put items like this on a diffrent rendering plane / canvas to prevent
        // clipping with the enviroment - Edmund

        // Start is called before the first frame update
        void Start()
        {
            // set weapon stats
            damage = 5;
            range = 15;
            maxAmmo = 2;
            reserves = maxAmmo;
            currentAmmo = 1;
            reloadTime = 2;
            typeName = "HarpoonGun";

            // set bools
            canShoot = true;
            isEquiped = false;

            // Tobias ;)
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
                AudioManager.audioManager.StopSound(weaponAudioSource);
                reloadProgress = 0;
                canShoot = true;
                currentAmmo++;
                reserves--;
            }
            else if (reloadProgress > 0 && isEquiped)
            {
                AudioManager.audioManager.PlaySound(weaponAudioSource, AudioManager.audioManager.harpoonReloading); // - Tobias Audio
                reloadProgress -= Time.deltaTime;
            }

            // SHOOTING
            if (Input.GetKeyDown(KeyCode.Mouse0) && currentAmmo > 0 && canShoot && isEquiped)
            {
                AudioManager.audioManager.PlaySound(weaponAudioSource, AudioManager.audioManager.harpoonShot); // - Tobias Audio
                ShootWeapon(damage);
            }
            else if (Input.GetKeyDown(KeyCode.Mouse0) && currentAmmo <= 0 && isEquiped)
            {
                // no ammo sound
                AudioManager.audioManager.PlaySound(weaponAudioSource, AudioManager.audioManager.harpoonNoAmmo); // - Tobias Audio
            }

            if (FirstPersonController_Sam.fpsSam.carryingHeavyObj)
            {
                return;
            }
        }

        public override void ResetForNewRun()
        {
            base.ResetForNewRun();
            gameObject.GetComponent<BoxCollider>().enabled = true;
        }
    }

}

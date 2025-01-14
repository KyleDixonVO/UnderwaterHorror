using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnderwaterHorror
{
    public class HeavyObject : Interactable
    {
        // Tobias
        [Header("Outline")]
        [SerializeField] public Outline _outline;

        [SerializeField] private bool _isHeld = false;
        public bool isHeld;
        [SerializeField] string tooltip;
        [SerializeField] private Vector3 heldPos;
        [SerializeField] private Vector3 heldRot;
        [SerializeField] private float interactDistance = 5;
        // Start is called before the first frame update
        void Start()
        {
            _outline.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateObjectParent();
            UpdateHeldLocalPosition();
            isHeld = _isHeld;
        }

        // Interact system
        public override void OnInteract()
        {
            if (WithinPickupRange()
                && !_isHeld
                && FirstPersonController_Sam.fpsSam.carryingHeavyObj == false)
            {
                Debug.Log("Picked up heavy object");
                AudioManager.audioManager.PlaySound(this.gameObject.GetComponent<AudioSource>(), AudioManager.audioManager.pickupPipe);
                _isHeld = true;
                FirstPersonController_Sam.fpsSam.carryingHeavyObj = true;
            }
            else if (_isHeld)
            {
                Debug.Log("Dropped heavy object");
                AudioManager.audioManager.PlaySound(this.gameObject.GetComponent<AudioSource>(), AudioManager.audioManager.dropPipe);
                _isHeld = false;
                FirstPersonController_Sam.fpsSam.carryingHeavyObj = false;
            }
        }

        public override void OnFocus()
        {
            Debug.LogWarning("Looking at pipe");
            UI_Manager.ui_Manager.ActivatePrimaryInteractText(tooltip);

            if (this.gameObject.GetComponent<Outline>() != null && transform.parent != FirstPersonController_Sam.fpsSam.playerCamera.gameObject)
            {
                Debug.Log("Setting active outline");
                Outline.activeOutline = gameObject.GetComponent<Outline>();
                _outline.enabled = true;
            }
        }

        public override void OnLoseFocus()
        {
            UI_Manager.ui_Manager.DisablePrimaryInteractText();
            //Debug.LogWarning("not looking at pipe");
            _outline.enabled = false;
        }

        void UpdateObjectParent()
        {
            if (_isHeld && this.gameObject.transform.parent == null)
            {
                this.gameObject.transform.parent = FirstPersonController_Sam.fpsSam.transform;
            }
            else if (!_isHeld)
            {
                this.gameObject.transform.parent = null;
            }
        }

        void UpdateHeldLocalPosition()
        {
            if (this.gameObject.transform.parent != null)
            {
                this.gameObject.transform.localPosition = heldPos;
                this.gameObject.transform.rotation = transform.parent.transform.localRotation;
            }
        }

       public void ForceDropObject()
       {
            FirstPersonController_Sam.fpsSam.carryingHeavyObj = false;
            //InputManager.inputManager.eCycled = true;
            _isHeld = false;
            UpdateObjectParent();
       }

        public bool WithinPickupRange()
        {
            if (Vector3.Distance(this.transform.position, FirstPersonController_Sam.fpsSam.transform.position) < interactDistance) return true;
            return false;
        }

        public override void ResetForNewRun()
        {
            base.ResetForNewRun();
            _isHeld = false;
        }

        public void SetSavePos()
        {
            savePos = this.transform.position;
        }

        public void ReturnToSavePos()
        {
            this.transform.position = savePos;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnderwaterHorror
{
    public class MiniButton : Interactable
    {
        public AirlockMini door;
        [SerializeField] string tooltip;
        [SerializeField] AudioSource source;

        public override void OnInteract()
        {
            if (this.GetComponent<AudioSource>().isPlaying == false)
            {
                door.buttonPress = true;
                AudioManager.audioManager.PlaySound(source, AudioManager.audioManager.doorOpened);
            }
        }

        public override void OnFocus()
        {
            UI_Manager.ui_Manager.ActivatePrimaryInteractText(tooltip);
        }

        public override void OnLoseFocus()
        {
            UI_Manager.ui_Manager.DisablePrimaryInteractText();
        }
    }
}


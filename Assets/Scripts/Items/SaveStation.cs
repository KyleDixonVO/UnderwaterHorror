using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnderwaterHorror 
{
    public class SaveStation : Interactable
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void OnFocus()
        {
            UI_Manager.ui_Manager.ActivatePrimaryInteractText();
        }

        public override void OnLoseFocus()
        {
            UI_Manager.ui_Manager.ActivatePrimaryInteractText();
        }

        public override void OnInteract()
        {
            GameManager.gameManager.SaveGame();
        }
    }
}


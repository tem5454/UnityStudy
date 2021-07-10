using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NPCDialog
{
    public enum NPCInteractableType
    {
        None = 0,
        Range = 1,
        Raycast = 2,
        End,
    }
    public class NPCBase : MonoBehaviour
    {
        public NPCInteractableType InteractType;
        public List<string> DialogText = new List<string>();

        public void OnTriggerEnter(Collider other)
        {
            if(InteractType == NPCInteractableType.Range)
            {
                if(other.CompareTag("Player"))
                {
                    var player = other.GetComponent<survival.SurvivalCharacter>();
                    player.NotifyInteractNPC(this, true);
                }                    
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (InteractType == NPCInteractableType.Range)
            {
                if (other.CompareTag("Player"))
                {
                    var player = other.GetComponent<survival.SurvivalCharacter>();
                    player.NotifyInteractNPC(this, false);
                }
            }
        }
    }
}


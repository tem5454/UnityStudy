using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace survival
{
    public class ActionAreaBase : MonoBehaviour
    {
        public ActionType actionType;
        public float duration;
        public Transform ActionPos;
        public Transform ActionItem;
        public Transform ActionItemOriginalPos;


        private readonly float CorrectDistance = 0.01f;
        private readonly float CorrectTime = 1.0f;

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                var sc = other.GetComponent<SurvivalCharacter>();
                if(sc)
                {                    
                    StartCoroutine(CoCorrectCharacterPosition(sc));
                }
            }
        }

        IEnumerator CoCorrectCharacterPosition(SurvivalCharacter target)
        {
            yield return null;

            float distance = Vector3.Distance(target.transform.position, ActionPos.position);

            while (distance > CorrectDistance)
            {
                target.transform.position = Vector3.Slerp(target.transform.position, ActionPos.position, 0.1f);                

                distance = Vector3.Distance(target.transform.position, ActionPos.position);

                yield return null;
            }

            target.TriggerAction(this);
        }
    }
}


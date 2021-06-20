using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace survival
{
    public enum ActionType
    {
        None = 0,
        Cook = 1,
        ShakeTree = 2,
        End,
    }

    public class SurvivalCharacter : MonoBehaviour
    {
        public Animator CharacterAnimator;

        public float Speed;
        public float RotSpeed;
        public float ActionDuration;

        float rotationX = 0.0f;  
        float rotationY = 0.0f;  

        public bool IsAction;
        public bool IsMove;

        public Transform HandItemRoot;
        public Transform ModelTransform;

        public ActionAreaBase action;

        void Update()
        {
            Move();
        }

        public void Move()
        {
            if (IsAction)
                return;

            IsMove = false;

            if (Input.GetKey(KeyCode.W))
            {
                IsMove = true;
                transform.position += Vector3.forward * Speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                IsMove = true;
                transform.position += Vector3.back * Speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                IsMove = true;
                transform.position += Vector3.left * Speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                IsMove = true;
                transform.position += Vector3.right * Speed * Time.deltaTime;
            }

            if(Input.GetKey(KeyCode.Mouse0))
            {
                IsMove = true;

                float x = Input.GetAxis("Mouse X");
                rotationX += x * RotSpeed;
                transform.rotation = Quaternion.Euler(0.0f, rotationX, 0.0f);
                ModelTransform.rotation = transform.rotation;

            }

            CharacterAnimator.SetBool("IsMove", IsMove);
        }

        public void TriggerAction(ActionAreaBase action)
        {
            this.action = action;

            ActionDuration = action.duration;
            IsAction = true;
            CharacterAnimator.SetBool("IsAction", IsAction);          

            if(action.ActionItem)
            {
                action.ActionItem.parent = HandItemRoot;
                action.ActionItem.localPosition = Vector3.zero;
                action.ActionItem.localRotation = Quaternion.identity;
            }

            switch (action.actionType)
            {
                case ActionType.Cook:
                {
                    CharacterAnimator.SetTrigger("CookTrigger");                   
                    break;
                }
                case ActionType.ShakeTree:
                {
                    CharacterAnimator.SetTrigger("ShakeTreeTrigger");
                    break;
                }
                default:
                {
                    break;
                }                
            }

            Invoke(nameof(InvokeActionCancel), ActionDuration);
        }

        public void InvokeActionCancel()
        {
            IsAction = false;
            CharacterAnimator.SetBool("IsAction", IsAction);

            if (action.ActionItem)
            {
                this.action.ActionItem.parent = this.action.transform;
                this.action.ActionItem.position = this.action.ActionItemOriginalPos.position;
                this.action.ActionItem.rotation = this.action.ActionItemOriginalPos.rotation;
            }
        }
    }

}
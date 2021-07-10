using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        public GameObject interactUI;
        public GameObject DialogUI;
        public Text DialogText;

        public bool IsInteractable;
        public bool IsInteractProgress;        

        public NPCDialog.NPCBase InteractableNPC;

        public int DialogIndex;


        private void Start()
        {
            interactUI.SetActive(false);
            DialogUI.SetActive(false);
            IsInteractable = false;
            IsInteractProgress = false;
        }

        void Update()
        {
            Move();
            Interact();

            if(IsInteractProgress == false)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    OnClick(Input.mousePosition);
                }
            }                        
        }

        public void OnClick(Vector3 pos)
        {
            Ray ray = Camera.main.ScreenPointToRay(pos);
            if(Physics.Raycast(ray, out RaycastHit hitInfo ,50))
            {
                if(hitInfo.collider.CompareTag("NPC"))
                {
                    interactUI.SetActive(true);
                    IsInteractable = true;
                    InteractableNPC = hitInfo.collider.GetComponent<NPCDialog.NPCBase>();
                    DialogIndex = 0;
                }
                else
                {
                    interactUI.SetActive(false);
                }
            }
        }

        public void Interact()
        {
            if(Input.GetKeyDown(KeyCode.F))
            {                
                if (IsInteractable == false)
                {
                    return;
                }

                if(InteractableNPC != null)
                {
                    DialogUI.SetActive(true);
                    if (IsInteractProgress == false)
                    {
                        StartCoroutine(CoAutoDialogText());
                    }
                    IsInteractProgress = true;
                }               
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(IsInteractProgress)
                {
                    IsInteractProgress = false;
                    DialogUI.SetActive(false);
                    DialogText.text = string.Empty;

                    interactUI.SetActive(false);
                }                
            }

            if(Input.GetKeyDown(KeyCode.Mouse0))
            {
                if(IsInteractProgress)
                {                    
                    ++DialogIndex;
                    if (DialogIndex < InteractableNPC.DialogText.Count)
                    {
                        DialogText.text = InteractableNPC.DialogText[DialogIndex];
                    }
                }
            }
        }

        public IEnumerator CoAutoDialogText()
        {
            while(DialogIndex < InteractableNPC.DialogText.Count)
            {
                yield return null;

                DialogText.text = InteractableNPC.DialogText[DialogIndex];

                yield return new WaitForSeconds(2.0f);

                ++DialogIndex;
            }

            IsInteractProgress = false;
            interactUI.SetActive(false);
            DialogUI.SetActive(false);
        }

        public void Move()
        {
            if (IsInteractProgress)
                return; 

            if (IsAction)
                return;

            IsMove = false;

            if (Input.GetKey(KeyCode.W))
            {
                IsMove = true;
                transform.position += transform.forward * Speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.S))
            {
                IsMove = true;
                transform.position += (transform.forward * -1) * Speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.A))
            {
                IsMove = true;
                transform.position += (transform.right * -1) * Speed * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                IsMove = true;
                transform.position += transform.right * Speed * Time.deltaTime;
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

        public void NotifyInteractNPC(NPCDialog.NPCBase npcBase, bool isInOut)
        {
            interactUI.SetActive(isInOut);
            IsInteractable = isInOut;
            InteractableNPC = npcBase;
            DialogIndex = 0;
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
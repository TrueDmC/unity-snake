using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace Jogo.FPS.Multiplayer
{
    public class Motion : MonoBehaviourPunCallbacks
    {

        public float speed;
        public float sprintModifier;
        public float JumpForce;
        public Camera normalCam;
        public GameObject camParent;
        public Transform weaponParent;
        public Rigidbody rig;

        public Vector3 targetBobPosition;
        private Vector3 weaponParentOrigin;
        public float movementCounter;
        public float idleCounter;
        private float basefOV;
        public float sprintFOVModifier = 1.5f;



        private void Start()
        {
          
                camParent.SetActive(photonView.IsMine);
            if (photonView.IsMine) gameObject.layer = 11;

         
            basefOV = normalCam.fieldOfView;
            if(Camera.main) 
                    Camera.main.enabled = false;

            weaponParentOrigin = weaponParent.localPosition;

        }
        private void Update()
        {
            if (!photonView.IsMine) 
                return;


            bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);
        }

       void FixedUpdate()
        {
            if (!photonView.IsMine) return;

            //Axis
            float hmove = Input.GetAxis("Horizontal");
            float vmove = Input.GetAxis("Vertical");


            bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool jump = Input.GetKeyDown(KeyCode.Space);


            //States
            bool isJumping = jump;
            bool isSprinting = sprint && vmove > 0;

            //Jumping
            if (isJumping && transform.position.y < 1.5)
                rig.AddForce(Vector3.up * JumpForce);

            //Head bob

            if (hmove == 0 && vmove == 0) {
                Headbob(idleCounter, 0.01f, 0.01f); idleCounter += Time.deltaTime; 
            }
            else { 
                Headbob(movementCounter, 0.01f, 0.01f); movementCounter += Time.deltaTime * 0.01f; 
            }
            
            weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetBobPosition, Time.deltaTime * 0.01f);

           

            //Movement
            Vector3 t_direction = new Vector3(hmove, 0, vmove);
            t_direction.Normalize();

            float adjustedSpeed = speed;
            if (isSprinting) adjustedSpeed *= sprintModifier;

            Vector3 targetVelocity = transform.TransformDirection(t_direction) * adjustedSpeed * Time.deltaTime;
            targetVelocity.y = rig.velocity.y;

            //Field of View
            if (isSprinting) { 
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, basefOV * sprintFOVModifier, Time.deltaTime * 1f); 
            }
            else { 
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, basefOV, Time.deltaTime);
            }

            rig.velocity = targetVelocity;
        }

        void Headbob(float p_z, float p_x_intensity, float p_y_intensity)
        {
            targetBobPosition = weaponParent.localPosition = weaponParentOrigin + new Vector3(Mathf.Cos(p_z * 2) * (p_x_intensity), Mathf.Sin(p_z * 2) * p_y_intensity, weaponParentOrigin.z);
        }
    }
}



        

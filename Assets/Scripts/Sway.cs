using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace Jogo.FPS.Multiplayer
{ 
public class Sway : MonoBehaviourPunCallbacks
    {
    public float intensity;
    public float smooth;
        public Transform player;
        public Quaternion origin_rotation;

        public void Start()
        {
            player = transform.root;
            origin_rotation = transform.localRotation;
        }
        public void Update()
        {
     
            UpdateSway();

        }

        public void UpdateSway()

        {
            float t_x_mouse = Input.GetAxis("Mouse X");
            float t_y_mouse = Input.GetAxis("Mouse Y");

            Quaternion t_x_adj = Quaternion.AngleAxis(-intensity * t_x_mouse, Vector3.up);
            Quaternion t_y_adj = Quaternion.AngleAxis(intensity * t_y_mouse, Vector3.right);
            Quaternion target_rotation = origin_rotation * t_x_adj * t_y_adj;


            transform.localRotation = Quaternion.Lerp(transform.localRotation, target_rotation, Time.deltaTime * smooth);
        }



    }
}

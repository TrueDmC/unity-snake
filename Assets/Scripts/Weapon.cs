using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Photon.Pun;

namespace Jogo.FPS.Multiplayer
{
    public class Weapon : MonoBehaviourPunCallbacks
    {
        public Gun[] loadout;
        public Transform weaponParent;
        public GameObject BulletHolePrefab;
        private int currentIndex;
        private GameObject currentWeapon;
        public LayerMask canBeShot;
        public float currentCooldown;

        public Transform prefab;

        public Camera cam;


        private Vector3 Find(string v1, float v2)
        {
            throw new NotImplementedException();
        }


        void Update()
        {
            if (!photonView.IsMine) return;

            if (Input.GetKeyDown(KeyCode.Alpha1)) { photonView.RPC("Equip", RpcTarget.All, 0); }

            if (currentWeapon != null)
            {

                Aim(Input.GetMouseButton(1));

                if (Input.GetMouseButtonDown(0) && currentCooldown <= 0)
                {
                    Shoot();
                }

                //Weapon Position
                currentWeapon.transform.localPosition = (Vector3.Lerp(currentWeapon.transform.localPosition, weaponParent.transform.localPosition, Time.deltaTime) * 1f);

                //another cooldown if
                if (currentCooldown > 0)
                    currentCooldown -= Time.deltaTime;
            }
        }
        
        [PunRPC]
        void RPCShoot(float px, float py, float pz, float nx, float ny, float nz)
        {
            InstanciaHole(px, py, pz, nx, ny, nz);
        }

        void Shoot()
        {

            Transform spawn = transform.Find("Cameras/Normal Camera");

            //Bloom
            Vector3 bloom = spawn.position + spawn.forward * 1000f;
            bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * spawn.up;
            bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * spawn.right;
            bloom -= spawn.position;
            bloom.Normalize();


            ///Cooldown
            currentCooldown = loadout[currentIndex].firerate;


            // Raycast
            RaycastHit hit = new RaycastHit();
            Physics.Raycast(spawn.position, bloom, out hit, 1000f, canBeShot);
            {
                InstanciaHole(hit.point.x, hit.point.y, hit.point.z, hit.normal.x, hit.normal.y, hit.normal.z);
                photonView.RPC("RPCShoot", RpcTarget.Others, hit.point.x, hit.point.y, hit.point.z, hit.normal.x, hit.normal.y, hit.normal.z);

                if (hit.collider.gameObject.layer == 11)
                {

                }

            }


            //gun 

            currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
            currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;


        }

        void InstanciaHole(float px, float py, float pz, float nx, float ny, float nz)
        {
            Vector3 point = new Vector3(px, py, pz);
            Vector3 normal = new Vector3(nx, ny, nz);
            
            GameObject newHole = Instantiate(BulletHolePrefab, point + normal * 0.001f, Quaternion.identity) as GameObject;
            newHole.transform.LookAt(point + normal);
            Destroy(newHole, 5f);
        }

        [PunRPC]
        void Equip(int p_ind)
        {

            currentIndex = p_ind;

            if (currentWeapon != null)
                Destroy(currentWeapon);

            GameObject newWeapon = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            newWeapon.transform.SetParent(cam.transform);

            newWeapon.transform.localEulerAngles = Vector3.zero;
            newWeapon.GetComponent<Sway>().enabled = photonView.IsMine;
            currentWeapon = newWeapon;


        }


        void Aim(bool p_isAiming)
        {
            if (currentWeapon == null)
                return;

            Transform anchor = currentWeapon.transform.Find("Anchor");
            Transform state_ads = currentWeapon.transform.Find("States/ADS");
            Transform state_hip = currentWeapon.transform.Find("States/Hip");

            if (p_isAiming)
            {
                anchor.position = Vector3.Lerp(anchor.position, state_ads.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
            }
            else
            {
                anchor.position = Vector3.Lerp(anchor.position, state_hip.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
            }
        }
    }
}

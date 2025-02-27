﻿using System.Collections;
using UnityEngine;

///summary
 /*
Developers and Contributors: Ian Cahoon

Information
    Name: Snipe
    Type: Active
    Effect: Basic fire turns into hitscan lightning bolts with increase damage but decreased fire rate
    Tier: Rare
  */
///summary

namespace Powerups
{
    public class Active_Snipe : ActiveAbility
    {
        //private float CDstart;
        private bool CurrentlyActive = false;
        private PlayerShoot pShoot;
        float Damage = 40;
        ModelManager manager;
        Vector3 worldPoint;
        GameObject visual;
        CameraController camC;
        GameObject r;

        private void Awake()
        {
            // Set name
            Cooldown = 5f;
            Name = "Snipe";
            //Set Sprite
            Icon = Resources.Load<Sprite>("Images/Sniper");
            Tier = PowerupTier.Rare;
        }
        public override void OnAbilityAdd()
        {
            //Powerup added
            Debug.Log(Name + " Added");
            manager = GetComponent<ModelManager>();
            visual = Resources.Load<GameObject>("SnipeOrigin");
            camC = GetComponent<CameraController>();
            base.OnAbilityAdd();
        }

        public override void OnAbilityRemove()
        {
            PhotonNetwork.Destroy(r);
            base.OnAbilityRemove();
        }
        protected override void Activate()
        {
            if (!photonView.isMine)
            {
                return;
            }
            Debug.Log("Snipe Activated");
            StartCoroutine(SnipeRay());
            base.Activate();
        }
        IEnumerator SnipeRay()
        {
            GameObject sp = transform.Find("ShootPoint").gameObject;
            
            Ray aim = camC.cam.ScreenPointToRay(Input.mousePosition);
            //aim.origin = sp.transform.position;
            RaycastHit rHit;
            Vector3 dest = aim.direction;
            GameObject hitObject = null;
            if (Physics.Raycast(sp.transform.position, aim.direction, out rHit))
            {                
                Debug.Log("cast: " + rHit.transform.gameObject.name);
                dest = rHit.point;
                hitObject = rHit.transform.gameObject;
            }
            r = PhotonNetwork.Instantiate("SnipeOrigin", sp.transform.position, Quaternion.identity, 0);
            //GameObject r = Instantiate(visual, sp.transform.position, Quaternion.identity);
            Transform laser = r.transform.GetChild(0);
            
            float dist = Vector3.Distance(rHit.point, sp.transform.position);
            Ray visualRay = new Ray(sp.transform.position, rHit.point);
            
            laser.localScale = new Vector3(.1f, dist/2, .1f);
            laser.localPosition = new Vector3(-.2f, 0, (dist / 2) - 1);
            r.transform.LookAt(dest);
            if (hitObject != null)
            {
                if (hitObject.tag == "Player")
                {
                    ApplyDamage(rHit.transform.gameObject);
                }
                yield return new WaitForSecondsRealtime(.2f);
                PhotonNetwork.Destroy(r);
            }
            else
            {
                yield return new WaitForSecondsRealtime(.2f);
                PhotonNetwork.Destroy(r);
            }
        }
        void ApplyDamage(GameObject target)
        {
            PhotonView view = target.GetPhotonView();
            PlayerStats stats = target.GetComponent<PlayerStats>();
            if (PhotonNetwork.isMasterClient)
            {
                stats.TakeDamage(Damage);
            }
        }
    }
}
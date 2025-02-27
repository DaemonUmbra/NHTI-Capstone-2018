﻿using UnityEngine;

namespace Powerups
{
    public class Passive_Confuse : PassiveAbility
    {
        PlayerController movement;
        private AbilityManager AbilityManager;
        private float timeLimit = 15.0f;
        private float timer;

        private void Awake()
        {
            Name = "Confuse";
            Icon = Resources.Load<Sprite>("Images/Confuse");
            Tier = PowerupTier.Rare;
        }

        public override void OnAbilityAdd()
        {
            if (photonView.isMine)
            {
                movement = GetComponent<PlayerController>();
                movement.InvertX = true;
                movement.InvertY = true;
                AbilityManager = GetComponent<AbilityManager>();
            }
            base.OnAbilityAdd();
        }

        public override void OnAbilityRemove()
        {
            if (photonView.isMine)
            {
                movement = GetComponent<PlayerController>();
                movement.InvertX = false;
                movement.InvertY = false;
            }
            
        }

        private void Update()
        {
            if (active)
            {
                timer += Time.deltaTime;
                if (timer >= timeLimit)
                {
                    AbilityManager.RemoveAbility(this);
                }
            }
        }
    }
}
using System.Collections;
using UnityEngine;

namespace LivingObjectAddons
{
    public class MoveShootAndDie : CrusierSpeed, IUpdate
    {
        [Header("Configuration")]
        [Tooltip("Distance from screen to stop in percent (from 0 to 1).")]
        [Range(0, 1)]
        public float limit;
        [Tooltip("Whenever it should move to reach Limit or should start moving after reach Limit.")]
        public bool moveToLimit;
        [Tooltip("Time after been triggered to shoot and die.")]
        public float time;
        private float warmup;
        [Tooltip("Weapon to shoot.")]
        public Weapon weapon;
        [Tooltip("Amount of shoots before die.")]
        public float shoots;
        private float currentShoots;

        private bool hasBeenTriggered = false;
        private LivingObject livingObject;

        public override void Build(LivingObject livingObject)
        {
            this.livingObject = livingObject;
            base.Build(livingObject);
        }

        // TODO?: https://stackoverflow.com/questions/37310896/overriding-explicit-interface-implementations?rq=1
        public override void Move(float speedMultiplier)
        {
            if (moveToLimit)
            {
                if (1 - Boundary.GetYPercent(thisRigidbody.position.y) < limit)
                {
                    hasBeenTriggered = true;
                    base.Move(speedMultiplier);
                }
            }
            else
            {
                if (1 - Boundary.GetYPercent(thisRigidbody.position.y) > limit)
                {
                    hasBeenTriggered = true;
                    base.Move(speedMultiplier);
                }
            }
        }

        void IUpdate.Update()
        {
            if (hasBeenTriggered)
            {
                if ((warmup += Time.deltaTime) > time)
                {
                    if (currentShoots == shoots)
                    {
                        livingObject.Die();
                        currentShoots++; // To avoid loop
                    }
                    else if (currentShoots < shoots)
                        if (weapon.TryShoot())
                            currentShoots++;
                }
            }
        }
    }
}
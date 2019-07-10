﻿using UnityEngine;

namespace LivingObjectAddons
{
    public interface IStart
    {
        void OnStart(LivingObject livingObject);
    }
    public abstract class OnInitialize : MonoBehaviour, IStart
    {
        public virtual void Initialize() { }
        public abstract void OnStart(LivingObject livingObject);
    }
}

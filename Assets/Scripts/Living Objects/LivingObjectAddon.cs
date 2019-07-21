using UnityEngine;

namespace LivingObjectAddons
{
    public interface IBuild
    {
        void OnBuild(LivingObject livingObject);
    }
    public abstract class OnInitialize : MonoBehaviour, IBuild
    {
        public virtual void Initialize() { }
        public abstract void OnBuild(LivingObject livingObject);
    }
    public abstract class OnDeath : MonoBehaviour, IBuild
    {
        public virtual void Die() { }
        public abstract void OnBuild(LivingObject livingObject);
    }
    public abstract class Movement : OnInitialize
    {
        public virtual void Move(float speedMultiplier) { }
    }
}

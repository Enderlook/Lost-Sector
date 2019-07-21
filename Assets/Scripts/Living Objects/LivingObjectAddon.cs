namespace LivingObjectAddons
{
    public interface IBuild
    {
        void Build(LivingObject livingObject);
    }
    public interface IInitialize
    {
        void Initialize();
    }
    public interface IDie
    {
        void Die();
    }
    public interface IMove
    {
        void Move(float speedMultiplier);
    }
}

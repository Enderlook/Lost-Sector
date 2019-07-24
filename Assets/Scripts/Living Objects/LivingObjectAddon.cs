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
        /// <summary>
        /// Executed on death.
        /// </summary>
        /// <param name="suicide"><see langword="true"/> if it was a suicide. <see langword="false"/> if it was murderer.</param>
        void Die(bool suicide);
    }
    public interface IMove
    {
        void Move(float speedMultiplier);
    }
    public interface IUpdate
    {
        void Update();
    }
}

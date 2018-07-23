namespace GrimDank.MObjects.Components
{
    abstract class AIBase : Component
    {
        public AIBase(MObject parent)
            : base(parent)
        {

        }

        public abstract void TakeTurn();
    }
}

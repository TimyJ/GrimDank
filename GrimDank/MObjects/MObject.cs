using GoRogue;

namespace GrimDank.MObjects
{
    class MObject : IHasID
    {
        // This will pretty much be unsat later as far as serialization goes but easy enough to sort out then
        private static readonly IDGenerator _idGen = new IDGenerator();

        public uint ID { get; private set; }

        // Can add parameters just here for ID stuff.
        public MObject()
        {
            ID = _idGen.UseID();
        }

    }
}

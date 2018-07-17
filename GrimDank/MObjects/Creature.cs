using GoRogue;
using GoRogue.DiceNotation;

namespace GrimDank.MObjects
{
    class Creature : MObject
    {
        //This is Energy. If you don't know what that is you are probably an SO of a dev and should find something better to do with your time than read this
        public int CurrentEnergy { get; private set; }
        public int MaxEnergy { get; private set; }

        //This controlls the chance of hit. will get more complicated as time goes. but this is your to hit
        public int AttackRating { get; private set; }

        //The roll you make upon a hit to wreck face
        public string WeaponDice { get; private set; }

        //keeps you kicking ass and taking names
        public int DamageMitigation { get; private set; }

        public Creature(Coord pos, int baseEnergy, int attackRating, string weaponDice, int av) : base(Map.Layer.CREATURES, pos)
        {
            MaxEnergy = baseEnergy;
            CurrentEnergy = baseEnergy;
            AttackRating = attackRating;
            WeaponDice = WeaponDice;
            DamageMitigation = av;
        }

        public void TakeDamage(int amount)
        {
            CurrentEnergy -= amount;
            if (CurrentEnergy <= 0)
            {
                GrimDank.Instance.TestLevel.Remove(this);
            }
        }
    }
}

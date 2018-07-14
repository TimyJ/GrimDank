using GoRogue;
using GoRogue.DiceNotation;

namespace GrimDank.MObjects
{
    class Creature : MObject
    {
        //This is HP. If you don't know what that is you are an SO of a dev and should find something better to do with your time than read this
        public int CurrentHP { get; }
        public int MaxHP { get; }

        //This controlls the chance of hit. will get more complicated as time goes. but this is your to hit
        public int AttackRating { get; }

        //The roll you make upon a hit to wreck face
        public string WeaponDice { get; }

        //keeps you kicking ass and taking names
        public int ArmorValue { get; }
        public Creature(Coord pos, int baseHP, int attackRating, string weaponDice, int av) : base(Map.Layer.CREATURES, pos)
        {
            MaxHP = baseHP;
            CurrentHP = baseHP;
            AttackRating = attackRating;
            WeaponDice = WeaponDice;
            ArmorValue = av;
        }
    }
}

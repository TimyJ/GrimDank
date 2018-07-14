﻿using GoRogue;
using GoRogue.DiceNotation;

namespace GrimDank.MObjects
{
    class Creature : MObject
    {
        //This is HP. If you don't know what that is you are probably an SO of a dev and should find something better to do with your time than read this
        public int CurrentHP { get; private set; }
        public int MaxHP { get; private set; }

        //This controlls the chance of hit. will get more complicated as time goes. but this is your to hit
        public int AttackRating { get; private set; }

        //The roll you make upon a hit to wreck face
        public string WeaponDice { get; private set; }

        //keeps you kicking ass and taking names
        public int ArmorValue { get; private set; }

        public Creature(Coord pos, int baseHP, int attackRating, string weaponDice, int av) : base(Map.Layer.CREATURES, pos)
        {
            MaxHP = baseHP;
            CurrentHP = baseHP;
            AttackRating = attackRating;
            WeaponDice = WeaponDice;
            ArmorValue = av;
        }

        public void TakeDamage(int amount)
        {
            CurrentHP -= amount;
        }
    }
}
using Patterns;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Game.Characters.CharacterFactory;


namespace Game.Characters
{
    public class CharacterFactory : Factory<GameObject, CharacterEnum>
    {
        public enum CharacterEnum
        {
            CHICKEN = 0,
            KITTY = 1,
            DOG = 2,
            PENGUIN = 3,
            DEER = 4,
            HORSE = 5,
            TIGER = 6,
        }

        [Serializable]
        public struct CharacterDataEntry
        {
            public CharacterEnum characterType;
            public GameObject prefab;
        }

        public List<CharacterDataEntry> charactersInFactory;


        protected override void Awake()
        {
            base.Awake();
            foreach (CharacterDataEntry entry in charactersInFactory)
            {
                FactoryRegister(entry.characterType, () => Instantiate(entry.prefab));
            }
        }


        private void OnDestroy()
        {
            foreach (CharacterDataEntry entry in charactersInFactory)
            {
                FactoryUnregister(entry.characterType);
            }
        }
       

    }
}


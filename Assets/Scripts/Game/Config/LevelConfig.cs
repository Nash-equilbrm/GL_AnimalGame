using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using static Game.Characters.CharacterFactory;

namespace Game.Level
{
    [Serializable]
    public struct LevelConfig
    {
        public int id;
        public int noiseScale;
        public int player;
        public int[] mapSize;
        public int[] enemies;

        public CharacterEnum PlayerEnum => (CharacterEnum)player;
        public CharacterEnum[] EnemiesEnum => enemies.Select(x => (CharacterEnum)x).ToArray();

        public int TotalEnemies => enemies.Length;
    }

    [Serializable]
    public class LevelConfigList
    {
        public List<LevelConfig> levels;
    }
}

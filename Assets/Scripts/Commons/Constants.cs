using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Config
{
    public static class Constants
    {
        public static int MAX_HEALTH = 3;
        public static int MAX_ROWS = 2;
        public static int MAX_COLUMNS = 2;
        public static int CELL_SIZE = 1;

        public static string BLOCK_1_POOL_TAG = "Block1";
        public static string BLOCK_2_POOL_TAG = "Block2";
        public static string BLOCK_TILE_TAG = "block_tile";
        public static string MAP_TILE_TAG = "map_tile";
        public static string ENEMY_TAG = "enemy";
        public static string END_CHECKPOINT_TAG = "end";
        public static string START_CHECKPOINT_TAG = "start";
        public static string DECORATE_POOL_TAG = "Decorate";


        public static float INTERVAL_BETWEEN_LEVELS = 3f;

        public static float MAX_SPEED_ANIMATE = 1f;
        public static float MAX_SPEED = 2f;

        public static float CAMERA_SIZE_REF_FOR_MAP_10X10 = .65f;


        #region VFX
        #endregion

        #region AUDIO
        public static string AUDIO_TAKE_DAMAGE = "TakeDamage";
        public static string AUDIO_HIT_BLOCK_1 = "Hit Block 1";
        public static string AUDIO_HIT_BLOCK_2 = "Hit Block 2";
        public static string AUDIO_HIT_ENEMY = "Hit Enemy";
        public static string AUDIO_WIN = "Lose sound";
        public static string AUDIO_LOSE = "Win sound";
        public static string AUDIO_CHECKPOINT = "Checkpoint";
        public static string AUDIO_BTN_CLICK = "BtnClicked";


        public static string AUDIO_BGM_MAIN_MENU = "mainmenu";
        public static string AUDIO_BGM_GAMEPLAY = "gameplay";
        #endregion
    }

}

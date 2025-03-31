using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Patterns
{
    public enum EventID
    {
        OnInitLevel,
        OnSetBlockToSlot,
        OnFinishLevel,
        OnCleanupLevel,
        OnStartGameplay,
        OnReplayBtnClicked,
        OnFinishPlacingSpawn,
        OnFinishInitRedDot,
        OnFinishSpawnPlayer,
        OnReachEndCheckPoint,
        OnTakeDamage,
        OnFinishCleanupMap,
        OnFinishGame,
        OnResetGameplay,
        OnFinishInitAreaForEnemies,
        OnFinishSpawnEnemies,
        OnPlayBtnClicked,
        OnBackToMenuClicked,
        OnContinueBtnClicked,
        OnPlayerResetGameplay
    }
}

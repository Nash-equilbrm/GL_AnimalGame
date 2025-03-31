using DG.Tweening;
using Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Map
{
    public class MapPlane : MonoBehaviour
    {
        private void OnEnable()
        {
            this.PubSubRegister(EventID.OnInitLevel, OnInitLevel);
        }

        private void OnDisable()
        {
            this.PubSubUnregister(EventID.OnInitLevel, OnInitLevel);
        }

        private void OnInitLevel(object obj)
        {
            var mapSize = GameManager.Instance.CurrentConfig.mapSize;
            var targetPosition = new Vector3(mapSize[0]/2f - .5f, 0f, mapSize[1]/2f - .5f);
            var targetScale = new Vector3(mapSize[0], 1f, mapSize[1]) * 0.1f;
            transform.DOMove(targetPosition, 1f).SetEase(Ease.OutCubic);
            transform.DOScale(targetScale, 1f).SetEase(Ease.OutCubic);
        }
    }
}


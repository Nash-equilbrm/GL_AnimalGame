using System;
using Game.Level;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Commons
{
    public static class Common
    {
        private static readonly System.Random _random = new();

        public static T GetRandomItem<T>(this IEnumerable<T> collection)
        {
            if (collection == null || !collection.Any())
                throw new InvalidOperationException("Collection is empty or null.");

            return collection.ElementAt(_random.Next(collection.Count()));
        }

        

        public static TKey GetKeyFromValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue value)
        {
            return dict.FirstOrDefault(x => EqualityComparer<TValue>.Default.Equals(x.Value, value)).Key;
        }



        public static Vector2 GetScreenPosition(RectTransform uiElement, Camera uiCamera)
        {
            if (uiElement == null || uiCamera == null) return Vector2.zero;

            Vector3 worldPosition = uiElement.position;
            Vector2 screenPosition = uiCamera.WorldToScreenPoint(worldPosition);

            return screenPosition;
        }

        public static Vector2 GetScreenPosition3D(Transform target, Camera mainCamera)
        {
            if (target == null || mainCamera == null) return Vector2.zero;

            Vector3 screenPoint = mainCamera.WorldToScreenPoint(target.position);

            if (screenPoint.z < 0)
            {
                return Vector2.negativeInfinity;
            }

            return new Vector2(screenPoint.x, screenPoint.y);
        }

        public static Vector2 GetScreenPosition3D(Vector3 target, Camera mainCamera)
        {
            if (target == null || mainCamera == null) return Vector2.zero;

            Vector3 screenPoint = mainCamera.WorldToScreenPoint(target);

            if (screenPoint.z < 0)
            {
                return Vector2.negativeInfinity;
            }

            return new Vector2(screenPoint.x, screenPoint.y);
        }


        public static T GetRandomEnumValue<T>() where T : Enum
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        }

        public static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int randIndex = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[randIndex]) = (list[randIndex], list[i]); // Swap
            }
        }

        public static bool IsPointInQuadrilateral(Vector2 A, Vector2 B, Vector2 C, Vector2 D, Vector2 P)
        {
            float QuadArea = TriangleArea(A, B, C) + TriangleArea(A, C, D);
            float AreaSum = TriangleArea(P, A, B) + TriangleArea(P, B, C) + TriangleArea(P, C, D) + TriangleArea(P, D, A);
            return Mathf.Abs(QuadArea - AreaSum) < 0.001f;
        }

        public static bool IsPointInTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
        {
            float AreaABC = TriangleArea(A, B, C);
            float AreaSum = TriangleArea(P, A, B) + TriangleArea(P, B, C) + TriangleArea(P, C, A);
            return Mathf.Abs(AreaABC - AreaSum) < 0.001f;
        }

        public static bool IsPointOnLine(Vector2 A, Vector2 B, Vector2 P)
        {
            float crossProduct = (P.y - A.y) * (B.x - A.x) - (P.x - A.x) * (B.y - A.y);
            return Mathf.Abs(crossProduct) < 0.001f;
        }

        private static float TriangleArea(Vector2 A, Vector2 B, Vector2 C)
        {
            return Mathf.Abs((A.x * (B.y - C.y) + B.x * (C.y - A.y) + C.x * (A.y - B.y)) / 2.0f);
        }


        public static Vector2 ConvertToVector2(Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }


        public static Vector3 ConvertToVector3(Vector2 v)
        {
            return new Vector3(v.x, 0f, v.y);
        }

        public static string FormatTime(TimeSpan timeSpan)
        {
            return string.Format("{0:00} : {1:00}", timeSpan.Minutes, timeSpan.Seconds);
        }
    }

}

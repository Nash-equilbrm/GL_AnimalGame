
using System;
using UnityEngine;


namespace Commons
{
    public class LogUtility
    {
        public static void Info(string methodName = "unknown", string message = "", string methodColor = null, string messageColor = null)
        {
            var info1 = methodName;
            var info2 = message;
            if (methodColor != null)
            {
                info1 = $"<color={methodColor}>{methodName}</color>";
            }
            if (messageColor != null)
            {
                info2 = $"<color={messageColor}>{message}</color>";
            }
            Debug.Log($">> [{info1}] {info2}");
        }
        public static void ConditionalInfo(string methodName = "unknown", string message = "", bool isTrue = false)
        {
            Info(methodName, $"({isTrue}) {message}", methodColor: isTrue ? "green" : "red");
        }
        public static void NotificationInfo(string methodName = "unknown", string message = "")
        {
            Info(methodName, message, methodColor: "yellow");
        }
        public static void ValidInfo(string methodName = "unknown", string message = "")
        {
            Info(methodName, message, methodColor: "green");
        }
        public static void InvalidInfo(string methodName = "unknown", string message = "")
        {
            Info(methodName, message, methodColor: "red");
        }

        public static void Warning(string methodName = "unknown", string message = "")
        {
            Debug.LogWarning($">> [{methodName}] {message}");
        }

        public static void Error(string methodName = "unknown", string message = "")
        {
            Debug.LogError($">> [{methodName}] {message}");
        }

        public static void Exception(Exception e)
        {
            Debug.LogException(e);
        }
    }
}
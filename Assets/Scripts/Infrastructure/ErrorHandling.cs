using System;
using Assets.Scripts.Services;
using UnityEngine;
using Zenject;
using ZLogger;

namespace Assets.Scripts.Infrastructure
{
    public class ErrorHandling : MonoBehaviour
    {
        private LogService logService;

        [Inject]
        public void Init(LogService logService)
        {
            this.logService = logService;
        }

        void Awake()
        {
            Application.logMessageReceived += HandleException;
        }

        void HandleException(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Exception || type == LogType.Error)
            {
                logService.Loggger.ZLogError(logString + Environment.NewLine + stackTrace);
            }

            if (type == LogType.Warning)
            {
                logService.Loggger.ZLogWarning(logString + Environment.NewLine + stackTrace);
            }
        }
    }
}

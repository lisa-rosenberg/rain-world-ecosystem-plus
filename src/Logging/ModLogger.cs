using BepInEx.Logging;
using System;

namespace EcosystemPlus.src.Logging
{
    /// <summary>
    /// Provides standardized logging helpers for EcosystemPlus mod classes.
    /// </summary>
    public static class ModLogger
    {
        /// <summary>
        /// Logs an info message with a standardized prefix including timestamp, mod name, class name, and optional creature ID.
        /// </summary>
        public static void Info(ManualLogSource logger, string modName, string className, string message, string creatureId = null)
        {
            logger.LogInfo(Prefix(modName, className, creatureId) + message);
        }

        /// <summary>
        /// Logs a debug message with a standardized prefix including timestamp, mod name, class name, and optional creature ID.
        /// </summary>
        public static void Debug(ManualLogSource logger, string modName, string className, string message, string creatureId = null)
        {
            logger.LogDebug(Prefix(modName, className, creatureId) + message);
        }

        /// <summary>
        /// Logs a warning message with a standardized prefix including timestamp, mod name, class name, and optional creature ID.
        /// </summary>
        public static void Warning(ManualLogSource logger, string modName, string className, string message, string creatureId = null)
        {
            logger.LogWarning(Prefix(modName, className, creatureId) + message);
        }

        /// <summary>
        /// Logs an error message with a standardized prefix including timestamp, mod name, class name, and optional creature ID.
        /// </summary>
        public static void Error(ManualLogSource logger, string modName, string className, string message, string creatureId = null)
        {
            logger.LogError(Prefix(modName, className, creatureId) + message);
        }

        /// <summary>
        /// Constructs the standardized log prefix.
        /// </summary>
        private static string Prefix(string modName, string className, string creatureId = null)
        {
            string prefix = $"{DateTime.Now} - {modName} | {className}";
            if (!string.IsNullOrEmpty(creatureId))
                prefix += $" - [{creatureId}]";
            return prefix + ": ";
        }
    }
}

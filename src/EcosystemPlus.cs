using BepInEx;
using EcosystemPlus.src.Logging;

namespace EcosystemPlus.src
{
    [BepInPlugin("de.lisa-rosenberg.ecosystemplus", "EcosystemPlus", "1.0")]

    public class EcosystemPlus : BaseUnityPlugin
    {

        /// <summary>
        /// Mod name for logging purposes.
        /// </summary>
        public static readonly string ModName = typeof(EcosystemPlus).Namespace?.Split('.')[0];

        /// <summary>
        /// Class name for logging purposes, used to identify the source of log messages.
        /// </summary>
        private static readonly string ClassName = typeof(EcosystemPlus).Name;

        /// <summary>
        /// Mod version, extracted from the assembly information.
        /// </summary>
        private static readonly string ModVersion = typeof(EcosystemPlus).Assembly.GetName().Version.ToString();

        public void OnEnable()
        {
            ModLogger.Info(Logger, ModName, ClassName, $"{ClassName} {ModVersion} is loaded.");

            Behaviors.NeedleWormBehavior.Apply();
        }
    }
}

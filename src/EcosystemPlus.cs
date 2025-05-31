using BepInEx;
using BepInEx.Logging;

namespace EcosystemPlus.src
{
    [BepInPlugin("de.lisa-rosenberg.ecosystemplus", "EcosystemPlus", "1.0")]

    public class EcosystemPlus : BaseUnityPlugin
    {

        public void OnEnable()
        {
            Logger.LogInfo(System.DateTime.Now + " - EcosystemPlus: EcosystemPlus 1.0 is loaded.");

            Behaviors.NeedleWormBehavior.Apply();
        }
    }
}

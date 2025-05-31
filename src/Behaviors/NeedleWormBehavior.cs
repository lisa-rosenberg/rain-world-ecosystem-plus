using BepInEx.Logging;
using RWCustom;
using UnityEngine;
using EcosystemPlus.src.Logging;
using EcosystemPlus.src;

namespace EcosystemPlus.src.Behaviors
{

    /// <summary>
    /// Contains extended AI behavior logic for the <c>NeedleWormAI</c> creature, such as grazing and food-seeking functionality.
    /// </summary>
    public static class NeedleWormBehavior
    {

        /// <summary>
        /// Mod name and class name for logging purposes.
        /// </summary>
        private static readonly string ModName = EcosystemPlus.ModName;

        /// <summary>
        /// Class name for logging purposes, used to identify the source of log messages.
        /// </summary>
        private static readonly string ClassName = typeof(NeedleWormBehavior).Name;

        /// <summary>
        /// Logger for the NeedleWormBehavior class.
        /// </summary>
        private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("NeedleWormBehavior");

        /// <summary>
        /// Probability threshold for grazing behavior. This value determines how often the NeedleWorm will attempt to graze on the ground.
        /// The value 0.0015 means there's a 0.15% chance per update that the NeedleWorm will attempt to graze.
        /// </summary>
        private const double GrazingProbability = 0.0015;

        /// <summary>
        /// Horizontal range for searching solid tiles. This value defines how far left and right the NeedleWorm will search for solid tiles to graze on.
        /// </summary>
        private const int HorizontalRange = 5;

        /// <summary>
        /// Tile coordinate node value used to distinguish between normal tile coordinates and abstract nodes.
        /// abstractNode == -1: The coordinate refers to a specific tile (normal case for movement within a room).
        /// abstractNode >= 0: The coordinate refers to a special node, such as a shortcut entrance/exit, den, or other abstract navigation point defined in the room's data.
        /// </summary>
        private const int TileCoordinateNode = -1;

        /// <summary>
        /// Minimum value (in frames) for the grazing counter, which determines how long the NeedleWorm will graze before resuming normal behavior.
        /// In second, this is approximately 0.83 seconds (50 frames at 60 FPS).
        /// </summary>
        private const int MinGrazingFramesValue = 50;

        /// <summary>
        /// Maximum value (in frames) for the grazing counter, which determines how long the NeedleWorm will graze before resuming normal behavior.
        /// In second, this is approximately 3.33 seconds (200 frames at 60 FPS).
        /// </summary>
        private const int MaxGrazingFramesValue = 200;

        /// <summary>
        /// Random number generator used for behavior variation. Shared instance to avoid repeated seeding and ensure consistency.
        /// </summary>
        private static readonly System.Random rng = new System.Random();

        /// <summary>
        /// Per-creature grazing state storage. This dictionary maps each <see cref="NeedleWormAI"/> instance to its grazing state, which includes a counter and target coordinates.
        /// </summary>
        private static readonly System.Collections.Generic.Dictionary<NeedleWormAI, GrazingState> GrazingStates = new System.Collections.Generic.Dictionary<NeedleWormAI, GrazingState>();

        /// <summary>
        /// Represents the grazing state of a NeedleWorm, including the remaining grazing counter and the target position for grazing.
        /// </summary>
        class GrazingState
        {
            /// <summary>
            /// Counter for how many frames the NeedleWorm will continue grazing.
            /// </summary>
            public int counter;

            /// <summary>
            /// The target position where the NeedleWorm will graze or is currently grazing.
            /// </summary>
            public WorldCoordinate target;
        }

        /// <summary>
        /// Registers all necessary hooks for <see cref="NeedleWormAI"/> to enable extended AI behaviors, such as grazing or dynamic food search.
        /// </summary>
        public static void Apply()
        {
            ModLogger.Info(Logger, ModName, ClassName, "Applying NeedleWormAI hooks.");

            On.NeedleWormAI.Update += NeedleWormAI_Update;
        }

        /// <summary>
        /// Hook into <c>NeedleWormAI.Update</c> to inject additional behavior, such as grazing logic, while preserving the original update functionality.
        /// </summary>
        /// <param name="orig">
        /// The original update delegate. This is called to retain the base AI behavior.
        /// </param>
        /// <param name="self">
        /// The current instance of <see cref="NeedleWormAI"/> on which the update logic is executed.
        /// </param>
        private static void NeedleWormAI_Update(On.NeedleWormAI.orig_Update orig, NeedleWormAI self)
        {
            // Keep default behavior of NeedleWormAI (and possibly other mods that uses NeedleWormAI)
            orig(self);

            // Chain the new behavior for NeedleWormAI
            ApplyGrazingBehavior(self);
        }

        /// <summary>
        /// Applies grazing behavior to the NeedleWorm.
        /// </summary>
        /// <param name="self">
        /// The NeedleWorm AI instance
        /// </param>
        private static void ApplyGrazingBehavior(NeedleWormAI self)
        {
            string creatureId = self.creature.ToString();

            // If already grazing, continue grazing unless interrupted
            if (GrazingStates.TryGetValue(self, out GrazingState grazing))
            {
                // Interrupt if threatened or attacking, or grazing finished
                if (!IsNeedleWormIdle(self, creatureId) || grazing.counter <= 0)
                {
                    ModLogger.Info(Logger, ModName, ClassName, "Grazing interrupted or finished.", creatureId);
                    GrazingStates.Remove(self);
                    return;
                }

                // Continue grazing: keep destination and decrement counter
                self.creature.abstractAI.SetDestination(grazing.target);
                grazing.counter--;
                ModLogger.Debug(Logger, ModName, ClassName, $"Grazing... {grazing.counter} frames left.", creatureId);
                return;
            }

            // Not currently grazing. Check for grazing conditions
            if (IsNeedleWormIdle(self, creatureId) && RollForGrazing(creatureId))
            {
                ModLogger.Debug(Logger, ModName, ClassName, "Attempting to graze.", creatureId);

                IntVector2 tile = FindSolidTileNearby(self, creatureId);
                if (tile != null)
                {
                    int roomIndex = self.creature.Room.realizedRoom.abstractRoom.index;
                    WorldCoordinate target = new WorldCoordinate(roomIndex, tile.x, tile.y, TileCoordinateNode);

                    ModLogger.Info(Logger, ModName, ClassName, $"Setting grazing destination to ({tile.x}, {tile.y}) in room index {roomIndex}", creatureId);
                    self.creature.abstractAI.SetDestination(target);

                    // Start grazing state
                    GrazingStates[self] = new GrazingState
                    {
                        counter = rng.Next(MinGrazingFramesValue, MaxGrazingFramesValue),
                        target = target
                    };
                }
            }
        }

        /// <summary>
        /// Determines whether the NeedleWorm should attempt to graze based on a random roll.
        /// </summary>
        /// <param name="creatureId">
        /// The ID of the creature, used for logging purposes.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if the random roll is successful and the NeedleWorm should graze; otherwise, <c>false</c>.
        /// </returns>
        private static bool RollForGrazing(string creatureId)
        {
            double grazingRoll = rng.NextDouble();
            ModLogger.Debug(Logger, ModName, ClassName, $"Rolling for grazing behavior with result {grazingRoll} (Threshold: {GrazingProbability}). Grazing {(grazingRoll < GrazingProbability ? "successful" : "failed")}.", creatureId);
            return grazingRoll < GrazingProbability;
        }

        /// <summary>
        /// Finds the first solid tile below or near the NeedleWorm's current position, within a horizontal range.
        /// </summary>
        /// <param name="self">
        /// The NeedleWorm AI instance to search for solid tiles.
        /// </param>
        /// <returns>
        /// The coordinates of the first solid tile found below the NeedleWorm's current position, within the specified horizontal range.
        /// </returns>
        private static IntVector2 FindSolidTileNearby(NeedleWormAI self, string creatureId)
        {
            IntVector2 origin = self.creature.pos.Tile;
            Room room = self.creature.Room.realizedRoom;

            ModLogger.Debug(Logger, ModName, ClassName, $"Searching for solid tile near [{origin.x}, {origin.y}] in room index {room.abstractRoom.index}.", creatureId);

            // Pick a random horizontal offset in [-HorizontalRange, HorizontalRange]
            int offset = rng.Next(-HorizontalRange, HorizontalRange + 1);
            int x = Mathf.Clamp(origin.x + offset, 0, room.TileWidth - 1);

            // Start from the creature's current y
            int y = origin.y;

            // Search downward for the first solid tile
            while (y > 0 && !room.GetTile(new IntVector2(x, y)).Solid)
            {
                y--;
            }

            ModLogger.Info(Logger, ModName, ClassName, $"Found solid tile at ({x}, {y}) for grazing.", creatureId);

            return new IntVector2(x, y);
        }

        /// <summary>
        /// Checks if the NeedleWorm is idle and not currently threatened, attacking, or in the den.
        /// </summary>
        /// <param name="self">
        /// The NeedleWorm AI instance to check for idle state.
        /// </param>
        /// <param name="creatureId">
        /// The ID of the creature, used for logging purposes.
        /// </param>
        /// <returns>
        /// Returns <c>true</c> if the NeedleWorm is idle; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsNeedleWormIdle(NeedleWormAI self, string creatureId)
        {
            bool isThreatened = self.threatTracker?.mostThreateningCreature != null;
            bool isAttacking = self.preyTracker?.MostAttractivePrey != null;
            bool isInDen = self.creature?.InDen == true;

            if (isInDen || isThreatened || isAttacking)
            {
                ModLogger.Debug(Logger, ModName, ClassName, $"{creatureId} is not idle. In Den: {isInDen}, Threatened: {isThreatened}, Attacking: {isAttacking}", creatureId);
                return false;
            }

            ModLogger.Debug(Logger, ModName, ClassName, $"{creatureId} is idle.", creatureId);
            return true;
        }
    }
}

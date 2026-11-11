using UnityEngine;

namespace DamGame.Runtime
{
    /// <summary>Temporary readable HUD for the first playable slice.</summary>
    public sealed class DamGameHud : MonoBehaviour
    {
        private DamGameController game;

        private void Start()
        {
            game = GetComponent<DamGameController>();
        }

        private void OnGUI()
        {
            if (game == null)
            {
                return;
            }

            var width = Screen.width;
            GUI.Box(new Rect(20f, 18f, 310f, 164f), "SAVE THE VILLAGE");
            GUI.Label(new Rect(40f, 52f, 260f, 24f), $"Coins: {game.Coins:N0}   (+{game.TapStrength} per tap)");
            GUI.Label(new Rect(40f, 80f, 260f, 24f), $"Weather: {game.Weather}");
            GUI.Label(new Rect(40f, 108f, 260f, 24f), $"Pumps: {game.PumpCount}/4   Drain: {game.TotalDrainRate:0.00}/s");
            GUI.Label(new Rect(40f, 136f, 260f, 24f), $"Water: {game.WaterLevel:0}%   Village: {game.VillageHealth:0}%");

            GUI.Box(new Rect(width - 280f, 18f, 260f, 158f), "UPGRADES");
            var pumpLabel = game.AllPumpsInstalled
                ? $"Upgrade pumps  ({game.PumpUpgradeCost:N0})"
                : $"Install pump  ({game.NextPumpCost:N0})";

            if (GUI.Button(new Rect(width - 260f, 56f, 220f, 34f), pumpLabel))
            {
                game.TryBuyPump();
            }

            if (GUI.Button(new Rect(width - 260f, 102f, 220f, 34f), $"Improve tools  ({game.TapUpgradeCost:N0})"))
            {
                game.TryUpgradeTap();
            }

            GUI.Box(new Rect((width * 0.5f) - 170f, Screen.height - 78f, 340f, 44f), "Tap anywhere to earn coins");
        }
    }
}

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace DamGame.Runtime
{
    public enum WeatherState
    {
        Clear,
        Drizzle,
        Rain,
        Storm,
        Thunderstorm
    }

    /// <summary>
    /// First playable economy and survival loop. Balance values are intentionally
    /// kept together here while the prototype is tuned.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public sealed class DamGameController : MonoBehaviour
    {
        public event Action StateChanged;

        public int Coins { get; private set; }
        public int TapStrength { get; private set; } = 1;
        public int PumpCount { get; private set; }
        public int PumpLevel { get; private set; }
        public float WaterLevel { get; private set; } = 18f;
        public float VillageHealth { get; private set; } = 100f;
        public WeatherState Weather { get; private set; } = WeatherState.Clear;
        public float WeatherTimeRemaining { get; private set; }

        public int TapUpgradeCost => Mathf.CeilToInt(24f * Mathf.Pow(2.15f, TapStrength - 1));
        public int NextPumpCost => Mathf.CeilToInt(28f * Mathf.Pow(2.8f, PumpCount));
        public int PumpUpgradeCost => Mathf.CeilToInt(40f * Mathf.Pow(1.7f, PumpLevel));
        public bool AllPumpsInstalled => PumpCount >= 4;
        public float TotalDrainRate => PumpCount * (0.18f + (PumpLevel * 0.08f));
        public float RainRate => Weather switch
        {
            WeatherState.Drizzle => 0.28f,
            WeatherState.Rain => 0.7f,
            WeatherState.Storm => 1.65f,
            WeatherState.Thunderstorm => 3.2f,
            _ => 0f
        };

        private float calmTimeRemaining;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateGame()
        {
            if (FindFirstObjectByType<DamGameController>() != null)
            {
                return;
            }

            var game = new GameObject("Dam Game");
            game.AddComponent<DamGameController>();
            game.AddComponent<DamGameWorld>();
            game.AddComponent<DamGameHud>();
        }

        private void Start()
        {
            calmTimeRemaining = 4f;
            StateChanged?.Invoke();
        }

        private void Update()
        {
            if (Pointer.current != null
                && Pointer.current.press.wasPressedThisFrame
                && (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject(Pointer.current.deviceId)))
            {
                Coins += TapStrength;
                StateChanged?.Invoke();
            }

            UpdateWeather(Time.deltaTime);
            WaterLevel = Mathf.Clamp(WaterLevel + ((RainRate - TotalDrainRate) * Time.deltaTime), 0f, 125f);

            if (WaterLevel > 100f)
            {
                VillageHealth = Mathf.Max(0f, VillageHealth - ((WaterLevel - 100f) * 0.18f * Time.deltaTime));
            }

            StateChanged?.Invoke();
        }

        public void TryBuyPump()
        {
            if (AllPumpsInstalled)
            {
                TryUpgradePumps();
                return;
            }

            var cost = NextPumpCost;
            if (Coins < cost)
            {
                return;
            }

            Coins -= cost;
            PumpCount++;
            StateChanged?.Invoke();
        }

        public void TryUpgradePumps()
        {
            var cost = PumpUpgradeCost;
            if (Coins < cost)
            {
                return;
            }

            Coins -= cost;
            PumpLevel++;
            StateChanged?.Invoke();
        }

        public void TryUpgradeTap()
        {
            var cost = TapUpgradeCost;
            if (Coins < cost)
            {
                return;
            }

            Coins -= cost;
            TapStrength++;
            StateChanged?.Invoke();
        }

        private void UpdateWeather(float deltaTime)
        {
            if (Weather == WeatherState.Clear)
            {
                calmTimeRemaining -= deltaTime;
                if (calmTimeRemaining <= 0f)
                {
                    StartWeatherEvent();
                }

                return;
            }

            WeatherTimeRemaining -= deltaTime;
            if (WeatherTimeRemaining <= 0f)
            {
                Weather = WeatherState.Clear;
                calmTimeRemaining = UnityEngine.Random.Range(5f, 12f);
            }
        }

        private void StartWeatherEvent()
        {
            var roll = UnityEngine.Random.value;
            Weather = roll switch
            {
                < 0.36f => WeatherState.Drizzle,
                < 0.7f => WeatherState.Rain,
                < 0.92f => WeatherState.Storm,
                _ => WeatherState.Thunderstorm
            };

            WeatherTimeRemaining = Weather switch
            {
                WeatherState.Drizzle => UnityEngine.Random.Range(8f, 15f),
                WeatherState.Rain => UnityEngine.Random.Range(10f, 18f),
                WeatherState.Storm => UnityEngine.Random.Range(7f, 14f),
                _ => UnityEngine.Random.Range(5f, 10f)
            };
        }
    }
}

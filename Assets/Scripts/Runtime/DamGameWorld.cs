using System.Collections.Generic;
using UnityEngine;

namespace DamGame.Runtime
{
    /// <summary>Builds a readable temporary world from primitives while custom models are in production.</summary>
    public sealed class DamGameWorld : MonoBehaviour
    {
        private readonly List<GameObject> pumps = new();
        private DamGameController game;
        private Transform reservoir;
        private Transform waterfall;
        private Renderer skyRenderer;
        private Material waterMaterial;

        private void Start()
        {
            game = GetComponent<DamGameController>();
            BuildWorld();
            game.StateChanged += Refresh;
            Refresh();
        }

        private void OnDestroy()
        {
            if (game != null)
            {
                game.StateChanged -= Refresh;
            }
        }

        private void BuildWorld()
        {
            ConfigureCameraAndSun();

            var grass = CreateMaterial(new Color(0.29f, 0.57f, 0.27f));
            waterMaterial = CreateMaterial(new Color(0.08f, 0.49f, 0.75f));
            var concrete = CreateMaterial(new Color(0.62f, 0.59f, 0.53f));
            var roof = CreateMaterial(new Color(0.76f, 0.23f, 0.16f));
            var wall = CreateMaterial(new Color(0.96f, 0.82f, 0.53f));
            var pumpMaterial = CreateMaterial(new Color(0.93f, 0.59f, 0.12f));

            CreatePrimitive(PrimitiveType.Plane, "Ground", new Vector3(0f, 0f, 0f), new Vector3(2.2f, 1f, 0.9f), grass);
            reservoir = CreatePrimitive(PrimitiveType.Cube, "Reservoir Water", new Vector3(-5.5f, 0.15f, 0f), new Vector3(7.5f, 0.3f, 5.6f), waterMaterial).transform;
            CreatePrimitive(PrimitiveType.Cube, "Dam", new Vector3(-1.55f, 2.1f, 0f), new Vector3(0.8f, 4.2f, 5.9f), concrete);
            waterfall = CreatePrimitive(PrimitiveType.Cube, "Overflow", new Vector3(-0.98f, 0.5f, 0f), new Vector3(0.18f, 0.1f, 1.4f), waterMaterial).transform;

            BuildVillage(wall, roof);
            BuildPumpPads(pumpMaterial);

            skyRenderer = CreatePrimitive(PrimitiveType.Quad, "Backdrop", new Vector3(0f, 7f, 5.8f), new Vector3(24f, 12f, 1f), CreateMaterial(new Color(0.53f, 0.78f, 0.92f))).GetComponent<Renderer>();
        }

        private void ConfigureCameraAndSun()
        {
            var camera = Camera.main;
            if (camera != null)
            {
                camera.transform.SetPositionAndRotation(new Vector3(0f, 12f, -16f), Quaternion.Euler(31f, 0f, 0f));
                camera.fieldOfView = 52f;
                camera.backgroundColor = new Color(0.53f, 0.78f, 0.92f);
            }

            var sun = FindFirstObjectByType<Light>();
            if (sun != null)
            {
                sun.transform.rotation = Quaternion.Euler(45f, -35f, 0f);
                sun.intensity = 1.25f;
                sun.color = new Color(1f, 0.94f, 0.82f);
            }
        }

        private void BuildVillage(Material wall, Material roof)
        {
            for (var i = 0; i < 5; i++)
            {
                var x = 1.4f + (i * 1.85f);
                var z = i % 2 == 0 ? -1.5f : 1.35f;
                var house = CreatePrimitive(PrimitiveType.Cube, $"Village House {i + 1}", new Vector3(x, 0.85f, z), new Vector3(1.25f, 1.7f, 1.1f), wall);
                var roofObject = CreatePrimitive(PrimitiveType.Cylinder, $"Village Roof {i + 1}", new Vector3(x, 1.85f, z), new Vector3(0.98f, 0.38f, 0.98f), roof);
                roofObject.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                house.transform.SetParent(transform);
                roofObject.transform.SetParent(transform);
            }
        }

        private void BuildPumpPads(Material pumpMaterial)
        {
            for (var i = 0; i < 4; i++)
            {
                var pump = CreatePrimitive(PrimitiveType.Cylinder, $"Pump {i + 1}", new Vector3(-0.1f + (i * 0.42f), 0.43f, -2.5f), new Vector3(0.26f, 0.85f, 0.26f), pumpMaterial);
                pump.SetActive(false);
                pumps.Add(pump);
            }
        }

        private void Refresh()
        {
            var waterHeight = Mathf.Lerp(0.15f, 4.1f, game.WaterLevel / 125f);
            reservoir.localScale = new Vector3(7.5f, waterHeight, 5.6f);
            reservoir.position = new Vector3(-5.5f, waterHeight * 0.5f, 0f);

            var overflow = Mathf.Clamp01((game.WaterLevel - 88f) / 30f);
            waterfall.gameObject.SetActive(overflow > 0f);
            waterfall.localScale = new Vector3(0.18f, 4f * overflow, 1.4f + (overflow * 3f));
            waterfall.position = new Vector3(-0.98f, waterfall.localScale.y * 0.5f, 0f);

            for (var i = 0; i < pumps.Count; i++)
            {
                pumps[i].SetActive(i < game.PumpCount);
                if (i < game.PumpCount)
                {
                    pumps[i].transform.Rotate(0f, 120f * Time.deltaTime, 0f, Space.World);
                }
            }

            var storm = game.Weather is WeatherState.Storm or WeatherState.Thunderstorm;
            var rain = game.Weather is WeatherState.Rain or WeatherState.Storm or WeatherState.Thunderstorm;
            skyRenderer.material.color = storm ? new Color(0.2f, 0.27f, 0.36f) : rain ? new Color(0.38f, 0.54f, 0.64f) : new Color(0.53f, 0.78f, 0.92f);
            waterMaterial.color = game.WaterLevel > 100f ? new Color(0.08f, 0.25f, 0.45f) : new Color(0.08f, 0.49f, 0.75f);
        }

        private GameObject CreatePrimitive(PrimitiveType type, string objectName, Vector3 position, Vector3 scale, Material material)
        {
            var created = GameObject.CreatePrimitive(type);
            created.name = objectName;
            created.transform.SetPositionAndRotation(position, Quaternion.identity);
            created.transform.localScale = scale;
            created.GetComponent<Renderer>().sharedMaterial = material;
            created.transform.SetParent(transform);
            return created;
        }

        private static Material CreateMaterial(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var material = new Material(shader) { color = color };
            material.SetColor("_BaseColor", color);
            return material;
        }
    }
}

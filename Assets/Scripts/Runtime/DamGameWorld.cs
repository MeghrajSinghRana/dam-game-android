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
        private Camera sceneCamera;
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

            var terrain = Resources.Load<GameObject>("Models/Terrain");
            if (terrain != null)
            {
                var ground = Instantiate(terrain, Vector3.zero, terrain.transform.rotation, transform);
                foreach (var part in ground.GetComponentsInChildren<Renderer>()) part.sharedMaterial = grass;
            }
            else
                CreatePrimitive(PrimitiveType.Plane, "Ground", Vector3.zero, new Vector3(2.2f, 1f, 0.9f), grass);
            reservoir = CreatePrimitive(PrimitiveType.Cube, "Reservoir Water", new Vector3(-5.5f, 0.15f, 0f), new Vector3(7.5f, 0.3f, 5.6f), waterMaterial).transform;
            var damPrefab = Resources.Load<GameObject>("Models/Dam");
            if (damPrefab != null)
            {
                var dam = Instantiate(damPrefab, new Vector3(-1.55f, 0f, 0f), damPrefab.transform.rotation, transform);
                dam.name = "Dam";
                foreach (var part in dam.GetComponentsInChildren<Renderer>())
                    part.sharedMaterial = concrete;
            }
            else
            {
                CreatePrimitive(PrimitiveType.Cube, "Dam", new Vector3(-1.55f, 2.1f, 0f), new Vector3(0.8f, 4.2f, 5.9f), concrete);
            }
            waterfall = CreatePrimitive(PrimitiveType.Cube, "Overflow", new Vector3(-0.98f, 0.5f, 0f), new Vector3(0.18f, 0.1f, 1.4f), waterMaterial).transform;

            BuildVillage(wall, roof);
            BuildPumpPads(pumpMaterial);
            BuildNature();

        }

        private void ConfigureCameraAndSun()
        {
            var camera = Camera.main;
            if (camera != null)
            {
                sceneCamera = camera;
                camera.transform.SetPositionAndRotation(new Vector3(0f, 10f, -13f), Quaternion.Euler(35f, 0f, 0f));
                camera.fieldOfView = 46f;
                camera.clearFlags = CameraClearFlags.SolidColor;
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
            var prefab = Resources.Load<GameObject>("Models/VillageHouse");
            var trim = CreateMaterial(new Color(0.32f, 0.20f, 0.14f));
            var window = CreateMaterial(new Color(0.30f, 0.70f, 0.83f));
            for (var i = 0; i < 5; i++)
            {
                var x = 1.4f + (i * 1.85f);
                var z = i % 2 == 0 ? -1.5f : 1.35f;
                if (prefab != null)
                {
                    var model = Instantiate(prefab, new Vector3(x, 0f, z), prefab.transform.rotation, transform);
                    model.name = $"Village House {i + 1}";
                    foreach (var part in model.GetComponentsInChildren<Renderer>())
                    {
                        part.sharedMaterial = part.name.StartsWith("Roof") || part.name.StartsWith("Chimney") ? roof :
                            part.name.StartsWith("Door") || part.name.StartsWith("Foundation") ? trim : part.name.StartsWith("Window") ? window : wall;
                    }
                    continue;
                }
                var house = CreatePrimitive(PrimitiveType.Cube, $"Village House {i + 1}", new Vector3(x, 0.85f, z), new Vector3(1.25f, 1.7f, 1.1f), wall);
                var roofObject = CreatePrimitive(PrimitiveType.Cylinder, $"Village Roof {i + 1}", new Vector3(x, 1.85f, z), new Vector3(0.98f, 0.38f, 0.98f), roof);
                roofObject.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                house.transform.SetParent(transform);
                roofObject.transform.SetParent(transform);
            }
        }

        private void BuildNature()
        {
            var tree = Resources.Load<GameObject>("Models/PineTree");
            var rock = Resources.Load<GameObject>("Models/Rock");
            var leaves = CreateMaterial(new Color(0.16f, 0.46f, 0.29f));
            var bark = CreateMaterial(new Color(0.35f, 0.22f, 0.14f));
            var stone = CreateMaterial(new Color(0.53f, 0.58f, 0.53f));
            var positions = new[] { new Vector3(-9f, 0, 3f), new Vector3(-7.8f, 0, 3.1f),
                new Vector3(-5.9f, 0, 3.2f), new Vector3(0.3f, 0, 3.2f),
                new Vector3(3.3f, 0, 3.1f), new Vector3(6.4f, 0, 3.2f), new Vector3(9f, 0, 2.8f) };
            if (tree != null)
                for (var i = 0; i < positions.Length; i++)
                {
                    var instance = Instantiate(tree, positions[i], Quaternion.Euler(0, i * 47f, 0) * tree.transform.rotation, transform);
                    instance.transform.localScale = tree.transform.localScale * (0.65f + (i % 3) * 0.15f);
                    foreach (var part in instance.GetComponentsInChildren<Renderer>())
                        part.sharedMaterial = part.name.StartsWith("Trunk") ? bark : leaves;
                }
            if (rock != null)
                for (var i = 0; i < 4; i++)
                {
                    var instance = Instantiate(rock, new Vector3(-9f + i * 0.8f, 0, -3.4f), Quaternion.Euler(0, i * 63f, 0) * rock.transform.rotation, transform);
                    instance.transform.localScale = rock.transform.localScale * (0.45f + i * 0.08f);
                    foreach (var part in instance.GetComponentsInChildren<Renderer>()) part.sharedMaterial = stone;
                }
        }

        private void BuildPumpPads(Material pumpMaterial)
        {
            var prefab = Resources.Load<GameObject>("Models/WaterPump");
            var dark = CreateMaterial(new Color(0.18f, 0.27f, 0.29f));
            for (var i = 0; i < 4; i++)
            {
                var position = new Vector3(-0.4f + (i * 0.95f), 0f, -2.5f);
                var pump = prefab != null ? Instantiate(prefab, position, prefab.transform.rotation, transform) :
                    CreatePrimitive(PrimitiveType.Cylinder, $"Pump {i + 1}", position + Vector3.up * 0.43f, new Vector3(0.26f, 0.85f, 0.26f), pumpMaterial);
                pump.name = $"Pump {i + 1}";
                foreach (var part in pump.GetComponentsInChildren<Renderer>())
                    part.sharedMaterial = part.name.StartsWith("Body") || part.name.StartsWith("Cap") ? pumpMaterial : dark;
                pump.SetActive(false);
                pumps.Add(pump);
            }
        }

        private void Refresh()
        {
            var waterHeight = Mathf.Max(0.05f, 4.1f * game.WaterLevel / 100f);
            reservoir.localScale = new Vector3(7.5f, waterHeight, 5.6f);
            reservoir.position = new Vector3(-5.5f, waterHeight * 0.5f, 0f);

            var overflow = Mathf.Clamp01((game.WaterLevel - 100f) / 25f);
            waterfall.gameObject.SetActive(overflow > 0f);
            waterfall.localScale = new Vector3(0.18f, 4f * overflow, 1.4f + (overflow * 3f));
            waterfall.position = new Vector3(-0.98f, waterfall.localScale.y * 0.5f, 0f);

            for (var i = 0; i < pumps.Count; i++)
            {
                pumps[i].SetActive(i < game.PumpCount);
            }

            var storm = game.Weather is WeatherState.Storm or WeatherState.Thunderstorm;
            var rain = game.Weather is WeatherState.Rain or WeatherState.Storm or WeatherState.Thunderstorm;
            if (sceneCamera != null)
                sceneCamera.backgroundColor = storm ? new Color(0.2f, 0.27f, 0.36f) : rain ? new Color(0.38f, 0.54f, 0.64f) : new Color(0.53f, 0.78f, 0.92f);
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

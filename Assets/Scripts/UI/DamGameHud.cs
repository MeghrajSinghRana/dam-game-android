using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace DamGame.Runtime
{
    /// <summary>Landscape mobile HUD built entirely at runtime for rapid iteration.</summary>
    public sealed class DamGameHud : MonoBehaviour
    {
        private readonly Color panelColor = new(0.025f, 0.075f, 0.10f, 0.91f);
        private readonly Color accentColor = new(0.16f, 0.76f, 0.91f, 1f);
        private readonly Color goldColor = new(1f, 0.75f, 0.20f, 1f);
        private readonly Color textColor = new(0.91f, 0.97f, 0.98f, 1f);

        private DamGameController game;
        private Font font;
        private Text coinText;
        private Text weatherText;
        private Text pumpText;
        private Text waterText;
        private Text healthText;
        private Text pumpButtonText;
        private Text toolButtonText;
        private Image waterFill;
        private Image healthFill;
        private Button pumpButton;
        private Button toolButton;

        private void Start()
        {
            game = GetComponent<DamGameController>();
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureEventSystem();
            BuildHud();
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

        private void EnsureEventSystem()
        {
            if (FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            var input = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            input.transform.SetParent(transform);
        }

        private void BuildHud()
        {
            var root = new GameObject("Mobile HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.transform.SetParent(transform);

            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            var rootRect = root.GetComponent<RectTransform>();
            var statusPanel = CreatePanel(rootRect, "Status Panel", new Vector2(26f, -26f), new Vector2(390f, 270f), new Vector2(0f, 1f));
            var upgradePanel = CreatePanel(rootRect, "Upgrade Panel", new Vector2(-26f, -26f), new Vector2(400f, 270f), new Vector2(1f, 1f));

            CreateText(statusPanel, "SAVE THE VILLAGE", new Vector2(24f, -24f), new Vector2(340f, 34f), 28, goldColor, FontStyle.Bold);
            coinText = CreateText(statusPanel, string.Empty, new Vector2(24f, -72f), new Vector2(340f, 34f), 24, textColor, FontStyle.Bold);
            weatherText = CreateText(statusPanel, string.Empty, new Vector2(24f, -112f), new Vector2(340f, 30f), 20, accentColor, FontStyle.Normal);
            pumpText = CreateText(statusPanel, string.Empty, new Vector2(24f, -150f), new Vector2(340f, 30f), 18, textColor, FontStyle.Normal);
            waterText = CreateText(statusPanel, "RESERVOIR", new Vector2(24f, -190f), new Vector2(130f, 24f), 16, textColor, FontStyle.Bold);
            waterFill = CreateMeter(statusPanel, new Vector2(150f, -192f), new Vector2(210f, 20f), new Color(0.08f, 0.55f, 0.9f));
            healthText = CreateText(statusPanel, "VILLAGE", new Vector2(24f, -226f), new Vector2(130f, 24f), 16, textColor, FontStyle.Bold);
            healthFill = CreateMeter(statusPanel, new Vector2(150f, -228f), new Vector2(210f, 20f), new Color(0.25f, 0.88f, 0.47f));

            CreateText(upgradePanel, "BUILD & IMPROVE", new Vector2(24f, -24f), new Vector2(350f, 34f), 28, goldColor, FontStyle.Bold);
            CreateText(upgradePanel, "Keep the reservoir below the dam crest.", new Vector2(24f, -64f), new Vector2(350f, 28f), 16, new Color(0.68f, 0.79f, 0.82f), FontStyle.Normal);
            pumpButton = CreateButton(upgradePanel, new Vector2(24f, -108f), new Vector2(352f, 56f), out pumpButtonText);
            toolButton = CreateButton(upgradePanel, new Vector2(24f, -178f), new Vector2(352f, 56f), out toolButtonText);
            pumpButton.onClick.AddListener(game.TryBuyPump);
            toolButton.onClick.AddListener(game.TryUpgradeTap);

            var tapPrompt = CreatePanel(rootRect, "Tap Prompt", new Vector2(0f, 34f), new Vector2(460f, 58f), new Vector2(0.5f, 0f));
            tapPrompt.GetComponent<Image>().raycastTarget = false;
            var prompt = CreateText(tapPrompt, "TAP THE LANDSCAPE TO EARN COINS", new Vector2(0f, -8f), new Vector2(460f, 40f), 19, textColor, FontStyle.Bold);
            prompt.alignment = TextAnchor.MiddleCenter;
            prompt.raycastTarget = false;
        }

        private void Refresh()
        {
            coinText.text = $"{game.Coins:N0}  COINS     +{game.TapStrength} / TAP";
            weatherText.text = game.Weather == WeatherState.Clear
                ? "CLEAR SKIES"
                : $"{game.Weather.ToString().ToUpperInvariant()}  •  {Mathf.CeilToInt(game.WeatherTimeRemaining)}s";
            pumpText.text = $"PUMPS  {game.PumpCount}/4     DRAIN  {game.TotalDrainRate:0.00}/s";
            waterText.text = $"WATER  {game.WaterLevel:0}%";
            healthText.text = $"HEALTH  {game.VillageHealth:0}%";
            waterFill.fillAmount = game.WaterLevel / 125f;
            healthFill.fillAmount = game.VillageHealth / 100f;

            pumpButtonText.text = game.AllPumpsInstalled
                ? $"UPGRADE PUMPS   {game.PumpUpgradeCost:N0}"
                : $"INSTALL PUMP   {game.NextPumpCost:N0}";
            toolButtonText.text = $"IMPROVE TOOLS   {game.TapUpgradeCost:N0}";
            pumpButton.interactable = game.AllPumpsInstalled ? game.Coins >= game.PumpUpgradeCost : game.Coins >= game.NextPumpCost;
            toolButton.interactable = game.Coins >= game.TapUpgradeCost;
        }

        private RectTransform CreatePanel(RectTransform parent, string objectName, Vector2 position, Vector2 size, Vector2 anchor)
        {
            var panel = CreateUiObject(objectName, parent);
            panel.anchorMin = anchor;
            panel.anchorMax = anchor;
            panel.pivot = anchor;
            panel.anchoredPosition = position;
            panel.sizeDelta = size;
            panel.gameObject.AddComponent<Image>().color = panelColor;
            return panel;
        }

        private Text CreateText(RectTransform parent, string value, Vector2 position, Vector2 size, int fontSize, Color color, FontStyle style)
        {
            var textRect = CreateUiObject("Text", parent);
            textRect.anchorMin = new Vector2(0f, 1f);
            textRect.anchorMax = new Vector2(0f, 1f);
            textRect.pivot = new Vector2(0f, 1f);
            textRect.anchoredPosition = position;
            textRect.sizeDelta = size;

            var text = textRect.gameObject.AddComponent<Text>();
            text.font = font;
            text.text = value;
            text.fontSize = fontSize;
            text.fontStyle = style;
            text.color = color;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private Image CreateMeter(RectTransform parent, Vector2 position, Vector2 size, Color fillColor)
        {
            var track = CreateUiObject("Meter Track", parent);
            track.anchorMin = new Vector2(0f, 1f);
            track.anchorMax = new Vector2(0f, 1f);
            track.pivot = new Vector2(0f, 1f);
            track.anchoredPosition = position;
            track.sizeDelta = size;
            track.gameObject.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.35f);

            var fill = CreateUiObject("Meter Fill", track);
            fill.anchorMin = Vector2.zero;
            fill.anchorMax = Vector2.one;
            fill.offsetMin = Vector2.zero;
            fill.offsetMax = Vector2.zero;
            var image = fill.gameObject.AddComponent<Image>();
            image.color = fillColor;
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillAmount = 1f;
            image.raycastTarget = false;
            return image;
        }

        private Button CreateButton(RectTransform parent, Vector2 position, Vector2 size, out Text label)
        {
            var buttonRect = CreateUiObject("Upgrade Button", parent);
            buttonRect.anchorMin = new Vector2(0f, 1f);
            buttonRect.anchorMax = new Vector2(0f, 1f);
            buttonRect.pivot = new Vector2(0f, 1f);
            buttonRect.anchoredPosition = position;
            buttonRect.sizeDelta = size;

            var image = buttonRect.gameObject.AddComponent<Image>();
            image.color = new Color(0.11f, 0.47f, 0.59f, 1f);
            var button = buttonRect.gameObject.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.15f, 1.15f, 1.15f, 1f);
            colors.pressedColor = new Color(0.78f, 0.78f, 0.78f, 1f);
            colors.disabledColor = new Color(0.4f, 0.4f, 0.4f, 0.75f);
            button.colors = colors;

            label = CreateText(buttonRect, string.Empty, Vector2.zero, size, 20, Color.white, FontStyle.Bold);
            label.alignment = TextAnchor.MiddleCenter;
            label.raycastTarget = false;
            return button;
        }

        private static RectTransform CreateUiObject(string objectName, RectTransform parent)
        {
            var created = new GameObject(objectName, typeof(RectTransform));
            var rect = created.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            return rect;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace AeroRush
{
    [Serializable]
    public sealed class EarnableSkin
    {
        public string Id;
        public string DisplayName;
        public int MatchesRequired;
        public int WinsRequired;
        public Color BodyColor;

        public EarnableSkin(string id, string displayName, int matches, int wins, Color color)
        {
            Id = id;
            DisplayName = displayName;
            MatchesRequired = matches;
            WinsRequired = wins;
            BodyColor = color;
        }
    }

    /// <summary>Cosmetic rewards are earned through gameplay; no paid unlocks.</summary>
    public sealed class SkinProgression : MonoBehaviour
    {
        private const string MatchesKey = "AeroRush.Matches";
        private const string WinsKey = "AeroRush.Wins";
        private const string SelectedKey = "AeroRush.SelectedSkin";

        private readonly List<EarnableSkin> skins = new List<EarnableSkin>
        {
            new EarnableSkin("starter", "Trackside", 0, 0, new Color(0.12f, 0.62f, 0.22f)),
            new EarnableSkin("copper-comet", "Copper Comet", 5, 0, new Color(0.78f, 0.31f, 0.12f)),
            new EarnableSkin("frostline", "Frostline", 15, 3, new Color(0.25f, 0.78f, 0.94f)),
            new EarnableSkin("solar-crown", "Solar Crown", 30, 12, new Color(1f, 0.72f, 0.12f))
        };

        public int MatchesPlayed => PlayerPrefs.GetInt(MatchesKey, 0);
        public int Wins => PlayerPrefs.GetInt(WinsKey, 0);
        public IReadOnlyList<EarnableSkin> Skins => skins;
        public event Action ProgressChanged;

        public bool IsUnlocked(EarnableSkin skin)
        {
            return skin != null && MatchesPlayed >= skin.MatchesRequired && Wins >= skin.WinsRequired;
        }

        public bool SelectSkin(string skinId)
        {
            EarnableSkin skin = skins.Find(item => item.Id == skinId);
            if (!IsUnlocked(skin)) return false;
            PlayerPrefs.SetString(SelectedKey, skin.Id);
            PlayerPrefs.Save();
            ProgressChanged?.Invoke();
            return true;
        }

        public EarnableSkin GetSelectedSkin()
        {
            string id = PlayerPrefs.GetString(SelectedKey, "starter");
            EarnableSkin selected = skins.Find(item => item.Id == id);
            return IsUnlocked(selected) ? selected : skins[0];
        }

        /// <summary>Call once when a match has actually completed.</summary>
        public void RecordMatchResult(bool won)
        {
            PlayerPrefs.SetInt(MatchesKey, MatchesPlayed + 1);
            if (won) PlayerPrefs.SetInt(WinsKey, Wins + 1);
            PlayerPrefs.Save();
            ProgressChanged?.Invoke();
        }

        public void ApplySelectedSkin(Renderer[] carRenderers)
        {
            EarnableSkin skin = GetSelectedSkin();
            if (carRenderers == null) return;
            foreach (Renderer renderer in carRenderers)
            {
                if (renderer == null) continue;
                var block = new MaterialPropertyBlock();
                renderer.GetPropertyBlock(block);
                block.SetColor("_BaseColor", skin.BodyColor);
                block.SetColor("_Color", skin.BodyColor); // Built-in render pipeline fallback
                renderer.SetPropertyBlock(block);
            }
        }
    }
}

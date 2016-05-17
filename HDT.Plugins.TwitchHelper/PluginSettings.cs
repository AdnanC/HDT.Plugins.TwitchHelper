using Hearthstone_Deck_Tracker;
using System;
using System.IO;

namespace HDT.Plugins.TwitchHelper
{
    [Serializable]
    public class PluginSettings : Object
    {
        public string statsFileText { get; set; }
        public string statsFileLocation { get; set; }
        public string deckImagesLocation { get; set; }
        public string currentDeckFilename { get; set; }
        public bool changeImage { get; set; }

        public PluginSettings LoadSettings(string filePath)
        {
            PluginSettings settings;
            if (File.Exists(filePath))
            {
                settings = XmlManager<PluginSettings>.Load(filePath);
            }
            else
            {
                settings = new PluginSettings()
                {
                    statsFileText = "{DeckName}: {WinLoss} ({WinLossPercent})",
                    currentDeckFilename = "current-deck.png",
                    deckImagesLocation = "",
                    statsFileLocation = @"currentDeck.txt",
                    changeImage = false
                };

                SaveSettings(filePath);
            }

            return settings;
        }

        public void SaveSettings(string filePath)
        {
            if(!Directory.Exists(TwitchHelperPlugin._PluginDataDir))
            {
                Directory.CreateDirectory(TwitchHelperPlugin._PluginDataDir);
            }

            if (Directory.Exists(TwitchHelperPlugin._PluginDataDir))
                XmlManager<PluginSettings>.Save(filePath, this);
        }
    }
}
using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using HearthDb.Enums;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using CoreAPI = Hearthstone_Deck_Tracker.API.Core;

namespace HDT.Plugins.TwitchHelper
{
    internal class TwitchHelper
    {
        private int _mana = 0;
        private PluginSettings _appSettings;

        internal List<Entity> Entities =>
            Helper.DeepClone<Dictionary<int, Entity>>(CoreAPI.Game.Entities).Values.ToList<Entity>();

        internal Entity Opponent => Entities?.FirstOrDefault(x => x.IsOpponent);

        public TwitchHelper(PluginSettings settings)
        {
            _appSettings = settings;
        }

        internal void GameStart()
        {
            writeFile();
        }

        internal void GameEnd()
        {
            writeFile();
        }

        internal void DeckSelected(Deck deck)
        {
            try
            {
                //change deck image file       
                if (_appSettings.changeImage)
                {
                    string deckImagesLocation = _appSettings.deckImagesLocation;
                    string selectedDeckFilename = Path.Combine(deckImagesLocation, deck.Name + ".png");
                    string currentDeckFilename = Path.Combine(deckImagesLocation, _appSettings.currentDeckFilename);

                    if (DeckList.Instance.ActiveDeck != null && DeckList.Instance.ActiveDeck.DeckId == deck.DeckId)
                    {
                        Log.Info("Changing Deck Image: " + selectedDeckFilename + " => " + currentDeckFilename);
                        if (File.Exists(selectedDeckFilename))
                        {
                            Image.FromFile(selectedDeckFilename).Save(currentDeckFilename, ImageFormat.Png);
                        }
                        else
                            Properties.Resources.blank.Save(currentDeckFilename, ImageFormat.Png);
                    }
                    else
                    {
                        Properties.Resources.blank.Save(currentDeckFilename, ImageFormat.Png);
                    }
                }

                // write Win/Loss to file
                writeFile();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        internal void TurnStart(ActivePlayer currentPlayer)
        {
            try
            {
                if (currentPlayer == ActivePlayer.Player && Opponent != null)
                {
                    var mana = AvailableMana();
                    //Log.Info(DisplayObjectInfo(Core.Game.Player));
                }
            }
            catch
            {

            }
        }

        public static string DisplayObjectInfo(Object o)
        {
            StringBuilder sb = new StringBuilder();

            // Include the type of the object
            System.Type type = o.GetType();
            sb.Append("Type: " + type.Name);

            // Include information for each Field
            sb.Append("\r\n\r\nFields:");
            System.Reflection.FieldInfo[] fi = type.GetFields();
            if (fi.Length > 0)
            {
                foreach (FieldInfo f in fi)
                {
                    sb.Append("\r\n " + f.ToString() + " = " +
                              f.GetValue(o));
                }
            }
            else
                sb.Append("\r\n None");

            // Include information for each Property
            sb.Append("\r\n\r\nProperties:");
            System.Reflection.PropertyInfo[] pi = type.GetProperties();
            if (pi.Length > 0)
            {
                foreach (PropertyInfo p in pi)
                {
                    sb.Append("\r\n " + p.ToString() + " = " +
                              p.GetValue(o, null));
                }
            }
            else
                sb.Append("\r\n None");

            return sb.ToString();
        }

        internal int AvailableMana()
        {
            var opp = Opponent;
            if (opp != null)
            {
                var mana = opp.GetTag(GameTag.RESOURCES);
                var overload = opp.GetTag(GameTag.OVERLOAD_OWED);
                _mana = mana + 1 - overload;
            }
            return _mana;
        }
        private async void writeFile()
        {
            await PutTaskDelay();
            try
            {
                if (_appSettings.twitchFile)
                {
                    if (DeckList.Instance.ActiveDeck != null)
                    {
                        string sfilePath = _appSettings.twitchFilename;

                        string sDeckName = DeckList.Instance.ActiveDeck.Name;
                        string sWinLossString = DeckList.Instance.ActiveDeck.WinLossString;
                        string sWinLossPercent = DeckList.Instance.ActiveDeck.WinPercentString;

                        string sFileText = _appSettings.twitchFileText;
                        string sDeckStatsString = _appSettings.twitchDeckStatsString;

                        string selectDecksStats = "";
                        string activeDeckStats = "";
                        string tempStats = "";

                        tempStats = sDeckStatsString;
                        tempStats = tempStats.Replace("{DeckName}", sDeckName);
                        tempStats = tempStats.Replace("{WinLoss}", sWinLossString);
                        tempStats = tempStats.Replace("{WinLossPercent}", sWinLossPercent);
                        activeDeckStats = tempStats;

                        foreach (Deck deck in _appSettings.selectedDecks)
                        {
                            if (deck.DeckId != DeckList.Instance.ActiveDeck.DeckId)
                            {
                                tempStats = sDeckStatsString;
                                tempStats = tempStats.Replace("{DeckName}", deck.Name);
                                tempStats = tempStats.Replace("{WinLoss}", deck.WinLossString);
                                tempStats = tempStats.Replace("{WinLossPercent}", deck.WinPercentString);
                                selectDecksStats += tempStats + "\n";
                            }
                        }

                        sFileText = sFileText.Replace("{ActiveDeck}", activeDeckStats);
                        sFileText = sFileText.Replace("{SelectedDecks}", selectDecksStats.TrimEnd('\r', '\n'));

                        File.WriteAllText(sfilePath, sFileText);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        async Task PutTaskDelay()
        {
            await Task.Delay(500);
        }
    }
}
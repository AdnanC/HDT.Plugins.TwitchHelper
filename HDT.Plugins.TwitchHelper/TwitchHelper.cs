using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
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
                if (_appSettings.changeImage)
                {
                    string deckImageLocation = _appSettings.deckImagesLocation;
                    string selectedDeckFilename = deckImageLocation + deck.Name + ".png";
                    string currentDeckFilename = deckImageLocation + _appSettings.currentDeckFilename;

                    Log.Info("Changing Deck Image: " + selectedDeckFilename + " => " + currentDeckFilename);
                    if (File.Exists(selectedDeckFilename))
                    {
                        File.Copy(selectedDeckFilename, currentDeckFilename, true);
                    }
                }
                writeFile();
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
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
                // looking a turn ahead, so add one mana
                _mana = mana + 1 - overload;
            }
            return _mana;
        }
        private void writeFile()
        {
            try
            {
                if (DeckList.Instance.ActiveDeck != null)
                {
                    string sfilePath = _appSettings.statsFileLocation;

                    string sWinLossString = DeckList.Instance.ActiveDeck.WinLossString;
                    string sWinLossPercent = DeckList.Instance.ActiveDeck.WinPercentString;
                    string sDeckName = DeckList.Instance.ActiveDeck.Name;

                    string sFileText = _appSettings.statsFileText;

                    sFileText = sFileText.Replace("{DeckName}", sDeckName);
                    sFileText = sFileText.Replace("{WinLoss}", sWinLossString);
                    sFileText = sFileText.Replace("{WinLossPercent}", sWinLossPercent);

                    File.WriteAllText(sfilePath, sFileText);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
using System;
using System.IO;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Utility.Logging;
using Hearthstone_Deck_Tracker.Plugins;
using HDT.Plugins.TwitchHelper.Controls;

namespace HDT.Plugins.TwitchHelper
{
    public class TwitchHelperPlugin : IPlugin
    {
        private MenuItem _DeckStatsMenuItem;
        private MainWindow _MainWindow = null;

        internal static string _PluginDataDir => Path.Combine(Hearthstone_Deck_Tracker.Config.Instance.DataDir, "TwitchHelper");
        internal static string _SettingsFile => Path.Combine(_PluginDataDir, "TwitchHelper.config.xml");

        public PluginSettings plugingSettings = new PluginSettings();

        public string Author
        {
            get { return "AdnanC"; }
        }

        public string ButtonText
        {
            get { return "Settings"; }
        }

        public string Description
        {
            get { return "Write Win/Loss Results to file after a game ends. Change deck image when you select a new deck"; }
        }

        public MenuItem MenuItem
        {
            //get { return null; }
            get { return _DeckStatsMenuItem; }
        }

        public string Name
        {
            get { return "Twitch Helper"; }
        }        

        public void OnButtonPress()
        {            
        }

        public void OnLoad()
        {
            try
            {
                _DeckStatsMenuItem = new PluginMenu();

                plugingSettings.LoadSettings(_SettingsFile);

                TwitchHelper twitchHelper = new TwitchHelper(plugingSettings);

                _DeckStatsMenuItem.Click += (sender, args) =>
                {
                    try
                    {
                        _MainWindow = new MainWindow(plugingSettings);
                        _MainWindow.Show();                        
                    }
                    catch(Exception ex)
                    {
                        Log.Error(ex);
                    }
                };
                                
                GameEvents.OnGameLost.Add(twitchHelper.GameEnd);
                GameEvents.OnGameWon.Add(twitchHelper.GameEnd);
                GameEvents.OnGameTied.Add(twitchHelper.GameEnd);
                //GameEvents.OnGameEnd.Add(publishStats.GameEnd);
                GameEvents.OnGameStart.Add(twitchHelper.GameStart);
                GameEvents.OnTurnStart.Add(twitchHelper.TurnStart);
                DeckManagerEvents.OnDeckSelected.Add(twitchHelper.DeckSelected);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void OnUnload()
        {
            plugingSettings.SaveSettings(_SettingsFile);
        }

        public void OnUpdate()
        {
            
        }

        public Version Version
        {
            get { return new Version(0, 0, 2); }
        }
    }
}

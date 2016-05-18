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
        private MenuItem _TwitchHelperMenuItem;
        private MainWindow _MainWindow = null;
        private TwitchHelper _twitchHelper;
        
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
            get { return _TwitchHelperMenuItem; }
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
                _TwitchHelperMenuItem = new PluginMenu();

                plugingSettings.LoadSettings(_SettingsFile);

                plugingSettings.settingUpdated += new EventHandler(setttingsChanged);

                _twitchHelper = new TwitchHelper(plugingSettings);

                _TwitchHelperMenuItem.Click += (sender, args) =>
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
                                
                GameEvents.OnGameLost.Add(_twitchHelper.GameEnd);
                GameEvents.OnGameWon.Add(_twitchHelper.GameEnd);
                GameEvents.OnGameTied.Add(_twitchHelper.GameEnd);
                GameEvents.OnGameStart.Add(_twitchHelper.GameStart);
                GameEvents.OnTurnStart.Add(_twitchHelper.TurnStart);
                DeckManagerEvents.OnDeckSelected.Add(_twitchHelper.DeckSelected);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
        }

        public void OnUnload()
        {
                        
        }

        public void OnUpdate()
        {
            
        }

        public Version Version
        {
            get { return new Version(0, 0, 2); }
        }

        public void setttingsChanged(object s, EventArgs e)
        {
            _twitchHelper.GameEnd();
        }
    }
}
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Hearthstone;

namespace HDT.Plugins.TwitchHelper.Controls
{
    public partial class THMainWindow
    {
        private PluginSettings _appSettings;
        private Brush _defaultBorderBrush;
        private Brush _errorBrush;

        public THMainWindow(PluginSettings settings)
        {
            InitializeComponent();

            _appSettings = settings;
            _defaultBorderBrush = tbDeckImagesFolder.BorderBrush;
            _errorBrush = new SolidColorBrush(Colors.Red);

            cbTwitchFile.IsChecked = _appSettings.twitchFile;
            tbTwitchFilename.Text = _appSettings.twitchFilename;            
            tbTwitchFileText.Text = _appSettings.twitchFileText;
            tbDeckImagesFolder.Text = _appSettings.deckImagesLocation;
            tbCurrentDeckImage.Text = _appSettings.currentDeckFilename;
            cbChangeImage.IsChecked = _appSettings.changeImage;

            ObservableCollection<Deck> deckList = DeckList.Instance.Decks;
            lbDecks.Items.Clear();

            foreach (var deck in deckList.Where(
                                                    d => !d.IsArenaDeck &&
                                                    !d.Archived
                                                ).OrderBy(d => d.Class))
            {
                lbDecks.Items.Add(deck);

                if (_appSettings.selectedDecks.Exists(d => d.DeckId == deck.DeckId))
                    lbDecks.SelectedItems.Add(deck);
            }            
        }

        private void tbFileText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(cbChangeImage.IsChecked == true)
            {
                if(tbDeckImagesFolder.Text.Trim() == "" || !Directory.Exists(tbDeckImagesFolder.Text.Trim()))
                {
                    tbError.Text = "Please enter valid directory for deck images.";
                    tbDeckImagesFolder.BorderBrush = _errorBrush;
                    return;
                }

                if(string.IsNullOrEmpty(tbCurrentDeckImage.Text) || tbCurrentDeckImage.Text.IndexOfAny(Path.GetInvalidFileNameChars()) > 0)
                {
                    tbError.Text = "Please choose a valid filename for current deck image.";
                    tbCurrentDeckImage.BorderBrush = _errorBrush;
                    return;
                }
            }

            tbCurrentDeckImage.BorderBrush = _defaultBorderBrush;
            tbDeckImagesFolder.BorderBrush = _defaultBorderBrush;
            tbError.Text = "";

            tbTwitchFileText.SelectAll();
            
            _appSettings.twitchFileText = tbTwitchFileText.Text.Trim();
            _appSettings.twitchFilename = tbTwitchFilename.Text.Trim();
            _appSettings.currentDeckFilename = tbCurrentDeckImage.Text.Trim();
            _appSettings.deckImagesLocation = tbDeckImagesFolder.Text.Trim();

            _appSettings.selectedDecks.Clear();

            foreach (Deck selectedDeck in lbDecks.SelectedItems)
            {
                _appSettings.selectedDecks.Add(selectedDeck);
            }

            _appSettings.SaveSettings(TwitchHelperPlugin._SettingsFile);
        }

        private void cbChangeImage_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            tbCurrentDeckImage.IsEnabled = true;
            lbCurrentDeckFile.IsEnabled = true;
            lbDeckImagesFolder.IsEnabled = true;
            tbDeckImagesFolder.IsEnabled = true;
            btnDeckImagesFolder.IsEnabled = true;
            btnCurrentDeckFile.IsEnabled = true;
            _appSettings.changeImage = true;
        }


        private void cbChangeImage_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            tbCurrentDeckImage.IsEnabled = false;
            lbCurrentDeckFile.IsEnabled = false;
            lbDeckImagesFolder.IsEnabled = false;
            tbDeckImagesFolder.IsEnabled = false;
            btnDeckImagesFolder.IsEnabled = false;
            btnCurrentDeckFile.IsEnabled = false;
            _appSettings.changeImage = false;
        }

        private void btnDeckImagesFolder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                tbDeckImagesFolder.Text = fbd.SelectedPath.Trim();
                _appSettings.deckImagesLocation = fbd.SelectedPath.Trim();
            }
        }

        private void btnDeckName_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var selectionIndex = tbDeckStatsString.SelectionStart;
            tbDeckStatsString.Text = tbDeckStatsString.Text.Insert(tbDeckStatsString.SelectionStart, "{DeckName}");
            tbDeckStatsString.SelectionStart = selectionIndex + "{DeckName}".Length;

            tbDeckStatsString.Focus();
        }

        private void btnWinLossString_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            var selectionIndex = tbDeckStatsString.SelectionStart;
            tbDeckStatsString.Text = tbDeckStatsString.Text.Insert(tbDeckStatsString.SelectionStart, "{WinLoss}");
            tbDeckStatsString.SelectionStart = selectionIndex + "{WinLoss}".Length;

            tbDeckStatsString.Focus();
        }

        private void btnWinLossPercent_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            var selectionIndex = tbDeckStatsString.SelectionStart;
            tbDeckStatsString.Text = tbDeckStatsString.Text.Insert(tbDeckStatsString.SelectionStart, "{WinLossPercent}");
            tbDeckStatsString.SelectionStart = selectionIndex + "{WinLossPercent}".Length;

            tbDeckStatsString.Focus();
        }

        private void btnCurrentDeckFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {          
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.Filter = "PNG Files (.png)|*.png";
            fileDialog.FilterIndex = 1;

            fileDialog.Multiselect = false;

            if(Directory.Exists(tbDeckImagesFolder.Text))
            {
                fileDialog.InitialDirectory = tbDeckImagesFolder.Text;
            }

            DialogResult result = fileDialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fileDialog.SafeFileName))
            {
                tbCurrentDeckImage.Text = fileDialog.SafeFileName.Trim();
                _appSettings.currentDeckFilename = fileDialog.SafeFileName.Trim();
            }
        }

        private void btnTwitchFile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            fileDialog.Filter = "TXT Files (.txt)|*.txt";
            fileDialog.FilterIndex = 1;

            fileDialog.Multiselect = false;            

            DialogResult result = fileDialog.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fileDialog.SafeFileName))
            {
                tbTwitchFilename.Text = fileDialog.FileName.Trim();
                _appSettings.twitchFilename = fileDialog.FileName.Trim();
            }
        }

        private void cbTwitchFile_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            lbTwitchFilename.IsEnabled = true;
            lbTwitchFileText.IsEnabled = true;
            tbTwitchFilename.IsEnabled = true;
            tbTwitchFileText.IsEnabled = true;
            btnTwitchFile.IsEnabled = true;
            btnDeckName.IsEnabled = true;
            btnWinLossPercent.IsEnabled = true;
            btnWinLossString.IsEnabled = true;
            tbDeckStatsString.IsEnabled = true;
            lbDeckString.IsEnabled = true;
            lbDecks.IsEnabled = true;

            _appSettings.twitchFile = true;
        }

        private void cbTwitchFile_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            lbTwitchFilename.IsEnabled = false;
            lbTwitchFileText.IsEnabled = false;
            tbTwitchFilename.IsEnabled = false;
            tbTwitchFileText.IsEnabled = false;
            btnTwitchFile.IsEnabled = false;
            btnDeckName.IsEnabled = false;
            btnWinLossPercent.IsEnabled = false;
            btnWinLossString.IsEnabled = false;
            tbDeckStatsString.IsEnabled = false;
            lbDeckString.IsEnabled = false;
            lbDecks.IsEnabled = false;

            _appSettings.twitchFile = false;
        }

        private void lbDecks_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            
        }
    }
}

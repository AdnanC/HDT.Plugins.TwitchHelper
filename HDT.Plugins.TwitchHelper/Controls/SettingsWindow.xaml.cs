using System.Windows.Forms;
using System.Windows.Media;
using System.IO;
using System.Windows.Documents;

namespace HDT.Plugins.TwitchHelper.Controls
{
    public partial class MainWindow
    {
        private PluginSettings _appSettings;
        private Brush _defaultBorderBrush;
        private Brush _errorBrush;

        public MainWindow(PluginSettings settings)
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
            var selectionIndex = tbTwitchFileText.SelectionStart;
            tbTwitchFileText.Text = tbTwitchFileText.Text.Insert(tbTwitchFileText.SelectionStart, "{DeckName}");
            tbTwitchFileText.SelectionStart = selectionIndex + "{DeckName}".Length;

            tbTwitchFileText.Focus();
        }

        private void btnWinLossString_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            var selectionIndex = tbTwitchFileText.SelectionStart;
            tbTwitchFileText.Text = tbTwitchFileText.Text.Insert(tbTwitchFileText.SelectionStart, "{WinLoss}");
            tbTwitchFileText.SelectionStart = selectionIndex + "{WinLoss}".Length;

            tbTwitchFileText.Focus();
        }

        private void btnWinLossPercent_Click(object sender, System.Windows.RoutedEventArgs e)
        {            
            var selectionIndex = tbTwitchFileText.SelectionStart;
            tbTwitchFileText.Text = tbTwitchFileText.Text.Insert(tbTwitchFileText.SelectionStart, "{WinLossPercent}");
            tbTwitchFileText.SelectionStart = selectionIndex + "{WinLossPercent}".Length;

            tbTwitchFileText.Focus();
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

            _appSettings.twitchFile = false;
        }
    }
}

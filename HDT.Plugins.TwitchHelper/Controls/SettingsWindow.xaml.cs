using System.Windows.Forms;

namespace HDT.Plugins.TwitchHelper.Controls
{
    public partial class MainWindow
    {
        private PluginSettings _appSettings;

        public MainWindow(PluginSettings settings)
        {
            InitializeComponent();

            _appSettings = settings;

            tbFileText.Document.Blocks.Clear();
            tbFileText.AppendText(_appSettings.statsFileText);
            tbDeckImagesFolder.Text = _appSettings.deckImagesLocation;
            tbCurrentDeckImage.Text = _appSettings.currentDeckFilename;
            cbChangeImage.IsChecked = _appSettings.changeImage;
        }

        private void tbFileText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void btnSave_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbFileText.SelectAll();
            _appSettings.statsFileText = tbFileText.Selection.Text.Trim(); 
            _appSettings.SaveSettings(TwitchHelperPlugin._SettingsFile);
        }

        private void cbChangeImage_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            tbCurrentDeckImage.IsEnabled = true;
            lbCurrentDeckFile.IsEnabled = true;
            lbDeckImagesFolder.IsEnabled = true;
            tbDeckImagesFolder.IsEnabled = true;
            btnDeckImagesFolder.IsEnabled = true;
            _appSettings.changeImage = true;
        }


        private void cbChangeImage_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            tbCurrentDeckImage.IsEnabled = false;
            lbCurrentDeckFile.IsEnabled = false;
            lbDeckImagesFolder.IsEnabled = false;
            tbDeckImagesFolder.IsEnabled = false;
            btnDeckImagesFolder.IsEnabled = false;
            _appSettings.changeImage = false;
        }

        private void btnDeckImagesFolder_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                tbDeckImagesFolder.Text = fbd.SelectedPath.ToString();
                _appSettings.deckImagesLocation = fbd.SelectedPath.ToString();
            }
        }

        private void btnDeckName_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbFileText.CaretPosition.InsertTextInRun("{DeckName}");
        }

        private void btnWinLossString_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbFileText.CaretPosition.InsertTextInRun("{WinLoss}");
        }

        private void btnWinLossPercent_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbFileText.CaretPosition.InsertTextInRun("{WinLossPercent}");
        }
    }
}

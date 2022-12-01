using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NighttimeDisplayDimmer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OptionsViewModel model { get => (OptionsViewModel)this.DataContext; }
        
        public static RoutedCommand PreviewCommand = new RoutedCommand();
        public static RoutedCommand SaveCommand = new RoutedCommand();
        public static RoutedCommand SetDayCommand = new RoutedCommand();
        public static RoutedCommand SetNightCommand = new RoutedCommand();
        public MainWindow()
        {
            InitializeComponent();
        }

        
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!model.RemoteSession)
            {
                await model.LoadDisplays(Dispatcher);
            }
        }

        private async void PreviewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //MessageBox.Show("Preview Command Executed");
            await model.ChangeBrightness(Dispatcher, true);
        }

        private void PreviewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //MessageBox.Show("Preview Command Executed");
            model.SaveDisplays();
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !model.Loading;
        }

        private void SetDayCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //MessageBox.Show("Preview Command Executed");
            model.SetCurrentToConfig(ConfigType.DAY);
        }

        private void SetDayCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !model.Loading;
        }

        private void SetNightCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //MessageBox.Show("Preview Command Executed");
            model.SetCurrentToConfig(ConfigType.NIGHT);
        }

        private void SetNightCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !model.Loading;
        }

        private void RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Util.Links.Open(e.Uri.AbsoluteUri);
        }

        private void optionsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(model.IsDirty)
            {
                MessageBoxResult? r = ModernWpf.MessageBox.Show(Properties.Localization.UnsavedChangesPromptText, Properties.Localization.UnsavedChangesPromptTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if(r == MessageBoxResult.Yes)
                {
                    model.SaveDisplays();    
                } 
                else if(r == MessageBoxResult.Cancel) 
                {
                    e.Cancel = true;
                }
            }
        }

        public static void HandleOpen()
        {
            if (
#if DEBUG
                            Application.Current.MainWindow == null || System.Diagnostics.Debugger.IsAttached
#else
                            Application.Current.MainWindow == null
#endif
                        )
            {

                Application.Current.MainWindow = new MainWindow();
                Application.Current.MainWindow.Show();
            }
            else
            {
                Application.Current.MainWindow.Activate();
            }
        }
    }
}

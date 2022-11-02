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
        public MainWindow()
        {
            InitializeComponent();
        }

        
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await model.LoadDisplays(Dispatcher);
        }

        private async void PreviewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //MessageBox.Show("Preview Command Executed");
            await model.ChangeBrightness(Dispatcher);
        }

        private void PreviewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}

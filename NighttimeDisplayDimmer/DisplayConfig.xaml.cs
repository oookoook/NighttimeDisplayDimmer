using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
    /// Interaction logic for DisplayConfig.xaml
    /// </summary>
    public partial class DisplayConfig : UserControl
    {
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(DisplayConfig), new FrameworkPropertyMetadata() { DefaultValue = "N/A",  });

        public string? Label {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }
        
        public static readonly DependencyProperty BrightnessCurrentProperty =
            DependencyProperty.Register("BrightnessCurrent", typeof(int), typeof(DisplayConfig), new FrameworkPropertyMetadata() { DefaultValue = 50 });
        
        public int BrightnessCurrent {
            get { return (int)GetValue(BrightnessCurrentProperty); }
            set { SetValue(BrightnessCurrentProperty, value); }
        }

        public static readonly DependencyProperty BrightnessMinimumProperty =
            DependencyProperty.Register("BrightnessMinimum", typeof(int), typeof(DisplayConfig), new FrameworkPropertyMetadata() { DefaultValue = 0 });
        public int BrightnessMinimum {
            get { return (int)GetValue(BrightnessMinimumProperty); }
            set { SetValue(BrightnessMinimumProperty, value); }
        }

        public static readonly DependencyProperty BrightnessMaximumProperty =
            DependencyProperty.Register("BrightnessMaximum", typeof(int), typeof(DisplayConfig), new FrameworkPropertyMetadata() { DefaultValue = 100 });
        public int BrightnessMaximum {
            get { return (int)GetValue(BrightnessMaximumProperty); }
            set { SetValue(BrightnessMaximumProperty, value); }
        }
        public DisplayConfig()
        {
            InitializeComponent();
            // this cant be here as it destroys the bindings!
            //this.DataContext = this;
        }

        private void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            string x="FOOO";
        }
    }
}

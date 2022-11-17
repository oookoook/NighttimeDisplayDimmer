using ABI.System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NighttimeDisplayDimmer
{
    // https://www.codeproject.com/Articles/36468/WPF-NotifyIcon-2
    public class TaskbarIconViewModel : INotifyPropertyChanged
    {

        
        
        /// <summary>
        /// Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    // hack to allow open options window in debug mode
                    CanExecuteFunc = () => {
#if DEBUG
                        return Application.Current.MainWindow == null || System.Diagnostics.Debugger.IsAttached;
#else
                        return Application.Current.MainWindow == null;
#endif
                     },
                    CommandAction = () =>
                    {
                        Application.Current.MainWindow = new MainWindow();
                        Application.Current.MainWindow.Show();
                    }
                };
            }
        }

        /// <summary>
        /// Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand HideWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => Application.Current.MainWindow.Close(),
                    CanExecuteFunc = () => Application.Current.MainWindow != null
                };
            }
        }

        public ICommand ShowHelpCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => Util.Links.Open(Util.Links.Help),
                    CanExecuteFunc = () => true
                };
            }
        }

        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
            }
        }

        /* All this is useless as there is no good way to get the windows theme (Windows 10 distinguishes system and app theme)
        private object icon;
        public object Icon { get => icon; set { icon = value; NotifyPropertyChanged(); } }

        private Windows.UI.ViewManagement.UISettings uiSettings = new Windows.UI.ViewManagement.UISettings();

        public TaskbarIconViewModel()
        {
            uiSettings.ColorValuesChanged += UiSettings_ColorValuesChanged;
            SetIcon();
           
        }

        private void UiSettings_ColorValuesChanged(Windows.UI.ViewManagement.UISettings sender, object args)
        {
            SetIcon();
        }

        public void SetIcon()
        {
            Windows.UI.Color color = uiSettings.GetColorValue(
                                    Windows.UI.ViewManagement.UIColorType.Background
                                   );
            if (color.Equals(Windows.UI.Color.FromArgb(255, 0, 0, 0)))
            {
                icon = "Resources/logow.ico"; //Application.Current.FindResource("Resources/Logo-white.ico");
            }
            else
            {
                icon = "Resources/logob.ico"; //Application.Current.FindResource("Resources/Logo-white.ico");
            }
        }
        */

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Simplistic delegate command for the demo.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public Action? CommandAction { get; set; }
        public Func<bool>? CanExecuteFunc { get; set; }

        public void Execute(object? parameter)
        {
            if(CommandAction == null)
            {
                return;
            }
            CommandAction();
        }

        public bool CanExecute(object? parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

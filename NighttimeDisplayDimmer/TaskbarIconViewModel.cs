using ABI.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NighttimeDisplayDimmer
{
    // https://www.codeproject.com/Articles/36468/WPF-NotifyIcon-2
    public class TaskbarIconViewModel
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

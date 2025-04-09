using System;
using System.Windows;
using MemoryMatch.Views;

namespace MemoryMatch
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            SignInView mainWindow = new SignInView();
            mainWindow.Show();
        }
    }
}

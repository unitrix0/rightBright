using Prism.Ioc;
using Prism.Unity;
using System;
using System.Windows;
using unitrix0.rightbright.Windows;

namespace unitrix0.rightbright
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //throw new NotImplementedException();
        }
    }
}

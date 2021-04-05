using System.Collections.Generic;
using unitrix0.rightbright.Monitors.Models;
using unitrix0.rightbright.Monitors.MonitorAPI;

namespace unitrix0.rightbright.Windows.ViewModel
{
    public class MainWindowViewModel
    {
        public List<DisplayInfo> Monitors { get; }

        public MainWindowViewModel()
        {
            var moniEnum = new MonitorEnummerationService();
            Monitors = moniEnum.GetDisplays();

            //Monitors = new List<DisplayInfo>()
            //{
            //    new DisplayInfo()
            //    {
            //        DeviceName = "LG Ultragear",
            //        ScreenHeight = 1440,
            //        ScreenWidth = 2560
            //    },
            //    new DisplayInfo()
            //    {
            //        DeviceName = "HP Compaq LA2405wg",
            //        ScreenHeight = 1600,
            //        ScreenWidth = 1200
            //    }
            //};
        }
    }
}
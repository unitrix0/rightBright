using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SetBrightness
{
    public partial class Form1 : Form
    {
        private YSensor.TimedReportCallback timedReport = new YSensor.TimedReportCallback((sensor, measure) =>
        {
            var val = measure.get_averageValue();
            Debug.Print(val.ToString(CultureInfo.CurrentCulture));
        });

        private string _err;
        private Timer _t = new Timer();
        public Form1()
        {
            InitializeComponent();
            _t.Interval = 1000;
            _t.Enabled = true;
            _t.Tick += (sender, args) =>
            {
                YAPI.HandleEvents(ref _err);
            };

            YAPI.RegisterHub("USB", ref _err);
            YAPI.UpdateDeviceList(ref _err);
            var sensor = YSensor.FirstSensor();
            sensor.registerTimedReportCallback(timedReport);
            _t.Start();

            tbLux.Text = sensor.FriendlyName;
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            //brightness = new BrightnessHelper(this.Handle);
            short.TryParse(tbValue.Text, out var value);
            //brightness.SetBrightness(value);

            var enummerator = new MonitorEnummerator();
            var mons = enummerator.GetDisplays();
            foreach (var monitor in mons)
            {
                var brightness = new BrightnessHelper();
                brightness.SetBrightness(monitor.Handle, value);
            }
        }
    }
}

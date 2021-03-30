using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SetBrightness
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var err = "";
            YAPI.RegisterHub("USB", ref err);
            YAPI.UpdateDeviceList(ref err);
            var sensor = YSensor.FirstSensor();
            
            tbLux.Text = sensor.get_advertisedValue();
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

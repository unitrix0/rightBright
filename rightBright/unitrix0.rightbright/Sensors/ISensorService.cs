using System;

namespace unitrix0.rightbright.Sensors
{
    public interface ISensorService
    {
        event EventHandler<double> Update;
    }
}
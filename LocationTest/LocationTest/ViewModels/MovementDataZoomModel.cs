using System;
using System.Collections.Generic;
using System.Text;

namespace LocationTest.ViewModels
{
    class MovementDataZoomModel:IGraphZoomModel
    {
        public int Max => 30000;
            public int Min => -30000;
            public string[] GraphItems => new[] { "AccelerometerX", "AccelerometerY", "AccelerometerZ", "GyroX", "GyroY", "GyroZ" };
     }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace LocationTest.ViewModels
{
    class PMZoomModel: IGraphZoomModel

    {
        public int Max => 30;
        public int Min => 0;
        public string[] GraphItems => new[] { "PM10p0", "PM4p0", "PM2p5", "PM1p0" };
    }
}

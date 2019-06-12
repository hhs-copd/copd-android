using System;
using System.Collections.Generic;
using System.Text;

namespace LocationTest.ViewModels
{
    class ThoraxZoomModel : IGraphZoomModel
    {
        public int Max => 400;
        public int Min => 0;
        public string[] GraphItems => new[] { "ThoraxCircumference" };
    }
}

using System;

namespace LocationTest.Services
{
    public class BluetoothHandler
    {
        public Action<string> OnConnect { get; set; }

        public Action<string> OnDisconnect { get; set; }

        public Action OnWrite { get; set; }
    }
}

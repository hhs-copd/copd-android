using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationTest.Services
{
    public interface IBluetoothService
    {
        Task Scan();

        List<string> GetConnectedDevices();

        void Listen(BluetoothHandler handler = null);
    }
}

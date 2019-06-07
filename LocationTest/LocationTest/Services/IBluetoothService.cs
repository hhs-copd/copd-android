using System.Threading.Tasks;

namespace LocationTest.Services
{
    public interface IBluetoothService
    {
        Task ConnectAndWrite(BluetoothHandler handler = null);
    }
}

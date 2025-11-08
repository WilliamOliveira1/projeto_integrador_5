using PIV.Models;

namespace PIV.interfaces
{
    public interface ISaveDataFromSensor
    {
        void SeedData(string jsonData);

        void DeleteAllSensorData();
    }
}
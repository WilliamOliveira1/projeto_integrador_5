﻿using PIV.Models;

namespace PIV.interfaces
{
    public interface IRainfallDataFromCsv
    {
        void StartRainfallData();
        Task StartRainfallDataGetterAsync(CancellationToken cancellationToken);

        List<Weather> getRainfallData();

        bool saveNewRainfallData(string humidity, string temperature, string date);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Domain.Interfaces
{
    public interface IVehicleService
    {
        List<Vehicles> AllVehicles(int page = 1, string? name = null, string? model = null);

        Vehicles? FindById(int id);
        Vehicles Save(Vehicles vehicle);
        void Update(Vehicles vehicle);
        void Delete(Vehicles vehicle);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Infrastructure;

namespace MinimalApi.Domain.Services
{
    public class VehicleService : IVehicleService
    {

        private readonly Dbcontext _context;

        public VehicleService(Dbcontext context)
        {
            _context = context;
        }

        public List<Vehicles> AllVehicles(int? page = 1, string? name = null, string? model = null)
        {
            var query = _context.Vehicles.AsQueryable();
            if(!string.IsNullOrEmpty(name))
            {
                query = query.Where(v => v.Name.ToLower().Contains(name.ToLower()));
            }

            if(!string.IsNullOrEmpty(model))
            {
                query = query.Where(v => v.Model.ToLower().Contains(model.ToLower()));
            }
            if(page != null)
            {
                query = query.Skip(((int)page - 1) * 10).Take(10);
            }

            return query.ToList();
        }

        public void Delete(Vehicles vehicle)
        {
            _context.Vehicles.Remove(vehicle);
            _context.SaveChanges();
        }

        public Vehicles? FindById(int id)
        {
            return _context.Vehicles.Where(v => v.Id == id).FirstOrDefault();
        }

        public Vehicles Save(Vehicles vehicle)
        {
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();
            return vehicle;
        }

        public void Update(Vehicles vehicles)
        {
            _context.Vehicles.Update(vehicles);
            _context.SaveChanges();
        }
    }
}
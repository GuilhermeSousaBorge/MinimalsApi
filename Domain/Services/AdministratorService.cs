using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interfaces;
using MinimalApi.DTO;
using MinimalApi.Infrastructure;

namespace MinimalApi.Domain.Services
{
    public class AdministratorService : IAdministratorService
    {
        private readonly Dbcontext _context;

        public AdministratorService(Dbcontext context)
        {
            _context = context;
        }

        public List<Administrator> AllAdministrators(int? page)
        {
            var query = _context.Administrators.AsQueryable();
            
            if(page != null)
            {
                query = query.Skip(((int)page - 1) * 10).Take(10);
            }

            return query.ToList();
        }

        public Administrator? FindById(int id)
        {
            return _context.Administrators.Where(a => a.Id == id).FirstOrDefault();
        }

        public Administrator? Login(LoginDTO loginDTO)
        {
            var adm = _context.Administrators.Where(adm => adm.Email == loginDTO.Email && adm.Password == loginDTO.Password).FirstOrDefault();
            return adm;
        }

        public Administrator Save(Administrator administrator)
        {
            _context.Administrators.Add(administrator);
            _context.SaveChanges();
            return administrator;
        }
    }
}
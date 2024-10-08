using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.DTO;

namespace MinimalApi.Domain.Interfaces
{
    public interface IAdministratorService
    {
        Administrator? Login(LoginDTO loginDTO);

        Administrator Save(Administrator administrator);
        Administrator? FindById(int id);
        List<Administrator> AllAdministrators(int? page);
    }
}
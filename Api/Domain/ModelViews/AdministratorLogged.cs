using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApi.Domain.ModelViews
{
    public record AdministratorLogged
    {
        public string Email { get; set; } =  default!;

        public string Role { get; set; } =  default!;

        public string Token { get; set; } = default!;
    }
}
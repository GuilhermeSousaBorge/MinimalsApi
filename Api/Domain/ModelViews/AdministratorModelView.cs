using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApi.Domain.ModelViews
{
    public record AdministratorModelView
    {
        public int Id { get; set; } = default!;
        public string Email { get; set; } =  default!;

        public string Role { get; set; } =  default!;
    }
}
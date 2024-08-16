using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApi.Domain.DTOs
{
    public record VehicleDTO
    {
        public string Name { get; set; } = default!;

        public string Model { get; set; } = default!;

        public int Ano { get; set; } = default!;
    }
}
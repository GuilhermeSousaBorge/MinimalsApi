using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApi.Domain.Entities
{
    public class Vehicles
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } = default!;

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = default!;

        [Required]
        [StringLength(150)]
        public string Model { get; set; } = default!;

        [Required]
        public int Ano { get; set; } = default!;
    }
}
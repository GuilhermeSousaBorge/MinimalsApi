using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalApi.Domain.ModelViews
{
    public struct Home
    {
        public string Welcome { get => "Bem vindo a api";  }
        public string Documentation { get => "/swagger";  }
    }
}
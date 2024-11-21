using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api_desafio.Domain.ModelViews
{
    public class Home
    {
        public string Mensagem { get => "Bem vindo a api"; }
        public string Doc { get => "/swagger"; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace bpFantasy.Domain.Entities
{
    public class Jogador : BaseEntity
    {
        public string Nome { get; set; }
        public string Posicao1 { get; set; }
        public string Posicao2 { get; set; }
    }
}
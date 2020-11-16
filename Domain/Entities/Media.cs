using System;
using System.Collections.Generic;
using System.Text;

namespace bpFantasy.Domain.Entities
{
    public class Media : BaseEntity
    {
        public string Nome { get; set; }
        public int Jogos { get; set; }
        public decimal Minutos { get; set; }
        public int Ano { get; set; }
        public decimal Ast { get; set; }
        public decimal Blk { get; set; }
        public decimal Pt3 { get; set; }
        public decimal Pts { get; set; }
        public decimal Reb { get; set; }
        public decimal Tov { get; set; }
        public decimal Stl { get; set; }
        public decimal PtsFinal { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace bpFantasy.Domain.Entities
{
    public class Time : BaseEntity
    {
        public string Nome { get; set; }
        public string Manager { get; set; }
        public int LigaId { get; set; }
        public Liga Liga { get; set; }
    }
}
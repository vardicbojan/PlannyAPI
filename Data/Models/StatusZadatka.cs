using System;
using System.Collections.Generic;

namespace Planny.Data.Models
{
    public partial class StatusZadatka
    {
        public StatusZadatka()
        {
            Zadatak = new HashSet<Zadatak>();
        }

        public Guid Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<Zadatak> Zadatak { get; set; }
    }
}

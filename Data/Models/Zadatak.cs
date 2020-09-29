using System;
using System.Collections.Generic;

namespace Planny.Data.Models
{
    public partial class Zadatak
    {
        public Guid Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public Guid StatusZadatkaId { get; set; }
        public Guid KorisnikId { get; set; }
        public bool Zakljucan { get; set; }
        public DateTime Pocetak { get; set; }
        public DateTime Kraj { get; set; }
        public bool Active { get; set; }

        public virtual Korisnik Korisnik { get; set; }
        public virtual StatusZadatka StatusZadatka { get; set; }
    }
}

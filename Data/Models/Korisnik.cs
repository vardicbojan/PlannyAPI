using System;
using System.Collections.Generic;

namespace Planny.Data.Models
{
    public partial class Korisnik
    {
        public Korisnik()
        {
            Zadatak = new HashSet<Zadatak>();
        }

        public Guid Id { get; set; }
        public string KorisnickoIme { get; set; }
        public DateTime DatumRegistracije { get; set; }
        public string LozinkaHash { get; set; }
        public string Salt { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public DateTime? DatumRodenja { get; set; }
        public bool JeJavniProfil { get; set; }
        public bool VidljiviSviTaskovi { get; set; }
        public bool Active { get; set; }

        public virtual ICollection<Zadatak> Zadatak { get; set; }
    }
}

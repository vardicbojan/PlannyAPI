using Planny.Data.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Planny.Data.ViewModels
{
    public class KorisnikDTO
    {
        //[JsonIgnore] -> u responsu neces ovo dobit, vrijednost gdje ovo stavis ce ti biti null.... hmmmm..
        public Guid Id { get; set; }

        [Required]
        public string KorisnickoIme { get; set; }

        public DateTime DatumRegistracije { get; set; }

        //[Required]
        public string LozinkaHash { get; set; }

        //[Required]
        public string Salt { get; set; }

        [Required]
        public string Ime { get; set; }

        [Required]
        public string Prezime { get; set; }

        [Required]
        public DateTime? DatumRodenja { get; set; }

        [Required]
        public string UnesenaLozinka { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public bool JeJavniProfil { get; set; }

        [Required]
        public bool VidljiviSviTaskovi { get; set; }

        //private readonly Regex HasNumber = new Regex(@"[0-9]+");
        //private readonly Regex HasUpperChar = new Regex(@"[A-Z]+");
        //private readonly Regex HasMinimum8Chars = new Regex(@".{8,}");
        //public KorisnikError Greska { get; set; }

        //public bool IsPasswordOK()
        //{
        //    bool lozinkaOK = HasNumber.IsMatch(UnesenaLozinka) && HasUpperChar.IsMatch(UnesenaLozinka) && HasMinimum8Chars.IsMatch(UnesenaLozinka);

        //    if (!string.IsNullOrWhiteSpace(UnesenaLozinka))
        //    {
        //        return lozinkaOK;
        //    }
        //    else
        //    {
        //        Greska.ErrorMessage = Constants.Constants.LOZINKA_NEISPRAVNA;
        //        return false;
        //    }
        //}

    }
}

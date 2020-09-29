using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planny.Constants
{
    public class Constants
    {
        public const string GENERICKA_GRESKA = "Nešto je pošlo po krivu.";

        public const string KORISNIK_LOZINKA_NEISPRAVNA = "Neispravna lozinka. Lozinka mora sadržavati minimalno 8 znakova, veliko slovo i broj.";
        public const string KORISNIK_ZAUZETO_KORISNICKO_IME = "Korisničko ime već postoji.";
        public const string KORISNIK_NE_POSTOJI = "Korisnik ne postoji.";
        public const string KORISNIK_AZURIRAN = "Korisnik uspješno ažuriran.";
        public const string KORISNIK_OBRISAN = "Korisnik uspješno obrisan.";
        public const string KORISNIK_NEOVLASTENO_BRISANJE = "Nije moguće obrisati tuđi profil.";
        public const string KORISNIK_NEOVLASTENI_PRISTUP_ODJAVLJEN_KORISNIK = "Morate biti ulogirati za pristup podacima.";
        public const string KORISNIK_NEOVLASTENI_PRISTUP_NEDOVOLJNA_RAZINA_PRAVA= "Nemate dovoljnu razinu prava.";
        public const string KORISNIK_ULOGIRAN_NIJE_MOGUCE_KREIRATI_RACUN= "Nije moguće kreirati novi račun. Prvo se odjavite pa pokušajte ponovno.";
    }
}

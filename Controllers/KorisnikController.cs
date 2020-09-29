using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Planny.AutoMapper;
using Planny.Data.Models;
using Planny.Data.ViewModels;
using Planny.DataContext;

namespace Planny.Controllers
{
    //[EnableCors("PolicaZaTestiranje")]
    [Route("api/[controller]")]
    [ApiController]
    public class KorisnikController : ControllerBase
    {
        private readonly PlannyContext _context;
        private readonly IMapper _mapper;

        public KorisnikController(PlannyContext context)
        {
            AutoMapperHelper autoMapperHelper = new AutoMapperHelper();
            _mapper = autoMapperHelper.IMapper;
            _context = context;
        }

        [HttpGet]
        [Route("korisnici")]
        public async Task<IActionResult> GetAllUsers()
        {
            if (!IsUserLoggedIn())
            {
                UnauthorizedAccess();
            }

            List<Korisnik> korisnici = await _context.Korisnik.Where(k => k.Active && k.JeJavniProfil).ToListAsync();
            List<KorisnikDTO> korisniciDTO = new List<KorisnikDTO>();

            foreach (var item in korisnici)
                korisniciDTO.Add(_mapper.Map<Korisnik, KorisnikDTO>(item));

            return StatusCode(StatusCodes.Status200OK, korisniciDTO);
        }

        [HttpGet]
        [Route("dohvatiKorisnika")]
        public async Task<IActionResult> GetUser(string userName)
        {
            if (!IsUserLoggedIn())
            {
                UnauthorizedAccess();
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                Korisnik korisnik;
                if (userName == GetCurrentLoggedInUser())
                {
                    korisnik = await _context.Korisnik.FirstOrDefaultAsync(x => x.KorisnickoIme == userName && x.Active);
                }
                else
                {
                    korisnik = await _context.Korisnik.FirstOrDefaultAsync(x => x.KorisnickoIme == userName && x.Active && x.JeJavniProfil);
                }

                if (korisnik == null)
                    return StatusCode(StatusCodes.Status404NotFound, Constants.Constants.KORISNIK_NE_POSTOJI);

                return StatusCode(StatusCodes.Status200OK, _mapper.Map<Korisnik, KorisnikDTO>(korisnik));
            }

            return StatusCode(StatusCodes.Status404NotFound, Constants.Constants.KORISNIK_NE_POSTOJI);
        }

        [HttpPost]
        [Route("novikorisnik")]
        public async Task<IActionResult> InsertUser(KorisnikDTO korisnikDTO)
        {
            if (IsUserLoggedIn())
            {
                return StatusCode(StatusCodes.Status403Forbidden, Constants.Constants.KORISNIK_ULOGIRAN_NIJE_MOGUCE_KREIRATI_RACUN);
            }

            try
            {
                if (IsUserNameTaken(korisnikDTO.KorisnickoIme).Result)
                    return BadRequest(Constants.Constants.KORISNIK_ZAUZETO_KORISNICKO_IME);
            }
            catch (Exception ex)
            {
                // mozda ne bi trebao exposati detaljni error korisnku, nego samo genericki error, a detaljni error logiras kod sebe negdje...
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message + "\n" + Constants.Constants.GENERICKA_GRESKA);
            }

            byte[] saltAsByteArray = EncryptionHelper.PasswordHashing.CreateSalt();

            try
            {
                while (IsSaltTaken(Convert.ToBase64String(saltAsByteArray)).Result)
                {
                    saltAsByteArray = EncryptionHelper.PasswordHashing.CreateSalt();
                }
            }
            catch (Exception ex)
            {
                // mozda ne bi trebao exposati detaljni error korisnku, nego samo genericki error, a detaljni error logiras kod sebe negdje...
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message + "\n" + Constants.Constants.GENERICKA_GRESKA);
            }

            string saltAsBase64 = Convert.ToBase64String(saltAsByteArray);

            byte[] passwordHashAsByteArray = EncryptionHelper.PasswordHashing.HashPassword(korisnikDTO.UnesenaLozinka, saltAsByteArray);
            string passwordHashAsBase64 = Convert.ToBase64String(passwordHashAsByteArray);

            korisnikDTO.Salt = saltAsBase64;
            korisnikDTO.LozinkaHash = passwordHashAsBase64;
            korisnikDTO.DatumRegistracije = DateTime.Now;
            korisnikDTO.Id = Guid.NewGuid();
            korisnikDTO.Active = true;

            var korisnik = _mapper.Map<KorisnikDTO, Korisnik>(korisnikDTO);

            _context.Entry(korisnik).State = EntityState.Added; // ovo radi što točno? što točno radi Added u EF-coru?

            try
            {
                await _context.SaveChangesAsync(); // save-a samo ili i updejta bazu? Saznaj i zapiši.... 
            }
            catch (Exception ex)
            {
                // mozda ne bi trebao exposati detaljni error korisnku, nego samo genericki error, a detaljni error logiras kod sebe negdje...
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message + "\n" + Constants.Constants.GENERICKA_GRESKA);
            }

            return StatusCode(StatusCodes.Status201Created, "Korisnik uspješno kreiran.");
        }

        [HttpPost]
        [Route("AzurirajKorisnika")]
        public async Task<IActionResult> UpdateUser(KorisnikDTO korisnikDTO)
        {
            if (!IsUserLoggedIn())
            {
                return UnauthorizedAccess();
            }

            if (korisnikDTO.KorisnickoIme != GetCurrentLoggedInUser())
            {
                return Forbidden();
            }

            var originalKorisnik = await _context.Korisnik.FirstOrDefaultAsync(x => x.KorisnickoIme == korisnikDTO.KorisnickoIme);

            if (originalKorisnik == null)
                return StatusCode(StatusCodes.Status404NotFound, Constants.Constants.KORISNIK_NE_POSTOJI);

            originalKorisnik.DatumRodenja = korisnikDTO.DatumRodenja;
            originalKorisnik.Ime = korisnikDTO.Ime;
            originalKorisnik.Prezime = korisnikDTO.Prezime;
            
            // naknadno stavi da se password moze mijenjati...
            _context.Korisnik.Update(originalKorisnik);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // mozda ne bi trebao exposati detaljni error korisnku, nego samo genericki error, a detaljni error logiras kod sebe negdje...
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message + "\n" + Constants.Constants.GENERICKA_GRESKA);
            }

            return StatusCode(StatusCodes.Status200OK, Constants.Constants.KORISNIK_AZURIRAN);
        }

        [HttpDelete]
        [Route("ObrisiKorisnika")]
        public async Task<IActionResult> DeleteUser(string userName)
        {
            if (!IsUserLoggedIn())
            {
                return UnauthorizedAccess();
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                Korisnik korisnik;
                if (userName == GetCurrentLoggedInUser())
                {
                    korisnik = await _context.Korisnik.FirstOrDefaultAsync(x => x.KorisnickoIme == userName && x.Active);

                    if (korisnik == null)
                        return StatusCode(StatusCodes.Status404NotFound, Constants.Constants.KORISNIK_NE_POSTOJI);

                    korisnik.Active = false;
                    _context.Korisnik.Update(korisnik);
                    //_context.Korisnik.Remove(korisnik);

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message + "\n" + Constants.Constants.GENERICKA_GRESKA);
                    }

                    return StatusCode(StatusCodes.Status200OK, Constants.Constants.KORISNIK_OBRISAN);
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized, Constants.Constants.KORISNIK_NEOVLASTENO_BRISANJE);
                }
            }

            return StatusCode(StatusCodes.Status404NotFound, Constants.Constants.KORISNIK_NE_POSTOJI);
        }

        private async Task<bool> IsUserNameTaken(string username)
        {
            return await _context.Korisnik.AnyAsync(x => x.KorisnickoIme == username);
        }

        private async Task<bool> IsSaltTaken(string salt)
        {
            return await _context.Korisnik.AnyAsync(x => x.Salt == salt);
        }

        private string GetCurrentLoggedInUser()
        {
            return "userName";
        }

        private IActionResult UnauthorizedAccess()
        {
            return StatusCode(StatusCodes.Status401Unauthorized, Constants.Constants.KORISNIK_NEOVLASTENI_PRISTUP_ODJAVLJEN_KORISNIK);
        }

        private IActionResult Forbidden()
        {
            return StatusCode(StatusCodes.Status403Forbidden, Constants.Constants.KORISNIK_NEOVLASTENI_PRISTUP_NEDOVOLJNA_RAZINA_PRAVA);
        }

        private bool IsUserLoggedIn()
        {
            return string.IsNullOrWhiteSpace(GetCurrentLoggedInUser());
        }
    }
}

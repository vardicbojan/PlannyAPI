//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using AutoMapper;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Planny.AutoMapper;
//using Planny.Data.Models;
//using Planny.DataContext;
//using Planny.EncryptionHelper;
//using Planny.Data.ViewModels;

//namespace Planny.Controllers
//{
//    [Route("api/[controller]/[action]")]
//    [ApiController]
//    public class KorisnikBadController : ControllerBase
//    {
//        private readonly PlannyContext _context;
//        private readonly IMapper iMapper;

//        public KorisnikBadController(PlannyContext context)
//        {
//            _context = context;
//            AutoMapperHelper autoMapperHelper = new AutoMapperHelper();
//            iMapper = autoMapperHelper.IMapper;
//        }

//        // GET: api/Korisnik
//        // svi korisnici
//        [HttpGet]
//        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        public async Task<ActionResult<IEnumerable<KorisnikVM>>> Korisnici()
//        {
//            var listaKorisnika = await _context.Korisnik.ToListAsync();
//            var listaKorisnikaVM= new List<KorisnikVM>();


//            foreach (var korisnik in listaKorisnika)
//            {
//                listaKorisnikaVM.Add(iMapper.Map<Korisnik, KorisnikVM>(korisnik));
//            }

//            return Ok(listaKorisnikaVM);
//        }

//        // GET: api/Korisnik/5
//        // specificni korisnik
//        [HttpGet("{id}")]
//        public async Task<ActionResult<KorisnikVM>> Korisnik(Guid id)
//        {
//            var korisnik = await _context.Korisnik.FindAsync(id);

//            if (korisnik == null)
//                return NotFound();

//            var korisnikDTO = iMapper.Map<Korisnik, KorisnikVM>(korisnik);

//            return korisnikDTO;
//        }

//        // PUT: api/Korisnik/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for
//        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
//        // azuriranje korisnika
//        [HttpPut("{id}")]
//        public async Task<IActionResult> AzurirajKorisnika(Guid id, KorisnikVM korisnikVM)
//        {
//            if (id != korisnikVM.Id)
//                return BadRequest();

//            if (!ValidirajNovogKorisnika(korisnikVM))
//                return new ObjectResult(korisnikVM.Greska);

//            var korisnik = iMapper.Map<KorisnikVM, Korisnik>(korisnikVM);

//            _context.Entry(korisnik).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!KorisnikPostoji(id))
//                    return NotFound();
//                else
//                    throw;
//            }

//            return NoContent();
//        }

//        // POST: api/Korisnik
//        // To protect from overposting attacks, enable the specific properties you want to bind to, for
//        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754
//        // novi korisnik
//        [HttpPost]
//        public async Task<ActionResult> NoviKorisnik(KorisnikVM korisnikVM)
//        {
//            korisnikVM.Greska = new KorisnikError();
//            if (!korisnikVM.IsPasswordOK())
//                return new ObjectResult(korisnikVM.Greska);

//            Korisnik noviKorisnik;

//            var passwordHashing = new PasswordHashing();
//            korisnikVM.Salt = Encoding.UTF8.GetString(passwordHashing.CreateSalt());
//            korisnikVM.LozinkaHash = Encoding.UTF8.GetString(passwordHashing.HashPassword(korisnikVM.UnesenaLozinka, passwordHashing.CreateSalt()));
//            // provjeri ovdje generira li se ID korisnika

//            noviKorisnik = iMapper.Map<KorisnikVM, Korisnik>(korisnikVM);

//            noviKorisnik.Id = Guid.NewGuid();
//            noviKorisnik.DatumRegistracije = DateTime.Now;

//            if (ValidirajNovogKorisnika(korisnikVM))
//                _context.Korisnik.Add(noviKorisnik);
//            else
//                return new ObjectResult(korisnikVM.Greska);

//            await _context.SaveChangesAsync();
//            return CreatedAtAction("Korisnik", new { id = noviKorisnik.Id }, noviKorisnik);
//        }

//        // DELETE: api/Korisnik/5
//        // brisanje korisnika
//        [HttpDelete("{id}")]
//        public async Task<ActionResult<Korisnik>> ObrisiKorisnika(Guid id)
//        {
//            Korisnik korisnikZaObrisati;
//            var korisnikZaObrisatiVM = await Korisnik(id);

//            if (korisnikZaObrisatiVM.Result == new NotFoundResult())
//                return NotFound();
//            else
//                korisnikZaObrisati = iMapper.Map<KorisnikVM, Korisnik>(korisnikZaObrisatiVM.Value);   

//            _context.Korisnik.Remove(korisnikZaObrisati);
//            await _context.SaveChangesAsync();

//            return new OkResult();
//            //return korisnikZaObrisati;
//        }

//        // korisnik postoji?
//        private bool KorisnikPostoji(Guid id)
//        {
//            return _context.Korisnik.Any(e => e.Id == id);
//        }

//        private bool ValidirajNovogKorisnika(KorisnikVM korisnikVM)
//        {
//            if (korisnikVM != null)
//            {
//                if (!korisnikVM.IsPasswordOK())
//                    return false;

//                bool jedinstvenKorisnik = _context.Korisnik.Count(x => x.Id == korisnikVM.Id) == 0;
//                if (!jedinstvenKorisnik)
//                    return false;

//                if (!KorisnickoImeJeJedinstveno(korisnikVM.KorisnickoIme))
//                {
//                    korisnikVM.Greska.ErrorMessage = Constants.Constants.ZAUZETO_KORISNICKO_IME;
//                    return false;
//                }

//                if (!SaltJeJedinstven(korisnikVM.Salt))
//                {
//                    korisnikVM.Greska.ErrorMessage = "";
//                    return false;
//                }

//                if (!HashJeJedinstven(korisnikVM.LozinkaHash))
//                {
//                    korisnikVM.Greska.ErrorMessage = "";
//                    return false;
//                }
//            }
//            else
//            {
//                return false;
//            }

//            return true;
//        }

//        private bool KorisnickoImeJeJedinstveno(string korisnickoIme)
//        {
//            if (!string.IsNullOrWhiteSpace(korisnickoIme))
//                return _context.Korisnik.Count(x => x.KorisnickoIme == korisnickoIme) == 0;
//            else
//                return false;
//        }

//        private bool SaltJeJedinstven(string salt)
//        {
//            if (!string.IsNullOrWhiteSpace(salt))
//                return _context.Korisnik.Count(x => x.Salt == salt) == 0;
//            else
//                return false;
//        }

//        private bool HashJeJedinstven(string hash)
//        {
//            if (!string.IsNullOrWhiteSpace(hash))
//                return _context.Korisnik.Count(x => x.LozinkaHash == hash) == 0;
//            else
//                return false;
//        }

//        //private KorisnikDTO MakniOsjetljivePodatkeIzKorisnika(KorisnikDTO korisnikDTO)
//        //{
//        //    if (korisnikDTO != null)
//        //    {
//        //        korisnikDTO.LozinkaHash = null;
//        //        korisnikDTO.Salt = null;
//        //    }
//        //    else
//        //    {
//        //        korisnikDTO.
//        //    }
//        //}


//    }
//}

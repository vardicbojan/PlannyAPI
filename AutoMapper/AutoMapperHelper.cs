using AutoMapper;
using Planny.Data.Models;
using Planny.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planny.AutoMapper
{
    public class AutoMapperHelper
    {
        internal MapperConfiguration Configuration
        {
            get 
            { 
                return new MapperConfiguration(cfg => {
                    cfg.CreateMap<Korisnik, KorisnikDTO>();
                    cfg.CreateMap<KorisnikDTO, Korisnik>();
                    //cfg.CreateMap<StatusZadatka, StatusZadatkaVM>();
                    //cfg.CreateMap<Zadatak, ZadatakVM>();
                });
            }
        }

        internal IMapper IMapper { get { return Configuration.CreateMapper(); } }
    }
}

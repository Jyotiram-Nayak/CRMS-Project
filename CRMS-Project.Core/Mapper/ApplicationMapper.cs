using AutoMapper;
using CRMS_Project.Core.Domain.Identity;
using CRMS_Project.Core.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMS_Project.Core.Mapper
{
    public class ApplicationMapper : Profile
    {
        public ApplicationMapper()
        {
            //CreateMap<AuthenticationResponse, ApplicationUser>();
            CreateMap<ApplicationUser,AuthenticationResponse>();
        }
    }
}

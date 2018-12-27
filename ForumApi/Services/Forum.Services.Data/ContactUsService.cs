using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects.InputModels.ContactUs;
using Forum.Data.Models;
using Forum.Data.Models.Users;
using Forum.Services.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Forum.Services.Data
{
    public class ContactUsService : BaseService, IContactUsService
    {
        private readonly IRepository<ContactUs> contractUsRepository;

        public ContactUsService(IRepository<ContactUs> contractUsRepository, UserManager<User> userManager, ILogger<BaseService> logger, IMapper mapper) : base(userManager, logger, mapper)
        {
            this.contractUsRepository = contractUsRepository;
        }

        public async Task Create(ContactUsInputModel model)
        {
            var entity = this.Mapper.Map<ContactUs>(model);

            await this.contractUsRepository.AddAsync(entity);
            await this.contractUsRepository.SaveChangesAsync();
        }
    }
}

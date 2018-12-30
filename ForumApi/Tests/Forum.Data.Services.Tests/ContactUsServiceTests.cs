using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Forum.Data.Common;
using Forum.Data.Common.Interfaces;
using Forum.Data.DataTransferObjects;
using Forum.Data.DataTransferObjects.InputModels.ContactUs;
using Forum.Data.Models;
using Forum.Data.Models.Users;
using Forum.Data.Services.Tests.Fake;
using Forum.Services.Data;
using Forum.Services.Data.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Forum.Data.Services.Tests
{
    public class ContactUsServiceTests
    {
        private readonly IContactUsService contactUsService;
        private readonly IRepository<ContactUs> contactUsRepository;

        public ContactUsServiceTests()
        {
            var guid = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<ForumContext>()
                .UseInMemoryDatabase(guid).Options;
            var context = new ForumContext(options);
            this.contactUsRepository = new Repository<ContactUs>(context);

            var userStore = new UserStore<User>(context);
            var logger = new Mock<ILogger<ContactUsService>>();

            var mapperProfile = new MapInitialization();
            var conf = new MapperConfiguration(cfg => cfg.AddProfile(mapperProfile));
            var mapper = new Mapper(conf);

            var fakeUserManager = new FakeUserManager(userStore);

            this.contactUsService = new ContactUsService(contactUsRepository, fakeUserManager, logger.Object, mapper);
        }

        [Fact]
        public async Task ShouldReturnEmptyCollection()
        {
            var all = await this.contactUsRepository.Query().ToListAsync();
            Assert.Empty(all);
        }

        [Theory]
        [InlineData("test@test.com", "test subject", "test description")]
        public async Task ShouldCreateFormSuccessfully(string email, string subject, string description)
        {
            await this.contactUsService.Create(new ContactUsInputModel
                {Email = email, Subject = subject, Description = description});

            var count = await this.contactUsRepository.Query().ToListAsync();

            Assert.Single(count);
        }
    }
}

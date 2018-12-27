using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.ContactUs;
using Forum.WebApi.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xunit;

namespace Forum.Web.Tests
{
    public class ContactUsControllerTests : BaseControllerTest
    {
        private const string ContactUsSuccessMessage = "Thank you for reaching us. We will contact you soon.";
        private const string ContactUsEndpoint = "api/contact-us";

        public ContactUsControllerTests(TestingWebApplicationFactory factory) : base(factory)
        {
        }

        [Theory]
        [InlineData("test@test.com", "Testing Subject Information", "Testing Description Information")]
        public async Task ContactUsSuccessful(string email, string subject, string description)
        {
            var form = new ContactUsInputModel
            {
                Email = email,
                Subject = subject,
                Description = description
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(form),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(ContactUsEndpoint, json);

            response.EnsureSuccessStatusCode();

            var content = JsonConvert.DeserializeObject<ReturnMessage>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status200OK, content.Status);
            Assert.Equal(ContactUsSuccessMessage, content.Message);
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData("test", "12341234", "test")]
        [InlineData("test@test.com", "123", "test")]
        [InlineData("test@test.com", "12341234", "t")]
        [InlineData("test@test.com", "12341234", "test@")]
        public async Task ContactUsFailWithInvalidModelData(string email, string subject, string description)
        {
            var form = new ContactUsInputModel
            {
                Email = email,
                Subject = subject,
                Description = description
            };

            var json = new StringContent(
                JsonConvert.SerializeObject(form),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(ContactUsEndpoint, json);

            var content = JsonConvert.DeserializeObject<ProblemDetails>(await response.Content.ReadAsStringAsync());

            Assert.Equal(StatusCodes.Status400BadRequest, content.Status);
            Assert.Equal(ValidationMessage, content.Title);
        }

    }
}

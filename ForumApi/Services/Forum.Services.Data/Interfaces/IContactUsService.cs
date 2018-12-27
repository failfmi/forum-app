using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.InputModels.ContactUs;

namespace Forum.Services.Data.Interfaces
{
    public interface IContactUsService
    {
        Task Create(ContactUsInputModel model);
    }
}

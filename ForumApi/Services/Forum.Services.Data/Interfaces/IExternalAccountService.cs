using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.ViewModels.ExternalAuth;

namespace Forum.Services.Data.Interfaces
{
    public interface IExternalAccountService
    {
        Task<string> FacebookLogin(FacebookLoginModel model);
    }
}

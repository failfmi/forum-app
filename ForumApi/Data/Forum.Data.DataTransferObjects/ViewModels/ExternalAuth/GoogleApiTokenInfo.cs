using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.Data.DataTransferObjects.ViewModels.ExternalAuth
{
    public class GoogleApiTokenInfo
    {
        /// <summary>
        /// The user's email address. This may not be unique and is not suitable for use as a primary key. Provided only if your scope included the string "email".
        /// </summary>
        public string email { get; set; }
    }
}

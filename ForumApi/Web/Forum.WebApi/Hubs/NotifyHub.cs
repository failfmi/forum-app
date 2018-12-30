using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.Data.DataTransferObjects.ViewModels.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Forum.WebApi.Hubs
{
    public class NotifyHub : Hub
    {
        public async Task Send(PostViewModel model)
        {
            await this.Clients
                .All
                .SendAsync("PostAdd", model);
        }
    }
}

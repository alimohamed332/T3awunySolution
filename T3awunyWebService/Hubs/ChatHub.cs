using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using T3awuny.Application.Contracts;

namespace T3awunyWebService.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        
    }
}

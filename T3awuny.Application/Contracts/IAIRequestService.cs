using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.Contracts
{
    public interface IAIRequestService
    {
        Task<string> SenChatBot(string message);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T3awuny.Application.Contracts
{
    public interface IEmailService 
    {
        Task SendAsync(string toEmail, string subject, string body);
    }
}

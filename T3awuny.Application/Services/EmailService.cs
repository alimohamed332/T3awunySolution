using FluentEmail.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T3awuny.Application.Contracts;

namespace T3awuny.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;

        public EmailService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            await _fluentEmail.To(toEmail)
                .Subject(subject)
                .Body(body,true)
                .SendAsync();
        }
    }
}

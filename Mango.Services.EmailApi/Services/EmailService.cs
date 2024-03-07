using Azure.Core.Amqp;
using Mango.Services.EmailApi.Data;
using Mango.Services.EmailApi.Message;
using Mango.Services.EmailApi.Models;
using Mango.Services.EmailApi.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Mango.Services.EmailApi.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public EmailService(DbContextOptions<AppDbContext> options)
        {
            this._dbOptions = options;
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<br/>Email cart received");
            sb.Append($"<br/> {cartDto.CartHeader.CartTotal}");
            sb.Append("<ul>");
            foreach (var item in cartDto.CartDetails)
            {
                sb.Append($"<li> {item.Product.Name} x {item.Count} </li>");
            }
            sb.Append("</ul>");

            await LogAndEmail(sb.ToString(), cartDto.CartHeader.Email);
        }

        public async Task LogOrderPlaced(RewardMessage rewardsDto)
        {
            string message = "New Order palced. <br/> Order ID: " + rewardsDto.OrderId;
            await LogAndEmail(message, "martinayush04@gmail.com");
        }

        public async Task RegisterUserAndLog(string email)
        {
            StringBuilder sb = new();
            sb.Append($"<br/>A new user has been registered {email}");

            await LogAndEmail(sb.ToString(), "martinayush04@gmail.com");
        }

        private async Task<bool> LogAndEmail(string message, string emailTo)
        {
            try
            {
                EmailLogger emailLogger = new()
                {
                    Email = emailTo,
                    Message = message,
                    EmailSent = DateTime.Now,
                };

                await using var _db = new AppDbContext(_dbOptions);
                await _db.EmailLoggers.AddAsync(emailLogger);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

using System.Threading.Tasks;

namespace ZO1.Identity.WebApp.Services
{
    public interface IEmailServices
    {
        Task SendAsync(string from, string to, string subject, string body);
    }
}
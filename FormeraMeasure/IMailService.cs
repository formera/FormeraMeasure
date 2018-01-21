using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormeraMeasure
{

    public interface IMailService
    {
        Task<bool> SendAsync(string from, string to, string subject, string message);
    }

}

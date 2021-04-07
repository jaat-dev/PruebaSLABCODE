using Projects.Api.Models;
using Projects.Api.Models.Responses;

namespace Projects.Api.Helpers
{
    public interface IMailHelper
    {
        Response SendMail(EmailMessageModel emailMessage);
    }
}

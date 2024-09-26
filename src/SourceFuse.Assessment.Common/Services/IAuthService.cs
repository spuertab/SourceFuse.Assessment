using SourceFuse.Assessment.Common.Models;

namespace SourceFuse.Assessment.Common.Services
{
    public interface IAuthService
    {
        string Login(LoginReqModel model);
    }
}

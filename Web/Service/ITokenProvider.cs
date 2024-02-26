using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserApp.Web.Service
{
    public interface ITokenProvider
    {
        void SetToken(string token, bool rememberMe);
        string GetToken();
        void ClearToken();
    }
}

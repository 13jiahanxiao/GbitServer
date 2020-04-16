using Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    class BaseController
    {
        protected RequestCode request = RequestCode.None;
        public RequestCode RequestCode
        {
            get { return request; }
        }

        public virtual string DefaultHandle(string data, Client client, Server server)
        {
            return null;
        }
    }
}

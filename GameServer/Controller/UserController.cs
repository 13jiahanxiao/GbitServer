using GameServer.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;
using GameServer.Model;

namespace GameServer.Controller
{
    class UserController : BaseController
    {
        private UserDAO userDAO = new UserDAO();
        public UserController()
        {
            request = RequestCode.User;
        }
        public string Login(string data, Client client, Server server)
        {
            string[] strs = data.Split(',');
            User user = userDAO.VerifyUser(client.MySqlConnect, strs[0], strs[1]);
            if (user == null)
            {
                return ((int)ReturnCode.Failed).ToString();
            }
            else
            {
                client.SetUserData(user);
                return string.Format("{0},{1},{2}", ((int)ReturnCode.Success).ToString(), user.Id,user.Username);
            }
        }
        public string Register(string data, Client client, Server server)
        {
            string[] strs = data.Split(',');
            string username = strs[0]; string password = strs[1];
            bool res = userDAO.GetUserByUsername(client.MySqlConnect, username);
            if (res)
            {
                return ((int)ReturnCode.Failed).ToString();
            }
            userDAO.AddUser(client.MySqlConnect, username, password);
            return ((int)ReturnCode.Success).ToString();
        }
    }
}

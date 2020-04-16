using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    class RoomController:BaseController
    {
        public RoomController()
        {
            request = RequestCode.Room;
        }
        public string CreateRoom(string data, Client client, Server server)
        {
            server.CreateRoom(client);
            return ((int)ReturnCode.Success).ToString();
        }
        public string ReadRoomList(string data, Client client, Server server)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Room room in server.GetRoomList())
            {
                if (room.IsWaitingJoin())
                {
                    sb.Append(room.GetHouseOwnerData() + ",");
                }
            }
            if (sb.Length == 0)
            {
                sb.Append(((int)ReturnCode.Failed).ToString());
            }
            else
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }
        public string JoinRoom(string data, Client client, Server server)
        {
            int id = int.Parse(data);
            Room room = server.GetRoomById(id);
            if (room == null)
            {
                return ((int)ReturnCode.Failed).ToString();
            }
            else if (room.IsWaitingJoin() == false)
            {
                return ((int)ReturnCode.Full).ToString();
            }
            else
            {
                room.AddClient(client);
                room.BroadcastMessage(client, ActionCode.UpdateRoom, room.GetRoomUserData());
                return ((int)ReturnCode.Success).ToString() + "," + room.GetRoomUserData();
            }
        }
        public string CloseRoom(string data, Client client, Server server)
        {
            bool isHouseOwner = client.IsHouseOwner();
            Room room = client.Room;
            if (isHouseOwner)
            {
                room.BroadcastMessage(client, ActionCode.CloseRoom, ((int)ReturnCode.Success).ToString());
                room.Close();
                return ((int)ReturnCode.Success).ToString();
            }
            else
            {
                client.Room.RemoveClient(client);
                room.BroadcastMessage(client, ActionCode.UpdateRoom, room.GetRoomUserData());
                return ((int)ReturnCode.Success).ToString();
            }
        }
    }
}

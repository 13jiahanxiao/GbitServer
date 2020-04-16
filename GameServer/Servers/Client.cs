using System;
using System.Net.Sockets;
using MySql.Data.MySqlClient;
using GameServer.Tool;
using Common;
using GameServer.Model;

namespace GameServer.Servers
{
    class Client
    {
        private Socket clientSocket;
        private Server server;
        private Message msg = new Message();
        private MySqlConnection mySqlConnection;
        private Room room;
        private User user;

        public MySqlConnection MySqlConnect
        {
            get { return mySqlConnection; }
        }
        public void SetUserData(User user)
        {
            this.user = user;
        }
        public string GetUserData()
        {
            return user.Id + "," + user.Username ;
        }
        public Room Room
        {
            set { room = value; }
            get { return room; }
        }
        public int GetUserId()
        {
            return user.Id;
        }

        public Client() { }
        public Client(Socket clientSocket, Server server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
            mySqlConnection = MySqlConnectTool.Connect();
        }
        public void Start()
        {
            if (clientSocket == null || clientSocket.Connected == false) return;
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (clientSocket == null || clientSocket.Connected == false) return;
                int count = clientSocket.EndReceive(ar);
                if (count == 0)
                {
                    Close();
                }
                msg.ReadMessage(count, OnProcessMessage);
                Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Close();
            }
        }
        private void OnProcessMessage(RequestCode requestCode, ActionCode actionCode, string data)
        {
            Console.WriteLine("收到消息" + "," +requestCode.ToString()+","+ actionCode.ToString() + "," + data);
            server.HandleRequest(requestCode, actionCode, data, this);
        }
        private void Close()
        {
            MySqlConnectTool.CloseConnect(mySqlConnection);
            if (clientSocket != null)
                clientSocket.Close();
            if (room != null)
            {
                room.QuitRoom(this);
            }
            server.RemoveClient(this);
        }
        public void Send(ActionCode actionCode, string data)
        {
            try
            {
                Console.WriteLine( "发出消息"+","+actionCode.ToString() + "," + data);
                byte[] bytes = Message.PackData(actionCode, data);
                clientSocket.Send(bytes);
            }
            catch (Exception e)
            {
                Console.WriteLine("无法发送消息:" + e);
            }
        }
        public bool IsHouseOwner()
        {
            return room.IsHouseOwner(this);
        }
    }
}

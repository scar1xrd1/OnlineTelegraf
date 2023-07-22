using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCF_Morze
{
    [ServiceBehavior (InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceMorze : IServiceMorze
    {
        List<ServerUser> users = new List<ServerUser>();
        int nextId = 1;

        public int Connect(string username)
        {
            ServerUser user = new ServerUser()
            {
                ID = nextId++,
                Name = username,
                operationContext = OperationContext.Current
            };

            SendMsg($"{user.Name} подключился к телеграфу!", 0);
            users.Add(user);

            return user.ID;
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(u => u.ID == id);

            if(user != null)
            {
                SendMsg($"{user.Name} покинул телеграф!", 0);
                users.Remove(user);               
            }
        }

        public void SendMsg(string msg, int senderID)
        {
            foreach(var item in users)
            {
                string answer = DateTime.Now.ToShortTimeString();
                var user = users.FirstOrDefault(u => u.ID == senderID);

                if (user != null) answer += $" | {user.Name}: ";
                answer += msg;

                item.operationContext.GetCallbackChannel<IServerMorzeCallback>().MsgCallback(answer, user.ID);
            }
        }

        public void SendSignal(int tick, int senderID)
        {
            
        }
    }
}

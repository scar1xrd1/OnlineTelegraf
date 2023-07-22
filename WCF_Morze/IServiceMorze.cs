using System.ServiceModel;

namespace WCF_Morze
{
    [ServiceContract (CallbackContract = typeof(IServerMorzeCallback))]
    public interface IServiceMorze
    {
        [OperationContract]
        int Connect(string username);

        [OperationContract]
        void Disconnect(int id);

        [OperationContract(IsOneWay = true)]
        void SendSignal(int tick, int senderID);

        [OperationContract(IsOneWay = true)]
        void SendMsg(string msg, int senderID);
    }

    public interface IServerMorzeCallback
    {
        [OperationContract(IsOneWay = true)]
        void SignalCallback(int tick, int senderID);
        
        [OperationContract(IsOneWay = true)]
        void MsgCallback(string msg, int senderID);
    }
}

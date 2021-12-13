using System.Net;
using UnityEngine;

namespace Playground
{
    [CreateAssetMenu(fileName = "ServerSettings", menuName = "Config/ServerSettings")]
    public class ServerSettings : ScriptableObject
    {
        [SerializeField] private string _address;
        [SerializeField] private int _port;

        public IPAddress Address => IPAddress.Parse(_address);
        public int Port => _port;
        public IPEndPoint EndPoint => new(Address, Port);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Nodes {
    public abstract class SWCommunicationNode :Node{
        public abstract void Trasmit(Packet packet);
    }
}

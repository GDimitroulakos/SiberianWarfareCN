using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Graph {

    
    public interface INodeMetadataPrinter {
        string ToString(INode node);
    }

    public class BaseNodeMetadataGraphvizPrinter : INodeMetadataPrinter{
    
        INodeMetadataPrinter _parent;

        public BaseNodeMetadataGraphvizPrinter(INodeMetadataPrinter parent) {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent), "Parent printer cannot be null.");
        }

        public string ToString(INode node) {
            if (_parent != null) {
                return _parent.ToString(node);
            }
            else {
                throw new NotImplementedException("No parent printer set.");
            }
        }

        public class DFSDirectedMetadataGraphVizPrinter : BaseNodeMetadataGraphvizPrinter{



        }
    }
    

}

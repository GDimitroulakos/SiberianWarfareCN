using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Graph {
    public interface IEdgeMetadataGraphvizPrinter {
        string ToString(IEdge edge);
    } 

    public class DFSGraphvizEdgePrinter : IEdgeMetadataGraphvizPrinter {
        public DFSGraphvizEdgePrinter( ) {
        }

        public string ToString(IEdge edge) {
            var s=DFSGraphvizNodePrinter.NodeID(edge.Source);
            var t = DFSGraphvizNodePrinter.NodeID(edge.Target);
            return $"\"{s}\" -> \"{t}\";";
        }
    }

    public class BaseEdgeMetadataGraphvizPrinter : IEdgeMetadataGraphvizPrinter {
        protected IEdgeMetadataGraphvizPrinter _wrappee;
        private bool isOuterDecorator = true;
        public bool IsOuterDecorator => isOuterDecorator;
        public bool IsInnerDecorator => _wrappee is not BaseNodeMetadataGraphvizPrinter;

        public BaseEdgeMetadataGraphvizPrinter(IEdgeMetadataGraphvizPrinter wrappee) {
            _wrappee = wrappee ?? throw new ArgumentNullException(nameof(wrappee), "Parent printer cannot be null.");
            if (wrappee is BaseEdgeMetadataGraphvizPrinter wr) {
                wr.IsWrapped();
            }
        }

        public virtual void IsWrapped() {
            isOuterDecorator = false;
        }

        public virtual string ToString(IEdge edge) {
            return _wrappee.ToString(edge);
        }
    }




}

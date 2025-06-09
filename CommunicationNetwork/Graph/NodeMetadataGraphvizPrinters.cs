using CommunicationNetwork.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Graph {

    public interface INodeMetadataGraphvizPrinter {
        string ToString(INode node);
    }

    public class DFSGraphvizNodePrinter : INodeMetadataGraphvizPrinter {
        public DFSGraphvizNodePrinter() {
        }

        public string ToString(INode node) {
            return $"\"{NodeID(node)}\" ";
        }

        public static string NodeID(INode node) {
            return node.Serial.ToString();
        }
    }

    public class BaseNodeMetadataGraphvizPrinter : INodeMetadataGraphvizPrinter{
    
        protected INodeMetadataGraphvizPrinter _wrappee;
        private bool isOuterDecorator=true;
        public bool IsOuterDecorator => isOuterDecorator;
        public bool IsInnerDecorator => _wrappee is not BaseNodeMetadataGraphvizPrinter;

        public BaseNodeMetadataGraphvizPrinter(INodeMetadataGraphvizPrinter wrappee) {
            _wrappee = wrappee ?? throw new ArgumentNullException(nameof(wrappee), "Parent printer cannot be null.");
           if (wrappee is BaseNodeMetadataGraphvizPrinter wr) {
                wr.IsWrapped();
            }
        }

        public virtual void IsWrapped() {
            isOuterDecorator = false;
        }

        public virtual string ToString(INode node) {
            return _wrappee.ToString(node);
        }
    }

    public class DFSDirectedGraphvizFixedSizePropertyPrinter : BaseNodeMetadataGraphvizPrinter {
        public DFSDirectedGraphvizFixedSizePropertyPrinter(INodeMetadataGraphvizPrinter wrappee) : base(wrappee) {
        }

        public override string ToString(INode node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            StringBuilder sb = new StringBuilder();

            sb.Append(_wrappee.ToString(node));

            if (IsInnerDecorator) {
                sb.Append($"[ fixedsize=false ");
            } else {
                sb.Append($", fixedsize=false ");
            }

            if (IsOuterDecorator) {
                sb.AppendLine("];");
            }
            return sb.ToString();
        }
    }

    public class DFSDirectedGraphVizNodeLabelPrinter : BaseNodeMetadataGraphvizPrinter {
        public DFSDirectedGraphVizNodeLabelPrinter(INodeMetadataGraphvizPrinter wrappee) : base(wrappee) {
        }

        public override string ToString(INode node) {
            if (node == null) throw new ArgumentNullException(nameof(node));
            StringBuilder sb = new StringBuilder();

            sb.Append(_wrappee.ToString(node));

            string nodeLabel = node.Name ?? node.Serial.ToString();
            var TimeDiscovered = node.MetaData.ContainsKey("DFSDirected")
                ? DFSDirected.TimeDiscovered(node).ToString()
                : "N/A";
            var TimeFinished = node.MetaData.ContainsKey("DFSDirected")
                ? DFSDirected.TimeFinished(node).ToString()
                : "N/A";

            if (IsInnerDecorator) {
                sb.Append($"[ label=\"{Escape(nodeLabel)} \\nTD:{TimeDiscovered} \\nTF:{TimeFinished}\"");
            } else {
                sb.Append($", label=\"{Escape(nodeLabel)} \\nTD:{TimeDiscovered} \\nTF:{TimeFinished}\"");
            }

            if (IsOuterDecorator) {
                sb.AppendLine("];");
            }
            return sb.ToString();
        }
        private string Escape(string label) {
            return label?.Replace("\"", "\\\"") ?? string.Empty;
        }
    }


}

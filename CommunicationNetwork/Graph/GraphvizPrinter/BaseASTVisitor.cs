using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Graph.GraphvizPrinter {
    public interface IGraphvizASTVisitor {
        void Visit(ASTLeaf leaf);
        void Visit(GraphvizFileLayout layout);
        void Visit(GraphvizNode node);
        void Visit(GraphvizEdge edge);
        void Visit(GraphvizProperties properties);
        void Visit(GraphvizProperty property);
        void Visit(GraphvizPropertyValue value);
    }
    public abstract class BaseASTVisitor : IGraphvizASTVisitor {
        protected Stack<ASTNode> m_parents = new Stack<ASTNode>();

        public virtual void Visit(ASTNode node) {
            // Default implementation does nothing
            if (node is ASTComposite composite) {
                VisitChildren(composite);
            } else if (node is ASTLeaf leaf) {
                Visit(leaf);
            }
        }

        public virtual void Visit(ASTLeaf leaf) {
            // Default implementation does nothing
        }
        public virtual void Visit(GraphvizFileLayout layout) {
            VisitChildren(layout);
        }
        public virtual void Visit(GraphvizNode node) {
            VisitChildren(node);
        }
        public virtual void Visit(GraphvizEdge edge) {
            // Default implementation does nothing
            VisitChildren(edge);
        }
        public virtual void Visit(GraphvizProperties property) {
            // Default implementation does nothing
            VisitChildren(property);
        }
        public virtual void Visit(GraphvizProperty property) {
            VisitChildren(property);
        }
        public virtual void Visit(GraphvizPropertyValue value) {
            // Default implementation does nothing
        }
        public virtual void VisitChildren(ASTComposite composite) {
            foreach (var childList in composite.Children.Values) {
                foreach (var child in childList) {
                    m_parents.Push(composite);
                    child.Accept(this);
                    m_parents.Pop();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Tests {
    [TestClass]
    public class AdjacencyListStorageTests {
        private AdjacencyListStorage storage;
        private Node<int> node1;
        private Node<int> node2;
        private Node<int> node3;
        private Edge<string> edge1;
        private Edge<string> edge2;

        [TestInitialize]
        public void Setup() {
            storage = new AdjacencyListStorage();
            node1 = new Node<int>();
            node2 = new Node<int>();
            node3 = new Node<int>();
        }

        #region Node Management Tests

        [TestMethod]
        public void AddNode_ValidNode_AddsSuccessfully() {
            // Act
            storage.AddNode(node1);

            // Assert
            Assert.IsTrue(storage.HasNode(node1));
            Assert.AreEqual(1, storage.Nodes.Count);
            Assert.AreSame(node1, storage.Nodes[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNode_NullNode_ThrowsArgumentNullException() {
            // Act
            storage.AddNode(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddNode_DuplicateNode_ThrowsArgumentException() {
            // Arrange
            storage.AddNode(node1);

            // Act
            storage.AddNode(node1);
        }

        [TestMethod]
        public void AddNode_InitializesEdgeLists() {
            // Act
            storage.AddNode(node1);

            // Assert
            var outgoingEdges = storage.GetOutgoingEdges(node1);
            var incomingEdges = storage.GetIncomingEdges(node1);

            Assert.IsNotNull(outgoingEdges);
            Assert.IsNotNull(incomingEdges);
            Assert.AreEqual(0, outgoingEdges.Count);
            Assert.AreEqual(0, incomingEdges.Count);
        }

        [TestMethod]
        public void RemoveNode_ExistingNodeWithNoEdges_RemovesSuccessfully() {
            // Arrange
            storage.AddNode(node1);

            // Act
            storage.RemoveNode(node1);

            // Assert
            Assert.IsFalse(storage.HasNode(node1));
            Assert.AreEqual(0, storage.Nodes.Count);
        }

        [TestMethod]
        public void RemoveNode_NodeWithOutgoingEdges_RemovesNodeAndEdges() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddNode(node3);
            edge1 = new Edge<string>(node1, node2);
            edge2 = new Edge<string>(node1, node3);
            storage.AddEdge(edge1);
            storage.AddEdge(edge2);

            // Act
            storage.RemoveNode(node1);

            // Assert
            Assert.IsFalse(storage.HasNode(node1));
            Assert.IsFalse(storage.HasEdge(edge1));
            Assert.IsFalse(storage.HasEdge(edge2));
            Assert.AreEqual(2, storage.Nodes.Count);
            Assert.AreEqual(0, storage.Edges.Count);
            Assert.AreEqual(0, storage.GetIncomingEdges(node2).Count);
            Assert.AreEqual(0, storage.GetIncomingEdges(node3).Count);
        }

        [TestMethod]
        public void RemoveNode_NodeWithIncomingEdges_RemovesNodeAndEdges() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddNode(node3);
            edge1 = new Edge<string>(node2, node1);
            edge2 = new Edge<string>(node3, node1);
            storage.AddEdge(edge1);
            storage.AddEdge(edge2);

            // Act
            storage.RemoveNode(node1);

            // Assert
            Assert.IsFalse(storage.HasNode(node1));
            Assert.IsFalse(storage.HasEdge(edge1));
            Assert.IsFalse(storage.HasEdge(edge2));
            Assert.AreEqual(2, storage.Nodes.Count);
            Assert.AreEqual(0, storage.Edges.Count);
            Assert.AreEqual(0, storage.GetOutgoingEdges(node2).Count);
            Assert.AreEqual(0, storage.GetOutgoingEdges(node3).Count);
        }

        [TestMethod]
        public void RemoveNode_NodeWithBothIncomingAndOutgoingEdges_RemovesAllCorrectly() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddNode(node3);

            edge1 = new Edge<string>(node1, node2); // outgoing from node1
            edge2 = new Edge<string>(node3, node1); // incoming to node1
            var edge3 = new Edge<string>(node1, node3); // outgoing from node1

            storage.AddEdge(edge1);
            storage.AddEdge(edge2);
            storage.AddEdge(edge3);

            // Act
            storage.RemoveNode(node1);

            // Assert
            Assert.IsFalse(storage.HasNode(node1));
            Assert.IsFalse(storage.HasEdge(edge1));
            Assert.IsFalse(storage.HasEdge(edge2));
            Assert.IsFalse(storage.HasEdge(edge3));
            Assert.AreEqual(2, storage.Nodes.Count);
            Assert.AreEqual(0, storage.Edges.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveNode_NullNode_ThrowsArgumentNullException() {
            // Act
            storage.RemoveNode(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveNode_NonExistentNode_ThrowsArgumentException() {
            // Act
            storage.RemoveNode(node1);
        }

        [TestMethod]
        public void HasNode_ExistingNode_ReturnsTrue() {
            // Arrange
            storage.AddNode(node1);

            // Act & Assert
            Assert.IsTrue(storage.HasNode(node1));
        }

        [TestMethod]
        public void HasNode_NonExistentNode_ReturnsFalse() {
            // Act & Assert
            Assert.IsFalse(storage.HasNode(node1));
        }

        #endregion

        #region Edge Management Tests

        [TestMethod]
        public void AddEdge_ValidEdge_AddsSuccessfully() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            edge1 = new Edge<string>(node1, node2);

            // Act
            storage.AddEdge(edge1);

            // Assert
            Assert.IsTrue(storage.HasEdge(edge1));
            Assert.AreEqual(1, storage.Edges.Count);
            Assert.AreSame(edge1, storage.Edges[0]);
            Assert.IsTrue(storage.GetOutgoingEdges(node1).Contains(edge1));
            Assert.IsTrue(storage.GetIncomingEdges(node2).Contains(edge1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEdge_NullEdge_ThrowsArgumentNullException() {
            // Act
            storage.AddEdge(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddEdge_DuplicateEdge_ThrowsArgumentException() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            edge1 = new Edge<string>(node1, node2);
            storage.AddEdge(edge1);

            // Act
            storage.AddEdge(edge1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddEdge_NonExistentSourceNode_ThrowsArgumentException() {
            // Arrange
            storage.AddNode(node2);
            edge1 = new Edge<string>(node1, node2); // node1 not added to storage

            // Act
            storage.AddEdge(edge1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddEdge_NonExistentTargetNode_ThrowsArgumentException() {
            // Arrange
            storage.AddNode(node1);
            edge1 = new Edge<string>(node1, node2); // node2 not added to storage

            // Act
            storage.AddEdge(edge1);
        }

        [TestMethod]
        public void RemoveEdge_ExistingEdge_RemovesSuccessfully() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            edge1 = new Edge<string>(node1, node2);
            storage.AddEdge(edge1);

            // Act
            storage.RemoveEdge(edge1);

            // Assert
            Assert.IsFalse(storage.HasEdge(edge1));
            Assert.AreEqual(0, storage.Edges.Count);
            Assert.AreEqual(0, storage.GetOutgoingEdges(node1).Count);
            Assert.AreEqual(0, storage.GetIncomingEdges(node2).Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveEdge_NullEdge_ThrowsArgumentNullException() {
            // Act
            storage.RemoveEdge(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveEdge_NonExistentEdge_ThrowsArgumentException() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            edge1 = new Edge<string>(node1, node2);

            // Act
            storage.RemoveEdge(edge1);
        }

        [TestMethod]
        public void HasEdge_ExistingEdge_ReturnsTrue() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            edge1 = new Edge<string>(node1, node2);
            storage.AddEdge(edge1);

            // Act & Assert
            Assert.IsTrue(storage.HasEdge(edge1));
        }

        [TestMethod]
        public void HasEdge_NonExistentEdge_ReturnsFalse() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            edge1 = new Edge<string>(node1, node2);

            // Act & Assert
            Assert.IsFalse(storage.HasEdge(edge1));
        }

        #endregion

        #region Edge Retrieval Tests

        [TestMethod]
        public void GetOutgoingEdges_NodeWithOutgoingEdges_ReturnsCorrectEdges() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddNode(node3);
            edge1 = new Edge<string>(node1, node2);
            edge2 = new Edge<string>(node1, node3);
            storage.AddEdge(edge1);
            storage.AddEdge(edge2);

            // Act
            var outgoingEdges = storage.GetOutgoingEdges(node1);

            // Assert
            Assert.AreEqual(2, outgoingEdges.Count);
            Assert.IsTrue(outgoingEdges.Contains(edge1));
            Assert.IsTrue(outgoingEdges.Contains(edge2));
        }

        [TestMethod]
        public void GetOutgoingEdges_NodeWithNoOutgoingEdges_ReturnsEmptyList() {
            // Arrange
            storage.AddNode(node1);

            // Act
            var outgoingEdges = storage.GetOutgoingEdges(node1);

            // Assert
            Assert.AreEqual(0, outgoingEdges.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetOutgoingEdges_NullNode_ThrowsArgumentNullException() {
            // Act
            storage.GetOutgoingEdges(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetOutgoingEdges_NonExistentNode_ThrowsArgumentException() {
            // Act
            storage.GetOutgoingEdges(node1);
        }

        [TestMethod]
        public void GetIncomingEdges_NodeWithIncomingEdges_ReturnsCorrectEdges() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddNode(node3);
            edge1 = new Edge<string>(node1, node3);
            edge2 = new Edge<string>(node2, node3);
            storage.AddEdge(edge1);
            storage.AddEdge(edge2);

            // Act
            var incomingEdges = storage.GetIncomingEdges(node3);

            // Assert
            Assert.AreEqual(2, incomingEdges.Count);
            Assert.IsTrue(incomingEdges.Contains(edge1));
            Assert.IsTrue(incomingEdges.Contains(edge2));
        }

        [TestMethod]
        public void GetIncomingEdges_NodeWithNoIncomingEdges_ReturnsEmptyList() {
            // Arrange
            storage.AddNode(node1);

            // Act
            var incomingEdges = storage.GetIncomingEdges(node1);

            // Assert
            Assert.AreEqual(0, incomingEdges.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetIncomingEdges_NullNode_ThrowsArgumentNullException() {
            // Act
            storage.GetIncomingEdges(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetIncomingEdges_NonExistentNode_ThrowsArgumentException() {
            // Act
            storage.GetIncomingEdges(node1);
        }

        #endregion

        #region Connection Tests

        [TestMethod]
        public void AreConnected_ConnectedNodes_ReturnsTrue() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            edge1 = new Edge<string>(node1, node2);
            storage.AddEdge(edge1);

            // Act & Assert
            Assert.IsTrue(storage.AreConnected(node1, node2));
        }

        [TestMethod]
        public void AreConnected_NotConnectedNodes_ReturnsFalse() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);

            // Act & Assert
            Assert.IsFalse(storage.AreConnected(node1, node2));
        }

        [TestMethod]
        public void AreConnected_NonExistentSourceNode_ReturnsFalse() {
            // Arrange
            storage.AddNode(node2);

            // Act & Assert
            Assert.IsFalse(storage.AreConnected(node1, node2));
        }

        [TestMethod]
        public void AreConnected_SelfConnection_ReturnsCorrectResult() {
            // Arrange
            storage.AddNode(node1);
            edge1 = new Edge<string>(node1, node1);
            storage.AddEdge(edge1);

            // Act & Assert
            Assert.IsTrue(storage.AreConnected(node1, node1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AreConnected_NullSourceNode_ThrowsArgumentNullException() {
            // Act
            storage.AreConnected(null, node1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AreConnected_NullTargetNode_ThrowsArgumentNullException() {
            // Act
            storage.AreConnected(node1, null);
        }

        #endregion

        #region Read-Only Collection Tests

        [TestMethod]
        public void Nodes_ReturnsReadOnlyCollection() {
            // Arrange
            storage.AddNode(node1);

            // Act
            var nodes = storage.Nodes;

            // Assert
            Assert.IsInstanceOfType(nodes, typeof(IReadOnlyList<INode>));
            Assert.AreEqual(1, nodes.Count);
            Assert.AreSame(node1, nodes[0]);
        }

        [TestMethod]
        public void Edges_ReturnsReadOnlyCollection() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            edge1 = new Edge<string>(node1, node2);
            storage.AddEdge(edge1);

            // Act
            var edges = storage.Edges;

            // Assert
            Assert.IsInstanceOfType(edges, typeof(IReadOnlyList<IEdge>));
            Assert.AreEqual(1, edges.Count);
            Assert.AreSame(edge1, edges[0]);
        }

        [TestMethod]
        public void GetOutgoingEdges_ReturnsReadOnlyCollection() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            edge1 = new Edge<string>(node1, node2);
            storage.AddEdge(edge1);

            // Act
            var outgoingEdges = storage.GetOutgoingEdges(node1);

            // Assert
            Assert.IsInstanceOfType(outgoingEdges, typeof(IReadOnlyList<IEdge>));
        }

        [TestMethod]
        public void GetIncomingEdges_ReturnsReadOnlyCollection() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            edge1 = new Edge<string>(node1, node2);
            storage.AddEdge(edge1);

            // Act
            var incomingEdges = storage.GetIncomingEdges(node2);

            // Assert
            Assert.IsInstanceOfType(incomingEdges, typeof(IReadOnlyList<IEdge>));
        }

        #endregion

        #region Integration and Complex Scenario Tests

        [TestMethod]
        public void ComplexScenario_MultipleNodesAndEdges_HandlesCorrectly() {
            // Arrange - Create a small directed graph: node1 -> node2 -> node3, node1 -> node3
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddNode(node3);

            edge1 = new Edge<string>(node1, node2);
            edge2 = new Edge<string>(node2, node3);
            var edge3 = new Edge<string>(node1, node3);

            storage.AddEdge(edge1);
            storage.AddEdge(edge2);
            storage.AddEdge(edge3);

            // Act & Assert - Verify structure
            Assert.AreEqual(3, storage.Nodes.Count);
            Assert.AreEqual(3, storage.Edges.Count);

            // Verify outgoing edges
            Assert.AreEqual(2, storage.GetOutgoingEdges(node1).Count);
            Assert.AreEqual(1, storage.GetOutgoingEdges(node2).Count);
            Assert.AreEqual(0, storage.GetOutgoingEdges(node3).Count);

            // Verify incoming edges
            Assert.AreEqual(0, storage.GetIncomingEdges(node1).Count);
            Assert.AreEqual(1, storage.GetIncomingEdges(node2).Count);
            Assert.AreEqual(2, storage.GetIncomingEdges(node3).Count);

            // Verify connections
            Assert.IsTrue(storage.AreConnected(node1, node2));
            Assert.IsTrue(storage.AreConnected(node2, node3));
            Assert.IsTrue(storage.AreConnected(node1, node3));
            Assert.IsFalse(storage.AreConnected(node2, node1));
            Assert.IsFalse(storage.AreConnected(node3, node1));
        }

        [TestMethod]
        public void ParallelEdges_MultipleEdgesBetweenSameNodes_HandlesCorrectly() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);

            edge1 = new Edge<string>(node1, node2);
            edge2 = new Edge<string>(node1, node2); // Different edge object, same source/target

            storage.AddEdge(edge1);
            storage.AddEdge(edge2);

            // Act & Assert
            Assert.AreEqual(2, storage.Edges.Count);
            Assert.AreEqual(2, storage.GetOutgoingEdges(node1).Count);
            Assert.AreEqual(2, storage.GetIncomingEdges(node2).Count);
            Assert.IsTrue(storage.AreConnected(node1, node2));
        }

        [TestMethod]
        public void EmptyGraph_AllOperations_HandleCorrectly() {
            // Act & Assert
            Assert.AreEqual(0, storage.Nodes.Count);
            Assert.AreEqual(0, storage.Edges.Count);
            Assert.IsFalse(storage.HasNode(node1));
            Assert.IsFalse(storage.HasEdge(new Edge<string>(node1, node2)));
            Assert.IsFalse(storage.AreConnected(node1, node2));
        }

        #endregion
    }
}
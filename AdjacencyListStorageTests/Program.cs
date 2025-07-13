using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Graph.Tests {

    [TestClass]
    public class DirectedAdjacencyListStorageTests {
        private DirectedAdjacencyListStorage storage;
        private Node<int> node1;
        private Node<int> node2;
        private Node<int> node3;
        private Edge<string> edge1;
        private Edge<string> edge2;

        [TestInitialize]
        public void Setup() {
            storage = new DirectedAdjacencyListStorage();
            node1 = new Node<int>();
            node2 = new Node<int>();
            node3 = new Node<int>();
        }

        private void SetupNodesAndEdges() {
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddNode(node3);

            edge1 = new Edge<string>(node1, node2);
            edge2 = new Edge<string>(node2, node3);
            storage.AddEdge(edge1);
            storage.AddEdge(edge2);
        }

        #region AddNode Tests
        [TestMethod]
        public void AddNode_ValidNode_ShouldAddSuccessfully() {
            // Act
            storage.AddNode(node1);

            // Assert
            Assert.IsTrue(storage.HasNode(node1));
            Assert.AreEqual(1, storage.Nodes.Count);
            Assert.IsTrue(storage.Nodes.Contains(node1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNode_NullNode_ShouldThrowArgumentNullException() {
            // Act
            storage.AddNode(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddNode_DuplicateNode_ShouldThrowArgumentException() {
            // Arrange
            storage.AddNode(node1);

            // Act
            storage.AddNode(node1);
        }

        [TestMethod]
        public void AddNode_ShouldInitializeEdgeLists() {
            // Act
            storage.AddNode(node1);

            // Assert - Should not throw when getting edges
            var outgoing = storage.GetOutgoingEdges(node1);
            var incoming = storage.GetIncomingEdges(node1);

            Assert.IsNotNull(outgoing);
            Assert.IsNotNull(incoming);
            Assert.AreEqual(0, outgoing.Count);
            Assert.AreEqual(0, incoming.Count);
        }
        #endregion

        #region AddEdge Tests
        [TestMethod]
        public void AddEdge_ValidEdge_ShouldAddSuccessfully() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            var edge = new Edge<string>(node1, node2);

            // Act
            storage.AddEdge(edge);

            // Assert
            Assert.IsTrue(storage.HasEdge(edge));
            Assert.AreEqual(1, storage.Edges.Count);
            Assert.IsTrue(storage.GetOutgoingEdges(node1).Contains(edge));
            Assert.IsTrue(storage.GetIncomingEdges(node2).Contains(edge));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEdge_NullEdge_ShouldThrowArgumentNullException() {
            // Act
            storage.AddEdge(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddEdge_DuplicateEdge_ShouldThrowArgumentException() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            var edge = new Edge<string>(node1, node2);
            storage.AddEdge(edge);

            // Act
            storage.AddEdge(edge);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddEdge_SourceNodeNotExists_ShouldThrowArgumentException() {
            // Arrange
            storage.AddNode(node2);
            var edge = new Edge<string>(node1, node2); // node1 not added

            // Act
            storage.AddEdge(edge);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddEdge_TargetNodeNotExists_ShouldThrowArgumentException() {
            // Arrange
            storage.AddNode(node1);
            var edge = new Edge<string>(node1, node2); // node2 not added

            // Act
            storage.AddEdge(edge);
        }
        #endregion

        #region GetOutgoingEdges Tests
        [TestMethod]
        public void GetOutgoingEdges_ValidNode_ShouldReturnCorrectEdges() {
            // Arrange
            SetupNodesAndEdges();

            // Act
            var outgoingEdges = storage.GetOutgoingEdges(node1);

            // Assert
            Assert.AreEqual(1, outgoingEdges.Count);
            Assert.IsTrue(outgoingEdges.Contains(edge1));
        }

        [TestMethod]
        public void GetOutgoingEdges_NodeWithNoOutgoingEdges_ShouldReturnEmptyList() {
            // Arrange
            SetupNodesAndEdges();

            // Act
            var outgoingEdges = storage.GetOutgoingEdges(node3);

            // Assert
            Assert.AreEqual(0, outgoingEdges.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetOutgoingEdges_NullNode_ShouldThrowArgumentNullException() {
            // Act
            storage.GetOutgoingEdges(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetOutgoingEdges_NodeNotExists_ShouldThrowArgumentException() {
            // Act
            storage.GetOutgoingEdges(node1);
        }
        #endregion

        #region GetIncomingEdges Tests
        [TestMethod]
        public void GetIncomingEdges_ValidNode_ShouldReturnCorrectEdges() {
            // Arrange
            SetupNodesAndEdges();

            // Act
            var incomingEdges = storage.GetIncomingEdges(node2);

            // Assert
            Assert.AreEqual(1, incomingEdges.Count);
            Assert.IsTrue(incomingEdges.Contains(edge1));
        }

        [TestMethod]
        public void GetIncomingEdges_NodeWithNoIncomingEdges_ShouldReturnEmptyList() {
            // Arrange
            SetupNodesAndEdges();

            // Act
            var incomingEdges = storage.GetIncomingEdges(node1);

            // Assert
            Assert.AreEqual(0, incomingEdges.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetIncomingEdges_NullNode_ShouldThrowArgumentNullException() {
            // Act
            storage.GetIncomingEdges(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetIncomingEdges_NodeNotExists_ShouldThrowArgumentException() {
            // Act
            storage.GetIncomingEdges(node1);
        }
        #endregion

        #region RemoveNode Tests
        [TestMethod]
        public void RemoveNode_ValidNode_ShouldRemoveNodeAndAssociatedEdges() {
            // Arrange
            SetupNodesAndEdges();

            // Act
            storage.RemoveNode(node2);

            // Assert
            Assert.IsFalse(storage.HasNode(node2));
            Assert.IsFalse(storage.HasEdge(edge1));
            Assert.IsFalse(storage.HasEdge(edge2));
            Assert.AreEqual(2, storage.Nodes.Count);
            Assert.AreEqual(0, storage.Edges.Count);
        }

        [TestMethod]
        public void RemoveNode_NodeWithOnlyOutgoingEdges_ShouldRemoveCorrectly() {
            // Arrange
            SetupNodesAndEdges();

            // Act
            storage.RemoveNode(node1);

            // Assert
            Assert.IsFalse(storage.HasNode(node1));
            Assert.IsFalse(storage.HasEdge(edge1));
            Assert.IsTrue(storage.HasEdge(edge2)); // edge2 should remain
            Assert.AreEqual(0, storage.GetIncomingEdges(node2).Count);
        }

        [TestMethod]
        public void RemoveNode_NodeWithOnlyIncomingEdges_ShouldRemoveCorrectly() {
            // Arrange
            SetupNodesAndEdges();

            // Act
            storage.RemoveNode(node3);

            // Assert
            Assert.IsFalse(storage.HasNode(node3));
            Assert.IsFalse(storage.HasEdge(edge2));
            Assert.IsTrue(storage.HasEdge(edge1)); // edge1 should remain
            Assert.AreEqual(0, storage.GetOutgoingEdges(node2).Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveNode_NodeNotExists_ShouldThrowArgumentException() {
            // Act
            storage.RemoveNode(node1);
        }
        #endregion

        #region RemoveEdge Tests
        [TestMethod]
        public void RemoveEdge_ValidEdge_ShouldRemoveSuccessfully() {
            // Arrange
            SetupNodesAndEdges();

            // Act
            storage.RemoveEdge(edge1);

            // Assert
            Assert.IsFalse(storage.HasEdge(edge1));
            Assert.IsFalse(storage.GetOutgoingEdges(node1).Contains(edge1));
            Assert.IsFalse(storage.GetIncomingEdges(node2).Contains(edge1));
            Assert.AreEqual(1, storage.Edges.Count); // edge2 should remain
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveEdge_EdgeNotExists_ShouldThrowArgumentException() {
            // Arrange
            var nonExistentEdge = new Edge<string>(node1, node2);

            // Act
            storage.RemoveEdge(nonExistentEdge);
        }
        #endregion

        #region AreConnected Tests
        [TestMethod]
        public void AreConnected_ConnectedNodes_ShouldReturnTrue() {
            // Arrange
            SetupNodesAndEdges();

            // Act & Assert
            Assert.IsTrue(storage.AreConnected(node1, node2));
            Assert.IsTrue(storage.AreConnected(node2, node3));
        }

        [TestMethod]
        public void AreConnected_NotConnectedNodes_ShouldReturnFalse() {
            // Arrange
            SetupNodesAndEdges();

            // Act & Assert
            Assert.IsFalse(storage.AreConnected(node2, node1)); // Directed graph - reverse should be false
            Assert.IsFalse(storage.AreConnected(node1, node3)); // Not directly connected
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AreConnected_NullSource_ShouldThrowArgumentNullException() {
            // Act
            storage.AreConnected(null, node2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AreConnected_NullTarget_ShouldThrowArgumentNullException() {
            // Act
            storage.AreConnected(node1, null);
        }
        #endregion

        #region Complex Scenarios
        [TestMethod]
        public void ComplexScenario_MultipleEdgesBetweenSameNodes_ShouldHandleCorrectly() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            var edge1 = new Edge<string>(node1, node2);
            var edge2 = new Edge<int>(node1, node2);

            // Act
            storage.AddEdge(edge1);
            storage.AddEdge(edge2);

            // Assert
            Assert.AreEqual(2, storage.GetOutgoingEdges(node1).Count);
            Assert.AreEqual(2, storage.GetIncomingEdges(node2).Count);
            Assert.IsTrue(storage.AreConnected(node1, node2));
        }

        [TestMethod]
        public void ComplexScenario_SelfLoop_ShouldHandleCorrectly() {
            // Arrange
            storage.AddNode(node1);
            var selfLoop = new Edge<string>(node1, node1);

            // Act
            storage.AddEdge(selfLoop);

            // Assert
            Assert.IsTrue(storage.HasEdge(selfLoop));
            Assert.IsTrue(storage.GetOutgoingEdges(node1).Contains(selfLoop));
            Assert.IsTrue(storage.GetIncomingEdges(node1).Contains(selfLoop));
            Assert.IsTrue(storage.AreConnected(node1, node1));
        }
        #endregion
    }

    [TestClass]
    public class UndirectedAdjacencyListStorageTests {
        private UndirectedAdjacencyListStorage storage;
        private Node<int> node1;
        private Node<int> node2;
        private Node<int> node3;
        private Edge<string> edge1;
        private Edge<string> edge2;

        [TestInitialize]
        public void Setup() {
            storage = new UndirectedAdjacencyListStorage();
            node1 = new Node<int>();
            node2 = new Node<int>();
            node3 = new Node<int>();
        }

        private void SetupNodesAndEdges() {
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddNode(node3);

            edge1 = new Edge<string>(node1, node2);
            edge2 = new Edge<string>(node2, node3);
            storage.AddEdge(edge1);
            storage.AddEdge(edge2);
        }

        #region GetEdges Tests
        [TestMethod]
        public void GetEdges_NodeWithEdges_ShouldReturnAllAdjacentEdges() {
            // Arrange
            SetupNodesAndEdges();

            // Act
            var edges = storage.GetEdges(node2);

            // Assert
            Assert.AreEqual(2, edges.Count);
            Assert.IsTrue(edges.Contains(edge1));
            Assert.IsTrue(edges.Contains(edge2));
        }

        [TestMethod]
        public void GetEdges_NodeWithNoEdges_ShouldReturnEmptyList() {
            // Arrange
            storage.AddNode(node1);

            // Act
            var edges = storage.GetEdges(node1);

            // Assert
            Assert.AreEqual(0, edges.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetEdges_NullNode_ShouldThrowArgumentNullException() {
            // Act
            storage.GetEdges(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetEdges_NodeNotExists_ShouldThrowArgumentException() {
            // Act
            storage.GetEdges(node1);
        }
        #endregion

        #region GetNeighbors Tests
        [TestMethod]
        public void GetNeighbors_NodeWithNeighbors_ShouldReturnCorrectNodes() {
            // Arrange
            SetupNodesAndEdges();

            // Act
            var neighbors = storage.GetNeighbors(node2);

            // Assert
            Assert.AreEqual(2, neighbors.Count);
            Assert.IsTrue(neighbors.Contains(node1));
            Assert.IsTrue(neighbors.Contains(node3));
        }

        [TestMethod]
        public void GetNeighbors_NodeWithNoNeighbors_ShouldReturnEmptyList() {
            // Arrange
            storage.AddNode(node1);

            // Act
            var neighbors = storage.GetNeighbors(node1);

            // Assert
            Assert.AreEqual(0, neighbors.Count);
        }

        [TestMethod]
        public void GetNeighbors_EdgeDirection_ShouldWorkBothWays() {
            // Arrange
            SetupNodesAndEdges();

            // Act
            var neighborsOfNode1 = storage.GetNeighbors(node1);
            var neighborsOfNode2 = storage.GetNeighbors(node2);

            // Assert - In undirected graph, both nodes should see each other as neighbors
            Assert.IsTrue(neighborsOfNode1.Contains(node2));
            Assert.IsTrue(neighborsOfNode2.Contains(node1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNeighbors_NullNode_ShouldThrowArgumentNullException() {
            // Act
            storage.GetNeighbors(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetNeighbors_NodeNotExists_ShouldThrowArgumentException() {
            // Act
            storage.GetNeighbors(node1);
        }
        #endregion

        #region AddEdge Tests (Undirected Specific)
        [TestMethod]
        public void AddEdge_UndirectedEdge_ShouldAddToBothNodes() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            var edge = new Edge<string>(node1, node2);

            // Act
            storage.AddEdge(edge);

            // Assert
            Assert.IsTrue(storage.GetEdges(node1).Contains(edge));
            Assert.IsTrue(storage.GetEdges(node2).Contains(edge));
            Assert.IsTrue(storage.GetNeighbors(node1).Contains(node2));
            Assert.IsTrue(storage.GetNeighbors(node2).Contains(node1));
        }
        #endregion

        #region RemoveNode Tests (Undirected Specific)
        [TestMethod]
        public void RemoveNode_UndirectedGraph_ShouldRemoveFromAllNeighbors() {
            // Arrange
            SetupNodesAndEdges();

            // Act
            storage.RemoveNode(node2);

            // Assert
            Assert.IsFalse(storage.HasNode(node2));
            Assert.IsFalse(storage.HasEdge(edge1));
            Assert.IsFalse(storage.HasEdge(edge2));

            // Verify neighbors are updated
            Assert.AreEqual(0, storage.GetNeighbors(node1).Count);
            Assert.AreEqual(0, storage.GetNeighbors(node3).Count);
        }
        #endregion

        #region RemoveEdge Tests (Undirected Specific)
        [TestMethod]
        public void RemoveEdge_UndirectedEdge_ShouldRemoveFromBothNodes() {
            // Arrange
            SetupNodesAndEdges();

            // Act
            storage.RemoveEdge(edge1);

            // Assert
            Assert.IsFalse(storage.HasEdge(edge1));
            Assert.IsFalse(storage.GetEdges(node1).Contains(edge1));
            Assert.IsFalse(storage.GetEdges(node2).Contains(edge1));
            Assert.IsFalse(storage.GetNeighbors(node1).Contains(node2));

            // node2 should still have node3 as neighbor through edge2
            Assert.IsTrue(storage.GetNeighbors(node2).Contains(node3));
        }
        #endregion

        #region AreConnected Tests (Undirected Specific)
        [TestMethod]
        public void AreConnected_UndirectedGraph_ShouldWorkBothDirections() {
            // Arrange
            SetupNodesAndEdges();

            // Act & Assert - Should work both ways in undirected graph
            Assert.IsTrue(storage.AreConnected(node1, node2));
            Assert.IsTrue(storage.AreConnected(node2, node1));
            Assert.IsTrue(storage.AreConnected(node2, node3));
            Assert.IsTrue(storage.AreConnected(node3, node2));
        }
        #endregion

        #region Complex Scenarios
        [TestMethod]
        public void ComplexScenario_SelfLoop_UndirectedGraph() {
            // Arrange
            storage.AddNode(node1);
            var selfLoop = new Edge<string>(node1, node1);

            // Act
            storage.AddEdge(selfLoop);

            // Assert
            Assert.IsTrue(storage.HasEdge(selfLoop));
            Assert.IsTrue(storage.GetEdges(node1).Contains(selfLoop));
            Assert.IsTrue(storage.GetNeighbors(node1).Contains(node1));
            Assert.IsTrue(storage.AreConnected(node1, node1));
        }

        [TestMethod]
        public void ComplexScenario_StarTopology_ShouldHandleCorrectly() {
            // Arrange - Create a star topology with node2 at center
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddNode(node3);
            var node4 = new Node<int>();
            storage.AddNode(node4);

            var edge1 = new Edge<string>(node2, node1);
            var edge2 = new Edge<string>(node2, node3);
            var edge3 = new Edge<string>(node2, node4);

            // Act
            storage.AddEdge(edge1);
            storage.AddEdge(edge2);
            storage.AddEdge(edge3);

            // Assert
            Assert.AreEqual(3, storage.GetNeighbors(node2).Count);
            Assert.AreEqual(1, storage.GetNeighbors(node1).Count);
            Assert.AreEqual(1, storage.GetNeighbors(node3).Count);
            Assert.AreEqual(1, storage.GetNeighbors(node4).Count);

            // Center node should be connected to all others
            Assert.IsTrue(storage.AreConnected(node2, node1));
            Assert.IsTrue(storage.AreConnected(node2, node3));
            Assert.IsTrue(storage.AreConnected(node2, node4));

            // Leaf nodes should not be connected to each other
            Assert.IsFalse(storage.AreConnected(node1, node3));
            Assert.IsFalse(storage.AreConnected(node1, node4));
            Assert.IsFalse(storage.AreConnected(node3, node4));
        }
        #endregion
    }

    [TestClass]
    public class AdjacencyListStorageBaseTests {
        private DirectedAdjacencyListStorage storage; // Using concrete implementation to test base class
        private Node<int> node1;
        private Node<int> node2;

        [TestInitialize]
        public void Setup() {
            storage = new DirectedAdjacencyListStorage();
            node1 = new Node<int>();
            node2 = new Node<int>();
        }

        #region Base Class Tests
        [TestMethod]
        public void Nodes_Property_ShouldReturnReadOnlyList() {
            // Arrange
            storage.AddNode(node1);

            // Act
            var nodes = storage.Nodes;

            // Assert
            Assert.IsNotNull(nodes);
            Assert.AreEqual(1, nodes.Count);
            Assert.IsTrue(nodes.Contains(node1));

            // Verify it's read-only by attempting to cast
            Assert.IsInstanceOfType(nodes, typeof(System.Collections.ObjectModel.ReadOnlyCollection<Node>));
        }

        [TestMethod]
        public void Edges_Property_ShouldReturnReadOnlyList() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            var edge = new Edge<string>(node1, node2);
            storage.AddEdge(edge);

            // Act
            var edges = storage.Edges;

            // Assert
            Assert.IsNotNull(edges);
            Assert.AreEqual(1, edges.Count);
            Assert.IsTrue(edges.Contains(edge));

            // Verify it's read-only by attempting to cast
            Assert.IsInstanceOfType(edges, typeof(System.Collections.ObjectModel.ReadOnlyCollection<Edge>));
        }

        [TestMethod]
        public void HasNode_ExistingNode_ShouldReturnTrue() {
            // Arrange
            storage.AddNode(node1);

            // Act & Assert
            Assert.IsTrue(storage.HasNode(node1));
        }

        [TestMethod]
        public void HasNode_NonExistingNode_ShouldReturnFalse() {
            // Act & Assert
            Assert.IsFalse(storage.HasNode(node1));
        }

        [TestMethod]
        public void HasEdge_ExistingEdge_ShouldReturnTrue() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            var edge = new Edge<string>(node1, node2);
            storage.AddEdge(edge);

            // Act & Assert
            Assert.IsTrue(storage.HasEdge(edge));
        }

        [TestMethod]
        public void HasEdge_NonExistingEdge_ShouldReturnFalse() {
            // Arrange
            var edge = new Edge<string>(node1, node2);

            // Act & Assert
            Assert.IsFalse(storage.HasEdge(edge));
        }
        #endregion
    }
}
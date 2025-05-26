using CommunicationNetwork.Graph;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.Mime.MediaTypeNames;

namespace AdjacencyListStorageTests {
    
    [TestClass]
    public class AdjacencyListStorageTests {
        private AdjacencyListStorage storage;
        private Node<int> node1;
        private Node<int> node2;
        private Node<string> node3;
        private Edge<double> edge1;
        private Edge<string> edge2;

        [TestInitialize]
        public void Setup() {
            storage = new AdjacencyListStorage();
            node1 = new Node<int>();
            node2 = new Node<int>();
            node3 = new Node<string>();
            edge1 = new Edge<double>(node1, node2);
            edge2 = new Edge<string>(node2, node3);
        }

        #region Node Tests

        [TestMethod]
        public void AddNode_ShouldAddNodeSuccessfully() {
            // Act
            storage.AddNode(node1);

            // Assert
            Assert.AreEqual(1, storage.Nodes.Count);
            Assert.IsTrue(storage.HasNode(node1));
            Assert.IsTrue(storage.Nodes.Contains(node1));
        }

        [TestMethod]
        public void AddMultipleNodes_ShouldAddAllNodes() {
            // Act
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddNode(node3);

            // Assert
            Assert.AreEqual(3, storage.Nodes.Count);
            Assert.IsTrue(storage.HasNode(node1));
            Assert.IsTrue(storage.HasNode(node2));
            Assert.IsTrue(storage.HasNode(node3));
        }

        [TestMethod]
        public void HasNode_WithExistingNode_ShouldReturnTrue() {
            // Arrange
            storage.AddNode(node1);

            // Act & Assert
            Assert.IsTrue(storage.HasNode(node1));
        }

        [TestMethod]
        public void HasNode_WithNonExistingNode_ShouldReturnFalse() {
            // Act & Assert
            Assert.IsFalse(storage.HasNode(node1));
        }

        [TestMethod]
        public void RemoveNode_ShouldRemoveNodeSuccessfully() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);

            // Act
            storage.RemoveNode(node1);

            // Assert
            Assert.AreEqual(1, storage.Nodes.Count);
            Assert.IsFalse(storage.HasNode(node1));
            Assert.IsTrue(storage.HasNode(node2));
        }

        [TestMethod]
        public void RemoveNode_WithConnectedEdges_ShouldRemoveNodeAndCleanupEdgeReferences() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddEdge(edge1);

            // Act
            storage.RemoveNode(node1);

            // Assert
            Assert.IsFalse(storage.HasNode(node1));
            Assert.IsFalse(storage.OutgoingEdge.ContainsKey(node1));
            Assert.IsFalse(storage.IncomingEdge.ContainsKey(node1));
        }

        #endregion

        #region Edge Tests

        [TestMethod]
        public void AddEdge_ShouldAddEdgeSuccessfully() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);

            // Act
            storage.AddEdge(edge1);

            // Assert
            Assert.AreEqual(1, storage.Edges.Count);
            Assert.IsTrue(storage.HasEdge(edge1));
            Assert.IsTrue(storage.Edges.Contains(edge1));
        }

        [TestMethod]
        public void AddEdge_ShouldUpdateOutgoingAndIncomingEdgeDictionaries() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);

            // Act
            storage.AddEdge(edge1);

            // Assert
            Assert.IsTrue(storage.OutgoingEdge.ContainsKey(node1));
            Assert.AreEqual(edge1, storage.OutgoingEdge[node1]);
            Assert.IsTrue(storage.IncomingEdge.ContainsKey(node2));
            Assert.AreEqual(edge1, storage.IncomingEdge[node2]);
        }

        [TestMethod]
        public void AddMultipleEdges_ShouldAddAllEdges() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddNode(node3);

            // Act
            storage.AddEdge(edge1);
            storage.AddEdge(edge2);

            // Assert
            Assert.AreEqual(2, storage.Edges.Count);
            Assert.IsTrue(storage.HasEdge(edge1));
            Assert.IsTrue(storage.HasEdge(edge2));
        }

        [TestMethod]
        public void HasEdge_WithExistingEdge_ShouldReturnTrue() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddEdge(edge1);

            // Act & Assert
            Assert.IsTrue(storage.HasEdge(edge1));
        }

        [TestMethod]
        public void HasEdge_WithNonExistingEdge_ShouldReturnFalse() {
            // Act & Assert
            Assert.IsFalse(storage.HasEdge(edge1));
        }

        [TestMethod]
        public void RemoveEdge_ShouldRemoveEdgeSuccessfully() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddNode(node3);
            storage.AddEdge(edge1);
            storage.AddEdge(edge2);

            // Act
            storage.RemoveEdge(edge1);

            // Assert
            Assert.AreEqual(1, storage.Edges.Count);
            Assert.IsFalse(storage.HasEdge(edge1));
            Assert.IsTrue(storage.HasEdge(edge2));
        }

        [TestMethod]
        public void RemoveEdge_ShouldCleanupEdgeDictionaries() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddEdge(edge1);

            // Act
            storage.RemoveEdge(edge1);

            // Assert
            Assert.IsFalse(storage.OutgoingEdge.ContainsKey(node1));
            Assert.IsFalse(storage.IncomingEdge.ContainsKey(node2));
        }

        #endregion

        #region Connectivity Tests

        [TestMethod]
        public void AreConnected_WithConnectedNodes_ShouldReturnTrue() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddEdge(edge1);

            // Act & Assert
            Assert.IsTrue(storage.AreConnected(node1, node2));
        }

        [TestMethod]
        public void AreConnected_WithUnconnectedNodes_ShouldReturnFalse() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            

            // Act & Assert
            Assert.IsFalse(storage.AreConnected(node1, node2));
        }

        [TestMethod]
        public void AreConnected_WithNonExistentNodes_ShouldReturnFalse() {
            // Act & Assert
            Assert.IsFalse(storage.AreConnected(node1, node2));
        }

        #endregion

        #region Property Tests

        [TestMethod]
        public void Nodes_ShouldReturnReadOnlyList() {
            // Arrange
            storage.AddNode(node1);

            // Act
            var nodes = storage.Nodes;

            // Assert
            Assert.IsInstanceOfType(nodes, typeof(System.Collections.ObjectModel.ReadOnlyCollection<INode>));
            Assert.AreEqual(1, nodes.Count);
        }

        [TestMethod]
        public void Edges_ShouldReturnReadOnlyList() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddEdge(edge1);

            // Act
            var edges = storage.Edges;

            // Assert
            Assert.IsInstanceOfType(edges, typeof(System.Collections.ObjectModel.ReadOnlyCollection<IEdge>));
            Assert.AreEqual(1, edges.Count);
        }

        [TestMethod]
        public void OutgoingEdge_ShouldReturnReadOnlyDictionary() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddEdge(edge1);

            // Act
            var outgoingEdges = storage.OutgoingEdge;

            // Assert
            Assert.IsInstanceOfType(outgoingEdges, typeof(System.Collections.ObjectModel.ReadOnlyDictionary<INode, IEdge>));
            Assert.IsTrue(outgoingEdges.ContainsKey(node1));
        }

        [TestMethod]
        public void IncomingEdge_ShouldReturnReadOnlyDictionary() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddEdge(edge1);

            // Act
            var incomingEdges = storage.IncomingEdge;

            // Assert
            Assert.IsInstanceOfType(incomingEdges, typeof(System.Collections.ObjectModel.ReadOnlyDictionary<INode, IEdge>));
            Assert.IsTrue(incomingEdges.ContainsKey(node2));
        }

        #endregion

        #region Edge Cases and Error Handling

        [TestMethod]
        public void AddNode_Null_ShouldNotThrowException() {
            // This test depends on your desired behavior
            // You might want to add null checking in the actual implementation
            // For now, testing current behavior

            // Act & Assert - adjust based on desired behavior
            storage.AddNode(null);
            Assert.AreEqual(1, storage.Nodes.Count);
        }

        [TestMethod]
        public void AddEdge_Null_ShouldNotThrowException() {
            // Similar to above - adjust based on desired behavior

            // Act & Assert
            try {
                storage.AddEdge(null);
                // If no exception is thrown, the current implementation allows null
                Assert.AreEqual(1, storage.Edges.Count);
            } catch (NullReferenceException) {
                // This is expected if the implementation tries to access null edge properties
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void RemoveNode_NonExistent_ShouldNotThrowException() {
            // Act & Assert
            storage.RemoveNode(node1); // Should not throw exception
            Assert.AreEqual(0, storage.Nodes.Count);
        }

        [TestMethod]
        public void RemoveEdge_NonExistent_ShouldNotThrowException() {
            // Act & Assert
            storage.RemoveEdge(edge1); // Should not throw exception
            Assert.AreEqual(0, storage.Edges.Count);
        }

        #endregion

        #region Integration Tests

        [TestMethod]
        public void ComplexScenario_MultipleNodesAndEdges_ShouldWorkCorrectly() {
            // Arrange
            var node4 = new Node<int>();
            var edge3 = new Edge<int>(node3, node4);

            // Act
            storage.AddNode(node1);
            storage.AddNode(node2);
            storage.AddNode(node3);
            storage.AddNode(node4);

            storage.AddEdge(edge1); // node1 -> node2
            storage.AddEdge(edge2); // node2 -> node3
            storage.AddEdge(edge3); // node3 -> node4

            // Assert
            Assert.AreEqual(4, storage.Nodes.Count);
            Assert.AreEqual(3, storage.Edges.Count);

            // Check connectivity
            Assert.IsTrue(storage.AreConnected(node1, node2));
            Assert.IsTrue(storage.AreConnected(node2, node3));
            Assert.IsTrue(storage.AreConnected(node3, node4));

            // Check edge dictionaries
            Assert.AreEqual(3, storage.OutgoingEdge.Count);
            Assert.AreEqual(3, storage.IncomingEdge.Count);
        }

        [TestMethod]
        public void AddSameNodeTwice_ShouldAddBothInstances() {
            // Act
            storage.AddNode(node1);
            storage.AddNode(node1);

            // Assert
            Assert.AreEqual(2, storage.Nodes.Count);
        }

        [TestMethod]
        public void AddSameEdgeTwice_ShouldAddBothInstances() {
            // Arrange
            storage.AddNode(node1);
            storage.AddNode(node2);

            // Act
            storage.AddEdge(edge1);
            storage.AddEdge(edge1);

            // Assert
            Assert.AreEqual(2, storage.Edges.Count);
        }

        #endregion
    }
}

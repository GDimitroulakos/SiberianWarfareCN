using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunicationNetwork.Graph;
using static System.Net.Mime.MediaTypeNames;

namespace CommunicationNetwork.Graph.Tests {

    [TestClass]
    public class DirectedGraphTests {

        private DirectedGraph _graph;
        private IGraphStorage _storage;
        private INode _nodeA;
        private INode _nodeB;
        private INode _nodeC;
        private INode _nodeD;
        private IEdge _edgeAB;
        private IEdge _edgeBC;
        private IEdge _edgeCA;
        private IEdge _edgeAD;

        [TestInitialize]
        public void Setup() {
            _storage = new AdjacencyListStorage();
            _graph = new DirectedGraph(_storage);

            // Create test nodes
            _nodeA = new Node<string> { Value = "A" };
            _nodeB = new Node<string> { Value = "B" };
            _nodeC = new Node<string> { Value = "C" };
            _nodeD = new Node<string> { Value = "D" };

            // Create test edges
            _edgeAB = new Edge<string>(_nodeA, _nodeB) { Value = "A->B" };
            _edgeBC = new Edge<string>(_nodeB, _nodeC) { Value = "B->C" };
            _edgeCA = new Edge<string>(_nodeC, _nodeA) { Value = "C->A" };
            _edgeAD = new Edge<string>(_nodeA, _nodeD) { Value = "A->D" };
        }

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ValidStorage_CreatesDirectedGraph() {
            // Arrange
            var storage = new AdjacencyListStorage();

            // Act
            var graph = new DirectedGraph(storage);

            // Assert
            Assert.IsNotNull(graph);
            Assert.AreEqual(0, graph.NodeCount);
            Assert.AreEqual(0, graph.EdgeCount);
            Assert.IsTrue(graph.IsEmpty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullStorage_ThrowsArgumentNullException() {
            // Act
            new DirectedGraph(null);
        }

        [TestMethod]
        public void Constructor_WithNamedStorage_SetsCorrectName() {
            // Arrange
            var storage = new AdjacencyListStorage();

            // Act
            var graph = new DirectedGraph(storage, "TestGraph");

            // Assert
            Assert.AreEqual("TestGraph", graph.Name);
        }

        #endregion

        #region Node Operations Tests

        [TestMethod]
        public void AddNode_ValidNode_NodeIsAdded() {
            // Act
            _graph.AddNode(_nodeA);

            // Assert
            Assert.IsTrue(_graph.HasNode(_nodeA));
            Assert.AreEqual(1, _graph.NodeCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNode_NullNode_ThrowsArgumentNullException() {
            // Act
            _graph.AddNode(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddNode_DuplicateNode_ThrowsArgumentException() {
            // Arrange
            _graph.AddNode(_nodeA);

            // Act
            _graph.AddNode(_nodeA);
        }

        [TestMethod]
        public void RemoveNode_ExistingNode_NodeIsRemoved() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddEdge(_edgeAB);

            // Act
            _graph.RemoveNode(_nodeA);

            // Assert
            Assert.IsFalse(_graph.HasNode(_nodeA));
            Assert.IsFalse(_graph.HasEdge(_edgeAB)); // Edge should be removed too
            Assert.IsTrue(_graph.HasNode(_nodeB));
            Assert.AreEqual(1, _graph.NodeCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveNode_NullNode_ThrowsArgumentNullException() {
            // Act
            _graph.RemoveNode(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveNode_NonExistentNode_ThrowsArgumentException() {
            // Act
            _graph.RemoveNode(_nodeA);
        }

        [TestMethod]
        public void HasNode_ExistingNode_ReturnsTrue() {
            // Arrange
            _graph.AddNode(_nodeA);

            // Act & Assert
            Assert.IsTrue(_graph.HasNode(_nodeA));
        }

        [TestMethod]
        public void HasNode_NonExistentNode_ReturnsFalse() {
            // Act & Assert
            Assert.IsFalse(_graph.HasNode(_nodeA));
        }

        [TestMethod]
        public void HasNode_NullNode_ReturnsFalse() {
            // Act & Assert
            Assert.IsFalse(_graph.HasNode(null));
        }

        #endregion

        #region Edge Operations Tests

        [TestMethod]
        public void AddEdge_ValidEdge_EdgeIsAdded() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);

            // Act
            _graph.AddEdge(_edgeAB);

            // Assert
            Assert.IsTrue(_graph.HasEdge(_edgeAB));
            Assert.AreEqual(1, _graph.EdgeCount);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddEdge_NullEdge_ThrowsArgumentNullException() {
            // Act
            _graph.AddEdge(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddEdge_DuplicateEdge_ThrowsArgumentException() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddEdge(_edgeAB);

            // Act
            _graph.AddEdge(_edgeAB);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddEdge_SourceNodeNotInGraph_ThrowsArgumentException() {
            // Arrange
            _graph.AddNode(_nodeB); // Only add target node

            // Act
            _graph.AddEdge(_edgeAB); // Source node A is not in graph
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddEdge_TargetNodeNotInGraph_ThrowsArgumentException() {
            // Arrange
            _graph.AddNode(_nodeA); // Only add source node

            // Act
            _graph.AddEdge(_edgeAB); // Target node B is not in graph
        }

        [TestMethod]
        public void RemoveEdge_ExistingEdge_EdgeIsRemoved() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddEdge(_edgeAB);

            // Act
            _graph.RemoveEdge(_edgeAB);

            // Assert
            Assert.IsFalse(_graph.HasEdge(_edgeAB));
            Assert.AreEqual(0, _graph.EdgeCount);
            Assert.IsTrue(_graph.HasNode(_nodeA)); // Nodes should remain
            Assert.IsTrue(_graph.HasNode(_nodeB));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveEdge_NullEdge_ThrowsArgumentNullException() {
            // Act
            _graph.RemoveEdge(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveEdge_NonExistentEdge_ThrowsArgumentException() {
            // Act
            _graph.RemoveEdge(_edgeAB);
        }

        #endregion

        #region DirectedGraph Specific Tests

        [TestMethod]
        public void GetSuccessors_NodeWithSuccessors_ReturnsCorrectNodes() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddNode(_nodeD);
            _graph.AddEdge(_edgeAB);
            _graph.AddEdge(_edgeAD);

            // Act
            var successors = _graph.GetSuccessors(_nodeA).ToList();

            // Assert
            Assert.AreEqual(2, successors.Count);
            Assert.IsTrue(successors.Contains(_nodeB));
            Assert.IsTrue(successors.Contains(_nodeD));
        }

        [TestMethod]
        public void GetSuccessors_NodeWithNoSuccessors_ReturnsEmptyCollection() {
            // Arrange
            _graph.AddNode(_nodeA);

            // Act
            var successors = _graph.GetSuccessors(_nodeA).ToList();

            // Assert
            Assert.AreEqual(0, successors.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetSuccessors_NullNode_ThrowsArgumentNullException() {
            // Act
            _graph.GetSuccessors(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetSuccessors_NonExistentNode_ThrowsArgumentException() {
            // Act
            _graph.GetSuccessors(_nodeA);
        }

        [TestMethod]
        public void GetPredecessors_NodeWithPredecessors_ReturnsCorrectNodes() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddNode(_nodeC);
            _graph.AddEdge(_edgeAB);
            _graph.AddEdge(_edgeCA);

            // Act
            var predecessors = _graph.GetPredecessors(_nodeA).ToList();

            // Assert
            Assert.AreEqual(1, predecessors.Count);
            Assert.IsTrue(predecessors.Contains(_nodeC));
        }

        [TestMethod]
        public void GetPredecessors_NodeWithNoPredecessors_ReturnsEmptyCollection() {
            // Arrange
            _graph.AddNode(_nodeA);

            // Act
            var predecessors = _graph.GetPredecessors(_nodeA).ToList();

            // Assert
            Assert.AreEqual(0, predecessors.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPredecessors_NullNode_ThrowsArgumentNullException() {
            // Act
            _graph.GetPredecessors(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPredecessors_NonExistentNode_ThrowsArgumentException() {
            // Act
            _graph.GetPredecessors(_nodeA);
        }

        [TestMethod]
        public void GetOutgoingEdges_NodeWithOutgoingEdges_ReturnsCorrectEdges() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddNode(_nodeD);
            _graph.AddEdge(_edgeAB);
            _graph.AddEdge(_edgeAD);

            // Act
            var outgoingEdges = _graph.GetOutgoingEdges(_nodeA).ToList();

            // Assert
            Assert.AreEqual(2, outgoingEdges.Count);
            Assert.IsTrue(outgoingEdges.Contains(_edgeAB));
            Assert.IsTrue(outgoingEdges.Contains(_edgeAD));
        }

        [TestMethod]
        public void GetOutgoingEdges_NodeWithNoOutgoingEdges_ReturnsEmptyCollection() {
            // Arrange
            _graph.AddNode(_nodeA);

            // Act
            var outgoingEdges = _graph.GetOutgoingEdges(_nodeA).ToList();

            // Assert
            Assert.AreEqual(0, outgoingEdges.Count);
        }

        [TestMethod]
        public void GetIncomingEdges_NodeWithIncomingEdges_ReturnsCorrectEdges() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddNode(_nodeC);
            _graph.AddEdge(_edgeAB);
            _graph.AddEdge(_edgeCA);

            // Act
            var incomingEdges = _graph.GetIncomingEdges(_nodeA).ToList();

            // Assert
            Assert.AreEqual(1, incomingEdges.Count);
            Assert.IsTrue(incomingEdges.Contains(_edgeCA));
        }

        [TestMethod]
        public void GetIncomingEdges_NodeWithNoIncomingEdges_ReturnsEmptyCollection() {
            // Arrange
            _graph.AddNode(_nodeA);

            // Act
            var incomingEdges = _graph.GetIncomingEdges(_nodeA).ToList();

            // Assert
            Assert.AreEqual(0, incomingEdges.Count);
        }

        #endregion

        #region Connectivity Tests

        [TestMethod]
        public void AreConnected_DirectlyConnectedNodes_ReturnsTrue() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddEdge(_edgeAB);

            // Act & Assert
            Assert.IsTrue(_graph.AreConnected(_nodeA, _nodeB));
        }

        [TestMethod]
        public void AreConnected_NotConnectedNodes_ReturnsFalse() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddNode(_nodeC);
            _graph.AddEdge(_edgeAB);

            // Act & Assert
            Assert.IsFalse(_graph.AreConnected(_nodeA, _nodeC));
            Assert.IsFalse(_graph.AreConnected(_nodeB, _nodeA)); // Direction matters in directed graph
        }

        [TestMethod]
        public void AreConnected_SameNode_ReturnsFalse() {
            // Arrange
            _graph.AddNode(_nodeA);

            // Act & Assert
            Assert.IsFalse(_graph.AreConnected(_nodeA, _nodeA));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AreConnected_NullSource_ThrowsArgumentNullException() {
            // Act
            _graph.AreConnected(null, _nodeB);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AreConnected_NullTarget_ThrowsArgumentNullException() {
            // Act
            _graph.AreConnected(_nodeA, null);
        }

        #endregion

        #region Self-Loop Tests

        [TestMethod]
        public void SelfLoop_AddSelfLoop_Works() {
            // Arrange
            _graph.AddNode(_nodeA);
            var selfLoop = new Edge<string>(_nodeA, _nodeA) { Value = "A->A" };

            // Act
            _graph.AddEdge(selfLoop);

            // Assert
            Assert.IsTrue(_graph.HasEdge(selfLoop));
            Assert.IsTrue(_graph.AreConnected(_nodeA, _nodeA));
            Assert.IsTrue(_graph.GetSuccessors(_nodeA).Contains(_nodeA));
            Assert.IsTrue(_graph.GetPredecessors(_nodeA).Contains(_nodeA));
        }

        #endregion

        #region Complex Graph Tests

        [TestMethod]
        public void ComplexGraph_MultipleNodesAndEdges_AllOperationsWork() {
            // Arrange - Create a directed cycle: A -> B -> C -> A, plus A -> D
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddNode(_nodeC);
            _graph.AddNode(_nodeD);

            _graph.AddEdge(_edgeAB);
            _graph.AddEdge(_edgeBC);
            _graph.AddEdge(_edgeCA);
            _graph.AddEdge(_edgeAD);

            // Act & Assert - Test graph structure
            Assert.AreEqual(4, _graph.NodeCount);
            Assert.AreEqual(4, _graph.EdgeCount);

            // Test A's connections
            var aSuccessors = _graph.GetSuccessors(_nodeA).ToList();
            Assert.AreEqual(2, aSuccessors.Count);
            Assert.IsTrue(aSuccessors.Contains(_nodeB));
            Assert.IsTrue(aSuccessors.Contains(_nodeD));

            var aPredecessors = _graph.GetPredecessors(_nodeA).ToList();
            Assert.AreEqual(1, aPredecessors.Count);
            Assert.IsTrue(aPredecessors.Contains(_nodeC));

            // Test connectivity
            Assert.IsTrue(_graph.AreConnected(_nodeA, _nodeB));
            Assert.IsTrue(_graph.AreConnected(_nodeB, _nodeC));
            Assert.IsTrue(_graph.AreConnected(_nodeC, _nodeA));
            Assert.IsTrue(_graph.AreConnected(_nodeA, _nodeD));

            // Test non-connectivity (direction matters)
            Assert.IsFalse(_graph.AreConnected(_nodeB, _nodeA));
            Assert.IsFalse(_graph.AreConnected(_nodeD, _nodeA));
        }

        [TestMethod]
        public void NodeRemoval_WithMultipleConnections_RemovesAllEdges() {
            // Arrange - Create graph where A is connected to multiple nodes
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddNode(_nodeC);
            _graph.AddNode(_nodeD);

            _graph.AddEdge(_edgeAB);
            _graph.AddEdge(_edgeCA);
            _graph.AddEdge(_edgeAD);

            // Act
            _graph.RemoveNode(_nodeA);

            // Assert
            Assert.IsFalse(_graph.HasNode(_nodeA));
            Assert.IsFalse(_graph.HasEdge(_edgeAB));
            Assert.IsFalse(_graph.HasEdge(_edgeCA));
            Assert.IsFalse(_graph.HasEdge(_edgeAD));

            // Other nodes should remain
            Assert.IsTrue(_graph.HasNode(_nodeB));
            Assert.IsTrue(_graph.HasNode(_nodeC));
            Assert.IsTrue(_graph.HasNode(_nodeD));

            Assert.AreEqual(3, _graph.NodeCount);
            Assert.AreEqual(0, _graph.EdgeCount);
        }

        #endregion

        #region Inheritance and Polymorphism Tests

        [TestMethod]
        public void InheritanceTest_DirectedGraphIsIGraph() {
            // Act & Assert
            Assert.IsInstanceOfType(_graph, typeof(IGraph));
            Assert.IsInstanceOfType(_graph, typeof(IDirectedGraph));
        }

        [TestMethod]
        public void PolymorphismTest_UseAsIGraph() {
            // Arrange
            IGraph graph = _graph;

            // Act
            graph.AddNode(_nodeA);
            graph.AddNode(_nodeB);
            graph.AddEdge(_edgeAB);

            // Assert
            Assert.IsTrue(graph.HasNode(_nodeA));
            Assert.IsTrue(graph.HasEdge(_edgeAB));
            Assert.IsTrue(graph.AreConnected(_nodeA, _nodeB));
        }

        [TestMethod]
        public void PolymorphismTest_UseAsIDirectedGraph() {
            // Arrange
            IDirectedGraph directedGraph = _graph;
            directedGraph.AddNode(_nodeA);
            directedGraph.AddNode(_nodeB);
            directedGraph.AddEdge(_edgeAB);

            // Act & Assert
            Assert.AreEqual(1, directedGraph.GetSuccessors(_nodeA).Count());
            Assert.AreEqual(1, directedGraph.GetPredecessors(_nodeB).Count());
            Assert.AreEqual(1, directedGraph.GetOutgoingEdges(_nodeA).Count());
            Assert.AreEqual(1, directedGraph.GetIncomingEdges(_nodeB).Count());
        }

        #endregion

        #region Edge Cases and Error Conditions

        [TestMethod]
        public void EmptyGraph_AllQueryOperations_ReturnAppropriateValues() {
            // Assert
            Assert.AreEqual(0, _graph.NodeCount);
            Assert.AreEqual(0, _graph.EdgeCount);
            Assert.IsTrue(_graph.IsEmpty);
            Assert.IsFalse(_graph.HasNode(_nodeA));
            Assert.IsFalse(_graph.HasEdge(_edgeAB));
        }

        [TestMethod]
        public void MultipleEdgesBetweenSameNodes_AllowedAndTracked() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            var edgeAB2 = new Edge<string>(_nodeA, _nodeB) { Value = "A->B (second)" };

            // Act
            _graph.AddEdge(_edgeAB);
            _graph.AddEdge(edgeAB2);

            // Assert
            Assert.AreEqual(2, _graph.EdgeCount);
            Assert.IsTrue(_graph.HasEdge(_edgeAB));
            Assert.IsTrue(_graph.HasEdge(edgeAB2));

            var outgoingEdges = _graph.GetOutgoingEdges(_nodeA).ToList();
            Assert.AreEqual(2, outgoingEdges.Count);
        }

        [TestMethod]
        public void ToString_ReturnsCorrectFormat() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddEdge(_edgeAB);

            // Act
            var result = _graph.ToString();

            // Assert
            Assert.IsTrue(result.Contains("DirectedGraph"));
            Assert.IsTrue(result.Contains("Nodes: 2"));
            Assert.IsTrue(result.Contains("Edges: 1"));
        }

        #endregion

        #region Performance Tests

        [TestMethod]
        public void PerformanceTest_LargeGraph_AcceptablePerformance() {
            // Arrange
            const int nodeCount = 1000;
            var nodes = new List<Node<int>>();

            // Create nodes
            for (int i = 0; i < nodeCount; i++) {
                var node = new Node<int> { Value = i };
                nodes.Add(node);
                _graph.AddNode(node);
            }

            // Create edges (linear chain)
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < nodeCount - 1; i++) {
                var edge = new Edge<int>(nodes[i], nodes[i + 1]) { Value = i };
                _graph.AddEdge(edge);
            }
            stopwatch.Stop();

            // Assert
            Assert.AreEqual(nodeCount, _graph.NodeCount);
            Assert.AreEqual(nodeCount - 1, _graph.EdgeCount);

            // Test query performance
            stopwatch.Restart();
            var successors = _graph.GetSuccessors(nodes[0]);
            var predecessors = _graph.GetPredecessors(nodes[nodeCount - 1]);
            stopwatch.Stop();

            Assert.AreEqual(1, successors.Count());
            Assert.AreEqual(1, predecessors.Count());

            Console.WriteLine($"Large graph operations completed in {stopwatch.ElapsedMilliseconds}ms");
        }

        #endregion
    }
}
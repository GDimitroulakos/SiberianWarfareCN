using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Graph.Tests {

    [TestClass]
    public class UndirectedGraphTests {

        private UnDirectedGraph _graph;
        private IGraphStorage _storage;
        private INode _nodeA;
        private INode _nodeB;
        private INode _nodeC;
        private INode _nodeD;
        private IEdge _edgeAB;
        private IEdge _edgeBC;
        private IEdge _edgeCA;
        private IEdge _edgeAD;
        private IEdge _edgeBA; // Reverse edge for testing bidirectional behavior

        [TestInitialize]
        public void Setup() {
            _storage = new AdjacencyListStorage();
            _graph = new UnDirectedGraph(_storage);

            // Create test nodes
            _nodeA = new Node<string> { Value = "A" };
            _nodeB = new Node<string> { Value = "B" };
            _nodeC = new Node<string> { Value = "C" };
            _nodeD = new Node<string> { Value = "D" };

            // Create test edges
            _edgeAB = new Edge<string>(_nodeA, _nodeB) { Value = "A-B" };
            _edgeBC = new Edge<string>(_nodeB, _nodeC) { Value = "B-C" };
            _edgeCA = new Edge<string>(_nodeC, _nodeA) { Value = "C-A" };
            _edgeAD = new Edge<string>(_nodeA, _nodeD) { Value = "A-D" };
            _edgeBA = new Edge<string>(_nodeB, _nodeA) { Value = "B-A" };
        }

        #region Constructor Tests

        [TestMethod]
        public void Constructor_ValidStorage_CreatesUndirectedGraph() {
            // Arrange
            var storage = new AdjacencyListStorage();

            // Act
            var graph = new UnDirectedGraph(storage);

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
            new UnDirectedGraph(null);
        }

        [TestMethod]
        public void Constructor_WithNamedStorage_SetsCorrectName() {
            // Arrange
            var storage = new AdjacencyListStorage();

            // Act
            var graph = new UnDirectedGraph(storage, "FriendshipNetwork");

            // Assert
            Assert.AreEqual("FriendshipNetwork", graph.Name);
        }

        #endregion

        #region Node Operations Tests (Inherited from BaseGraph)

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
            Assert.IsFalse(_graph.HasEdge(_edgeAB));
            Assert.IsTrue(_graph.HasNode(_nodeB));
            Assert.AreEqual(1, _graph.NodeCount);
        }

        #endregion

        #region Edge Operations Tests (Inherited from BaseGraph)

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
        [ExpectedException(typeof(ArgumentException))]
        public void AddEdge_NodesNotInGraph_ThrowsArgumentException() {
            // Act - Neither node is in the graph
            _graph.AddEdge(_edgeAB);
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
        }

        #endregion

        #region UndirectedGraph Specific Tests

        [TestMethod]
        public void GetNeighbors_NodeWithNeighbors_ReturnsAllConnectedNodes() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddNode(_nodeC);
            _graph.AddNode(_nodeD);

            // Add edges: A-B, A-C, D-A (all should be treated as undirected)
            _graph.AddEdge(_edgeAB); // A -> B
            _graph.AddEdge(_edgeCA); // C -> A  
            _graph.AddEdge(_edgeAD); // A -> D

            // Act
            var neighbors = _graph.GetNeighbors(_nodeA).ToList();

            // Assert
            Assert.AreEqual(3, neighbors.Count);
            Assert.IsTrue(neighbors.Contains(_nodeB)); // From outgoing edge A->B
            Assert.IsTrue(neighbors.Contains(_nodeC)); // From incoming edge C->A
            Assert.IsTrue(neighbors.Contains(_nodeD)); // From outgoing edge A->D
        }

        [TestMethod]
        public void GetNeighbors_NodeWithNoNeighbors_ReturnsEmptyCollection() {
            // Arrange
            _graph.AddNode(_nodeA);

            // Act
            var neighbors = _graph.GetNeighbors(_nodeA).ToList();

            // Assert
            Assert.AreEqual(0, neighbors.Count);
        }

        [TestMethod]
        public void GetNeighbors_NodeWithBidirectionalEdges_ReturnsUniqueNeighbors() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddEdge(_edgeAB); // A -> B
            _graph.AddEdge(_edgeBA); // B -> A

            // Act
            var neighborsA = _graph.GetNeighbors(_nodeA).ToList();
            var neighborsB = _graph.GetNeighbors(_nodeB).ToList();

            // Assert - Should only contain B once, despite two edges
            Assert.AreEqual(1, neighborsA.Count);
            Assert.IsTrue(neighborsA.Contains(_nodeB));

            Assert.AreEqual(1, neighborsB.Count);
            Assert.IsTrue(neighborsB.Contains(_nodeA));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNeighbors_NullNode_ThrowsArgumentNullException() {
            // Act
            _graph.GetNeighbors(null);
        }

        [TestMethod]
        public void GetEdges_NodeWithIncidentEdges_ReturnsAllEdges() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddNode(_nodeC);

            _graph.AddEdge(_edgeAB); // A -> B (outgoing from A)
            _graph.AddEdge(_edgeCA); // C -> A (incoming to A)

            // Act
            var edgesA = _graph.GetEdges(_nodeA).ToList();

            // Assert
            Assert.AreEqual(2, edgesA.Count);
            Assert.IsTrue(edgesA.Contains(_edgeAB)); // Outgoing edge
            Assert.IsTrue(edgesA.Contains(_edgeCA)); // Incoming edge
        }

        [TestMethod]
        public void GetEdges_NodeWithNoEdges_ReturnsEmptyCollection() {
            // Arrange
            _graph.AddNode(_nodeA);

            // Act
            var edges = _graph.GetEdges(_nodeA).ToList();

            // Assert
            Assert.AreEqual(0, edges.Count);
        }

        [TestMethod]
        public void GetEdges_NodeWithBidirectionalEdges_ReturnsAllEdges() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddEdge(_edgeAB); // A -> B
            _graph.AddEdge(_edgeBA); // B -> A

            // Act
            var edgesA = _graph.GetEdges(_nodeA).ToList();

            // Assert - Should return both edges (different edge objects)
            Assert.AreEqual(2, edgesA.Count);
            Assert.IsTrue(edgesA.Contains(_edgeAB));
            Assert.IsTrue(edgesA.Contains(_edgeBA));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetEdges_NullNode_ThrowsArgumentNullException() {
            // Act
            _graph.GetEdges(null);
        }

        #endregion

        #region Undirected Connectivity Tests

        [TestMethod]
        public void AreConnected_DirectEdge_ReturnsTrue() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddEdge(_edgeAB); // A -> B

            // Act & Assert - Both directions should return true in undirected graph
            Assert.IsTrue(_graph.AreConnected(_nodeA, _nodeB));
            Assert.IsTrue(_graph.AreConnected(_nodeB, _nodeA)); // Key difference from directed graph
        }

        [TestMethod]
        public void AreConnected_ReverseEdge_ReturnsTrue() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddEdge(_edgeBA); // B -> A

            // Act & Assert - Both directions should return true
            Assert.IsTrue(_graph.AreConnected(_nodeA, _nodeB));
            Assert.IsTrue(_graph.AreConnected(_nodeB, _nodeA));
        }

        [TestMethod]
        public void AreConnected_NotConnectedNodes_ReturnsFalse() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddNode(_nodeC);
            _graph.AddEdge(_edgeAB); // Only A-B connected

            // Act & Assert
            Assert.IsFalse(_graph.AreConnected(_nodeA, _nodeC));
            Assert.IsFalse(_graph.AreConnected(_nodeC, _nodeA));
        }

        [TestMethod]
        public void AreConnected_SelfLoop_ReturnsTrue() {
            // Arrange
            _graph.AddNode(_nodeA);
            var selfLoop = new Edge<string>(_nodeA, _nodeA) { Value = "A-A" };
            _graph.AddEdge(selfLoop);

            // Act & Assert
            Assert.IsTrue(_graph.AreConnected(_nodeA, _nodeA));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AreConnected_NullNodes_ThrowsArgumentNullException() {
            // Act
            _graph.AreConnected(null, _nodeB);
        }

        #endregion

        #region Complex Undirected Graph Tests

        [TestMethod]
        public void Triangle_AllNodesConnected_AllPairsConnected() {
            // Arrange - Create triangle: A-B-C-A
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddNode(_nodeC);

            _graph.AddEdge(_edgeAB); // A -> B
            _graph.AddEdge(_edgeBC); // B -> C
            _graph.AddEdge(_edgeCA); // C -> A

            // Act & Assert - All pairs should be connected in undirected interpretation
            Assert.IsTrue(_graph.AreConnected(_nodeA, _nodeB));
            Assert.IsTrue(_graph.AreConnected(_nodeB, _nodeA));

            Assert.IsTrue(_graph.AreConnected(_nodeB, _nodeC));
            Assert.IsTrue(_graph.AreConnected(_nodeC, _nodeB));

            Assert.IsTrue(_graph.AreConnected(_nodeC, _nodeA));
            Assert.IsTrue(_graph.AreConnected(_nodeA, _nodeC));

            // Test neighbors
            var neighborsA = _graph.GetNeighbors(_nodeA).ToList();
            Assert.AreEqual(2, neighborsA.Count);
            Assert.IsTrue(neighborsA.Contains(_nodeB));
            Assert.IsTrue(neighborsA.Contains(_nodeC));
        }

        [TestMethod]
        public void Star_CenterNodeConnectedToAll_CorrectNeighborCounts() {
            // Arrange - Create star pattern: B, C, D all connected to A
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddNode(_nodeC);
            _graph.AddNode(_nodeD);

            _graph.AddEdge(_edgeAB); // A -> B
            _graph.AddEdge(_edgeCA); // C -> A
            _graph.AddEdge(_edgeAD); // A -> D

            // Act
            var neighborsA = _graph.GetNeighbors(_nodeA).ToList();
            var neighborsB = _graph.GetNeighbors(_nodeB).ToList();
            var neighborsC = _graph.GetNeighbors(_nodeC).ToList();
            var neighborsD = _graph.GetNeighbors(_nodeD).ToList();

            // Assert
            Assert.AreEqual(3, neighborsA.Count); // A connected to B, C, D
            Assert.AreEqual(1, neighborsB.Count); // B only connected to A
            Assert.AreEqual(1, neighborsC.Count); // C only connected to A
            Assert.AreEqual(1, neighborsD.Count); // D only connected to A

            // Test specific connections
            Assert.IsTrue(_graph.AreConnected(_nodeA, _nodeB));
            Assert.IsTrue(_graph.AreConnected(_nodeB, _nodeA));
            Assert.IsTrue(_graph.AreConnected(_nodeA, _nodeC));
            Assert.IsTrue(_graph.AreConnected(_nodeC, _nodeA));
            Assert.IsTrue(_graph.AreConnected(_nodeA, _nodeD));
            Assert.IsTrue(_graph.AreConnected(_nodeD, _nodeA));

            // Non-connected pairs
            Assert.IsFalse(_graph.AreConnected(_nodeB, _nodeC));
            Assert.IsFalse(_graph.AreConnected(_nodeC, _nodeD));
        }

        [TestMethod]
        public void BidirectionalEdges_TreatedAsUndirected_CorrectBehavior() {
            // Arrange - Add both directions explicitly
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            _graph.AddEdge(_edgeAB); // A -> B
            _graph.AddEdge(_edgeBA); // B -> A

            // Act
            var neighborsA = _graph.GetNeighbors(_nodeA).ToList();
            var neighborsB = _graph.GetNeighbors(_nodeB).ToList();
            var edgesA = _graph.GetEdges(_nodeA).ToList();

            // Assert
            Assert.AreEqual(1, neighborsA.Count); // Only B (no duplicates)
            Assert.AreEqual(1, neighborsB.Count); // Only A (no duplicates)
            Assert.AreEqual(2, edgesA.Count);     // Both edge objects present

            Assert.IsTrue(_graph.AreConnected(_nodeA, _nodeB));
            Assert.IsTrue(_graph.AreConnected(_nodeB, _nodeA));
        }

        #endregion

        #region Node Removal in Undirected Context

        [TestMethod]
        public void RemoveNode_CenterOfStar_AllEdgesRemoved() {
            // Arrange - Create star with A at center
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
            Assert.AreEqual(0, _graph.EdgeCount); // All edges removed

            // Remaining nodes should have no neighbors
            Assert.AreEqual(0, _graph.GetNeighbors(_nodeB).Count());
            Assert.AreEqual(0, _graph.GetNeighbors(_nodeC).Count());
            Assert.AreEqual(0, _graph.GetNeighbors(_nodeD).Count());
        }

        #endregion

        #region Inheritance and Polymorphism Tests

        [TestMethod]
        public void InheritanceTest_UndirectedGraphIsIGraph() {
            // Act & Assert
            Assert.IsInstanceOfType(_graph, typeof(IGraph));
            Assert.IsInstanceOfType(_graph, typeof(IUndirectedGraph));
            Assert.IsInstanceOfType(_graph, typeof(BaseGraph));
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
            Assert.IsTrue(graph.AreConnected(_nodeB, _nodeA)); // Undirected behavior
        }

        [TestMethod]
        public void PolymorphismTest_UseAsIUndirectedGraph() {
            // Arrange
            IUndirectedGraph undirectedGraph = _graph;
            undirectedGraph.AddNode(_nodeA);
            undirectedGraph.AddNode(_nodeB);
            undirectedGraph.AddEdge(_edgeAB);

            // Act & Assert
            Assert.AreEqual(1, undirectedGraph.GetNeighbors(_nodeA).Count());
            Assert.AreEqual(1, undirectedGraph.GetNeighbors(_nodeB).Count());
            Assert.AreEqual(1, undirectedGraph.GetEdges(_nodeA).Count());
            Assert.AreEqual(1, undirectedGraph.GetEdges(_nodeB).Count());
        }

        #endregion

        #region Comparison with DirectedGraph Behavior

        [TestMethod]
        public void ComparisonWithDirected_SameData_DifferentConnectivity() {
            // Arrange - Create same structure in both graph types
            var directedStorage = new AdjacencyListStorage();
            var directedGraph = new DirectedGraph(directedStorage);

            // Add same nodes and edges to both graphs
            foreach (var node in new[] { _nodeA, _nodeB, _nodeC }) {
                _graph.AddNode(node);
                directedGraph.AddNode(node);
            }

            _graph.AddEdge(_edgeAB);      // A -> B
            directedGraph.AddEdge(_edgeAB);

            // Act & Assert - Different connectivity behavior
            Assert.IsTrue(_graph.AreConnected(_nodeA, _nodeB));
            Assert.IsTrue(_graph.AreConnected(_nodeB, _nodeA)); // Undirected: both ways

            Assert.IsTrue(directedGraph.AreConnected(_nodeA, _nodeB));
            Assert.IsFalse(directedGraph.AreConnected(_nodeB, _nodeA)); // Directed: one way only
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
        public void IsolatedNodes_NoNeighbors_EmptyCollections() {
            // Arrange
            _graph.AddNode(_nodeA);
            _graph.AddNode(_nodeB);
            // No edges added

            // Act & Assert
            Assert.AreEqual(0, _graph.GetNeighbors(_nodeA).Count());
            Assert.AreEqual(0, _graph.GetNeighbors(_nodeB).Count());
            Assert.AreEqual(0, _graph.GetEdges(_nodeA).Count());
            Assert.AreEqual(0, _graph.GetEdges(_nodeB).Count());
            Assert.IsFalse(_graph.AreConnected(_nodeA, _nodeB));
        }

        [TestMethod]
        public void SelfLoop_NodeIsItsOwnNeighbor() {
            // Arrange
            _graph.AddNode(_nodeA);
            var selfLoop = new Edge<string>(_nodeA, _nodeA) { Value = "A-A" };
            _graph.AddEdge(selfLoop);

            // Act
            var neighbors = _graph.GetNeighbors(_nodeA).ToList();
            var edges = _graph.GetEdges(_nodeA).ToList();

            // Assert
            Assert.AreEqual(1, neighbors.Count);
            Assert.IsTrue(neighbors.Contains(_nodeA));
            Assert.AreEqual(2, edges.Count); // Self-loop appears in both outgoing and incoming
            Assert.IsTrue(_graph.AreConnected(_nodeA, _nodeA));
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
            Assert.IsTrue(result.Contains("UnDirectedGraph"));
            Assert.IsTrue(result.Contains("Nodes: 2"));
            Assert.IsTrue(result.Contains("Edges: 1"));
        }

        #endregion

        #region Performance Tests

        [TestMethod]
        public void PerformanceTest_LargeUndirectedGraph_AcceptablePerformance() {
            // Arrange
            const int nodeCount = 500;
            var nodes = new List<Node<int>>();

            // Create nodes
            for (int i = 0; i < nodeCount; i++) {
                var node = new Node<int> { Value = i };
                nodes.Add(node);
                _graph.AddNode(node);
            }

            // Create edges (create a connected graph - each node connected to next)
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < nodeCount - 1; i++) {
                var edge = new Edge<int>(nodes[i], nodes[i + 1]) { Value = i };
                _graph.AddEdge(edge);
            }
            stopwatch.Stop();

            // Assert
            Assert.AreEqual(nodeCount, _graph.NodeCount);
            Assert.AreEqual(nodeCount - 1, _graph.EdgeCount);

            // Test neighbor queries
            stopwatch.Restart();
            var firstNodeNeighbors = _graph.GetNeighbors(nodes[0]);
            var middleNodeNeighbors = _graph.GetNeighbors(nodes[nodeCount / 2]);
            var lastNodeNeighbors = _graph.GetNeighbors(nodes[nodeCount - 1]);
            stopwatch.Stop();

            Assert.AreEqual(1, firstNodeNeighbors.Count()); // Connected to second node
            Assert.AreEqual(2, middleNodeNeighbors.Count()); // Connected to previous and next
            Assert.AreEqual(1, lastNodeNeighbors.Count()); // Connected to previous node

            Console.WriteLine($"Large undirected graph operations completed in {stopwatch.ElapsedMilliseconds}ms");
        }

        #endregion
    }
}
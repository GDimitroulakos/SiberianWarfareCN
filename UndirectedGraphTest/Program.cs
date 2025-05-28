using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Graph.Tests {

    [TestClass]
    public class UndirectedGraphTests {
        private UnDirectedGraph graph;
        private UndirectedAdjacencyListStorage storage;
        private Node<int> node1;
        private Node<int> node2;
        private Node<int> node3;
        private Node<int> node4;
        private Node<int> node5;
        private Edge<string> edge1;
        private Edge<string> edge2;
        private Edge<string> edge3;

        [TestInitialize]
        public void Setup() {
            storage = new UndirectedAdjacencyListStorage();
            graph = new UnDirectedGraph(storage);

            node1 = new Node<int>();
            node2 = new Node<int>();
            node3 = new Node<int>();
            node4 = new Node<int>();
            node5 = new Node<int>();
        }

        private void SetupLinearGraph() {
            // Create a linear undirected graph: node1 -- node2 -- node3
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);

            edge1 = new Edge<string>(node1, node2);
            edge2 = new Edge<string>(node2, node3);

            graph.AddEdge(edge1);
            graph.AddEdge(edge2);
        }

        private void SetupStarGraph() {
            // Create a star graph with node1 at center: node2 -- node1 -- node3
            //                                                    |
            //                                                  node4
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);
            graph.AddNode(node4);

            edge1 = new Edge<string>(node1, node2);
            edge2 = new Edge<string>(node1, node3);
            edge3 = new Edge<string>(node1, node4);

            graph.AddEdge(edge1);
            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
        }

        private void SetupComplexGraph() {
            // Create a more complex undirected graph:
            // node1 -- node2 -- node3
            //   |        |       |
            // node4 -- node5 ----+
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);
            graph.AddNode(node4);
            graph.AddNode(node5);

            edge1 = new Edge<string>(node1, node2);
            edge2 = new Edge<string>(node2, node3);
            edge3 = new Edge<string>(node1, node4);
            var edge4 = new Edge<string>(node4, node5);
            var edge5 = new Edge<string>(node2, node5);
            var edge6 = new Edge<string>(node3, node5);

            graph.AddEdge(edge1);
            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
            graph.AddEdge(edge4);
            graph.AddEdge(edge5);
            graph.AddEdge(edge6);
        }

        #region Constructor Tests
        [TestMethod]
        public void Constructor_ValidStorage_ShouldCreateGraph() {
            // Act
            var testGraph = new UnDirectedGraph(storage);

            // Assert
            Assert.IsNotNull(testGraph);
            Assert.AreEqual(0, testGraph.NodeCount);
            Assert.AreEqual(0, testGraph.EdgeCount);
            Assert.IsTrue(testGraph.IsEmpty);
        }

        [TestMethod]
        public void Constructor_WithName_ShouldSetName() {
            // Act
            var testGraph = new UnDirectedGraph(storage, "TestUndirectedGraph");

            // Assert
            Assert.AreEqual("TestUndirectedGraph", testGraph.Name);
        }

        [TestMethod]
        public void Constructor_WithoutName_ShouldGenerateDefaultName() {
            // Act
            var testGraph = new UnDirectedGraph(storage);

            // Assert
            Assert.IsTrue(testGraph.Name.StartsWith("UnDirectedGraph_"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullStorage_ShouldThrowArgumentNullException() {
            // Act
            new UnDirectedGraph(null);
        }
        #endregion

        #region Basic Graph Operations Tests
        [TestMethod]
        public void AddNode_ValidNode_ShouldAddSuccessfully() {
            // Act
            graph.AddNode(node1);

            // Assert
            Assert.IsTrue(graph.HasNode(node1));
            Assert.AreEqual(1, graph.NodeCount);
            Assert.IsFalse(graph.IsEmpty);
        }

        [TestMethod]
        public void AddEdge_ValidEdge_ShouldAddSuccessfully() {
            // Arrange
            graph.AddNode(node1);
            graph.AddNode(node2);
            var edge = new Edge<string>(node1, node2);

            // Act
            graph.AddEdge(edge);

            // Assert
            Assert.IsTrue(graph.HasEdge(edge));
            Assert.AreEqual(1, graph.EdgeCount);
        }

        [TestMethod]
        public void RemoveNode_ValidNode_ShouldRemoveSuccessfully() {
            // Arrange
            graph.AddNode(node1);
            graph.AddNode(node2);
            var edge = new Edge<string>(node1, node2);
            graph.AddEdge(edge);

            // Act
            graph.RemoveNode(node1);

            // Assert
            Assert.IsFalse(graph.HasNode(node1));
            Assert.IsFalse(graph.HasEdge(edge));
            Assert.AreEqual(1, graph.NodeCount);
            Assert.AreEqual(0, graph.EdgeCount);
        }

        [TestMethod]
        public void RemoveEdge_ValidEdge_ShouldRemoveSuccessfully() {
            // Arrange
            graph.AddNode(node1);
            graph.AddNode(node2);
            var edge = new Edge<string>(node1, node2);
            graph.AddEdge(edge);

            // Act
            graph.RemoveEdge(edge);

            // Assert
            Assert.IsFalse(graph.HasEdge(edge));
            Assert.AreEqual(0, graph.EdgeCount);
            Assert.AreEqual(2, graph.NodeCount); // Nodes should remain
        }
        #endregion

        #region AreConnected Tests (Undirected Specific)
        [TestMethod]
        public void AreConnected_DirectlyConnectedNodes_ShouldReturnTrueBothDirections() {
            // Arrange
            graph.AddNode(node1);
            graph.AddNode(node2);
            var edge = new Edge<string>(node1, node2);
            graph.AddEdge(edge);

            // Act & Assert - Should work both ways in undirected graph
            Assert.IsTrue(graph.AreConnected(node1, node2));
            Assert.IsTrue(graph.AreConnected(node2, node1));
        }

        [TestMethod]
        public void AreConnected_NotConnectedNodes_ShouldReturnFalse() {
            // Arrange
            SetupLinearGraph();

            // Act & Assert - node1 and node3 are not directly connected
            Assert.IsFalse(graph.AreConnected(node1, node3));
            Assert.IsFalse(graph.AreConnected(node3, node1));
        }

        [TestMethod]
        public void AreConnected_SameNode_ShouldReturnFalseUnlessSelfLoop() {
            // Arrange
            graph.AddNode(node1);

            // Act & Assert - No self-loop
            Assert.IsFalse(graph.AreConnected(node1, node1));
        }

        [TestMethod]
        public void AreConnected_SelfLoop_ShouldReturnTrue() {
            // Arrange
            graph.AddNode(node1);
            var selfLoop = new Edge<string>(node1, node1);
            graph.AddEdge(selfLoop);

            // Act & Assert
            Assert.IsTrue(graph.AreConnected(node1, node1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AreConnected_NullSource_ShouldThrowArgumentNullException() {
            // Act
            graph.AreConnected(null, node2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AreConnected_NullTarget_ShouldThrowArgumentNullException() {
            // Act
            graph.AreConnected(node1, null);
        }
        #endregion

        #region GetNeighbors Tests
        [TestMethod]
        public void GetNeighbors_NodeWithNeighbors_ShouldReturnAllAdjacentNodes() {
            // Arrange
            SetupStarGraph();

            // Act
            var neighbors = graph.GetNeighbors(node1).ToList();

            // Assert - Center node should have 3 neighbors
            Assert.AreEqual(3, neighbors.Count);
            Assert.IsTrue(neighbors.Contains(node2));
            Assert.IsTrue(neighbors.Contains(node3));
            Assert.IsTrue(neighbors.Contains(node4));
        }

        [TestMethod]
        public void GetNeighbors_LeafNode_ShouldReturnSingleNeighbor() {
            // Arrange
            SetupStarGraph();

            // Act
            var neighbors = graph.GetNeighbors(node2).ToList();

            // Assert - Leaf node should have only center as neighbor
            Assert.AreEqual(1, neighbors.Count);
            Assert.IsTrue(neighbors.Contains(node1));
        }

        [TestMethod]
        public void GetNeighbors_NodeWithNoNeighbors_ShouldReturnEmptyCollection() {
            // Arrange
            graph.AddNode(node1);

            // Act
            var neighbors = graph.GetNeighbors(node1).ToList();

            // Assert
            Assert.AreEqual(0, neighbors.Count);
        }

        [TestMethod]
        public void GetNeighbors_ComplexGraph_ShouldReturnCorrectNeighbors() {
            // Arrange
            SetupComplexGraph();

            // Act
            var neighborsNode2 = graph.GetNeighbors(node2).ToList();
            var neighborsNode5 = graph.GetNeighbors(node5).ToList();

            // Assert
            // node2 should be connected to node1, node3, and node5
            Assert.AreEqual(3, neighborsNode2.Count);
            Assert.IsTrue(neighborsNode2.Contains(node1));
            Assert.IsTrue(neighborsNode2.Contains(node3));
            Assert.IsTrue(neighborsNode2.Contains(node5));

            // node5 should be connected to node2, node3, and node4
            Assert.AreEqual(3, neighborsNode5.Count);
            Assert.IsTrue(neighborsNode5.Contains(node2));
            Assert.IsTrue(neighborsNode5.Contains(node3));
            Assert.IsTrue(neighborsNode5.Contains(node4));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetNeighbors_NullNode_ShouldThrowArgumentNullException() {
            // Act
            graph.GetNeighbors(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetNeighbors_NodeNotInGraph_ShouldThrowArgumentException() {
            // Act
            graph.GetNeighbors(node1);
        }
        #endregion

        #region GetEdges Tests
        [TestMethod]
        public void GetEdges_NodeWithEdges_ShouldReturnAllAdjacentEdges() {
            // Arrange
            SetupStarGraph();

            // Act
            var edges = graph.GetEdges(node1).ToList();

            // Assert - Center node should have 3 edges
            Assert.AreEqual(3, edges.Count);
            Assert.IsTrue(edges.Contains(edge1));
            Assert.IsTrue(edges.Contains(edge2));
            Assert.IsTrue(edges.Contains(edge3));
        }

        [TestMethod]
        public void GetEdges_LeafNode_ShouldReturnSingleEdge() {
            // Arrange
            SetupStarGraph();

            // Act
            var edges = graph.GetEdges(node2).ToList();

            // Assert - Leaf node should have only one edge
            Assert.AreEqual(1, edges.Count);
            Assert.IsTrue(edges.Contains(edge1));
        }

        [TestMethod]
        public void GetEdges_NodeWithNoEdges_ShouldReturnEmptyCollection() {
            // Arrange
            graph.AddNode(node1);

            // Act
            var edges = graph.GetEdges(node1).ToList();

            // Assert
            Assert.AreEqual(0, edges.Count);
        }

        [TestMethod]
        public void GetEdges_UndirectedBehavior_EdgeShouldAppearForBothNodes() {
            // Arrange
            graph.AddNode(node1);
            graph.AddNode(node2);
            var edge = new Edge<string>(node1, node2);
            graph.AddEdge(edge);

            // Act
            var edgesNode1 = graph.GetEdges(node1).ToList();
            var edgesNode2 = graph.GetEdges(node2).ToList();

            // Assert - Same edge should appear for both nodes in undirected graph
            Assert.AreEqual(1, edgesNode1.Count);
            Assert.AreEqual(1, edgesNode2.Count);
            Assert.IsTrue(edgesNode1.Contains(edge));
            Assert.IsTrue(edgesNode2.Contains(edge));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetEdges_NullNode_ShouldThrowArgumentNullException() {
            // Act
            graph.GetEdges(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetEdges_NodeNotInGraph_ShouldThrowArgumentException() {
            // Act
            graph.GetEdges(node1);
        }
        #endregion

        #region Graph Properties Tests
        [TestMethod]
        public void NodeCount_EmptyGraph_ShouldReturnZero() {
            // Act & Assert
            Assert.AreEqual(0, graph.NodeCount);
        }

        [TestMethod]
        public void NodeCount_AfterAddingNodes_ShouldReturnCorrectCount() {
            // Arrange
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);

            // Act & Assert
            Assert.AreEqual(3, graph.NodeCount);
        }

        [TestMethod]
        public void EdgeCount_EmptyGraph_ShouldReturnZero() {
            // Act & Assert
            Assert.AreEqual(0, graph.EdgeCount);
        }

        [TestMethod]
        public void EdgeCount_AfterAddingEdges_ShouldReturnCorrectCount() {
            // Arrange
            SetupComplexGraph();

            // Act & Assert
            Assert.AreEqual(6, graph.EdgeCount);
        }

        [TestMethod]
        public void IsEmpty_EmptyGraph_ShouldReturnTrue() {
            // Act & Assert
            Assert.IsTrue(graph.IsEmpty);
        }

        [TestMethod]
        public void IsEmpty_NonEmptyGraph_ShouldReturnFalse() {
            // Arrange
            graph.AddNode(node1);

            // Act & Assert
            Assert.IsFalse(graph.IsEmpty);
        }

        [TestMethod]
        public void Name_ShouldBeSettable() {
            // Act
            graph.Name = "MyUndirectedGraph";

            // Assert
            Assert.AreEqual("MyUndirectedGraph", graph.Name);
        }

        [TestMethod]
        public void SerialNumber_ShouldBeUnique() {
            // Act
            var graph1 = new UnDirectedGraph(new UndirectedAdjacencyListStorage());
            var graph2 = new UnDirectedGraph(new UndirectedAdjacencyListStorage());

            // Assert
            Assert.AreNotEqual(graph1.SerialNumber, graph2.SerialNumber);
        }

        [TestMethod]
        public void MetaData_ShouldBeInitializedAndModifiable() {
            // Act
            graph.MetaData["key1"] = "value1";
            graph.MetaData["key2"] = 42;

            // Assert
            Assert.AreEqual("value1", graph.MetaData["key1"]);
            Assert.AreEqual(42, graph.MetaData["key2"]);
            Assert.AreEqual(2, graph.MetaData.Count);
        }

        [TestMethod]
        public void ToString_ShouldReturnMeaningfulString() {
            // Arrange
            graph.Name = "TestUndirectedGraph";
            graph.AddNode(node1);
            graph.AddNode(node2);
            var edge = new Edge<string>(node1, node2);
            graph.AddEdge(edge);

            // Act
            var result = graph.ToString();

            // Assert
            Assert.IsTrue(result.Contains("UnDirectedGraph"));
            Assert.IsTrue(result.Contains("TestUndirectedGraph"));
            Assert.IsTrue(result.Contains("Nodes: 2"));
            Assert.IsTrue(result.Contains("Edges: 1"));
        }
        #endregion

        #region Complex Scenarios Tests
        [TestMethod]
        public void ComplexScenario_SelfLoop_ShouldHandleCorrectly() {
            // Arrange
            graph.AddNode(node1);
            var selfLoop = new Edge<string>(node1, node1);
            graph.AddEdge(selfLoop);

            // Act & Assert
            Assert.IsTrue(graph.AreConnected(node1, node1));
            Assert.IsTrue(graph.GetNeighbors(node1).Contains(node1));
            Assert.IsTrue(graph.GetEdges(node1).Contains(selfLoop));
        }

        [TestMethod]
        public void ComplexScenario_MultipleEdgesBetweenSameNodes_ShouldHandleCorrectly() {
            // Arrange
            graph.AddNode(node1);
            graph.AddNode(node2);
            var edge1 = new Edge<string>(node1, node2);
            var edge2 = new Edge<int>(node1, node2);
            graph.AddEdge(edge1);
            graph.AddEdge(edge2);

            // Act
            var edgesNode1 = graph.GetEdges(node1).ToList();
            var edgesNode2 = graph.GetEdges(node2).ToList();
            var neighborsNode1 = graph.GetNeighbors(node1).ToList();

            // Assert
            Assert.AreEqual(2, edgesNode1.Count);
            Assert.AreEqual(2, edgesNode2.Count);
            Assert.AreEqual(1, neighborsNode1.Count); // Only one unique neighbor
            Assert.IsTrue(neighborsNode1.Contains(node2));
        }

        [TestMethod]
        public void ComplexScenario_CyclicGraph_ShouldHandleCorrectly() {
            // Arrange - Create a triangle: node1 -- node2 -- node3 -- node1
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);
            var edge1 = new Edge<string>(node1, node2);
            var edge2 = new Edge<string>(node2, node3);
            var edge3 = new Edge<string>(node3, node1);
            graph.AddEdge(edge1);
            graph.AddEdge(edge2);
            graph.AddEdge(edge3);

            // Act & Assert
            // Each node should be connected to the other two
            Assert.IsTrue(graph.AreConnected(node1, node2));
            Assert.IsTrue(graph.AreConnected(node2, node3));
            Assert.IsTrue(graph.AreConnected(node3, node1));
            Assert.IsTrue(graph.AreConnected(node1, node3)); // Through node2, but direct in cycle

            // Each node should have exactly 2 neighbors
            Assert.AreEqual(2, graph.GetNeighbors(node1).Count());
            Assert.AreEqual(2, graph.GetNeighbors(node2).Count());
            Assert.AreEqual(2, graph.GetNeighbors(node3).Count());
        }

        [TestMethod]
        public void ComplexScenario_RemoveNodeFromComplexGraph_ShouldMaintainConsistency() {
            // Arrange
            SetupComplexGraph();
            var initialEdgeCount = graph.EdgeCount;

            // Act - Remove node2 which is highly connected
            graph.RemoveNode(node2);

            // Assert
            Assert.IsFalse(graph.HasNode(node2));
            Assert.IsFalse(graph.HasEdge(edge1)); // edge1 was node1 -- node2
            Assert.IsFalse(graph.HasEdge(edge2)); // edge2 was node2 -- node3

            // Verify remaining nodes are intact
            Assert.IsTrue(graph.HasNode(node1));
            Assert.IsTrue(graph.HasNode(node3));
            Assert.IsTrue(graph.HasNode(node4));
            Assert.IsTrue(graph.HasNode(node5));

            // Verify neighbors are updated - node1 should no longer have node2 as neighbor
            Assert.IsFalse(graph.GetNeighbors(node1).Contains(node2));
            Assert.IsFalse(graph.GetNeighbors(node3).Contains(node2));
            Assert.IsFalse(graph.GetNeighbors(node5).Contains(node2));
        }

        [TestMethod]
        public void ComplexScenario_PathGraph_ShouldMaintainLinearStructure() {
            // Arrange - Create a path: node1 -- node2 -- node3 -- node4 -- node5
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);
            graph.AddNode(node4);
            graph.AddNode(node5);

            var edge1 = new Edge<string>(node1, node2);
            var edge2 = new Edge<string>(node2, node3);
            var edge3 = new Edge<string>(node3, node4);
            var edge4 = new Edge<string>(node4, node5);

            graph.AddEdge(edge1);
            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
            graph.AddEdge(edge4);

            // Act & Assert - Verify path structure
            // End nodes should have 1 neighbor each
            Assert.AreEqual(1, graph.GetNeighbors(node1).Count());
            Assert.AreEqual(1, graph.GetNeighbors(node5).Count());

            // Middle nodes should have 2 neighbors each
            Assert.AreEqual(2, graph.GetNeighbors(node2).Count());
            Assert.AreEqual(2, graph.GetNeighbors(node3).Count());
            Assert.AreEqual(2, graph.GetNeighbors(node4).Count());

            // End nodes should not be directly connected
            Assert.IsFalse(graph.AreConnected(node1, node5));
        }

        [TestMethod]
        public void ComplexScenario_CompleteGraph_ShouldConnectAllNodes() {
            // Arrange - Create a complete graph with 4 nodes (K4)
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);
            graph.AddNode(node4);

            // Add all possible edges
            var edges = new List<Edge<string>> {
                new Edge<string>(node1, node2),
                new Edge<string>(node1, node3),
                new Edge<string>(node1, node4),
                new Edge<string>(node2, node3),
                new Edge<string>(node2, node4),
                new Edge<string>(node3, node4)
            };

            foreach (var edge in edges) {
                graph.AddEdge(edge);
            }

            // Act & Assert - Every node should be connected to every other node
            var nodes = new[] { node1, node2, node3, node4 };

            foreach (var nodeA in nodes) {
                // Each node should have 3 neighbors (all others)
                Assert.AreEqual(3, graph.GetNeighbors(nodeA).Count());

                foreach (var nodeB in nodes) {
                    if (nodeA != nodeB) {
                        Assert.IsTrue(graph.AreConnected(nodeA, nodeB));
                    }
                }
            }

            // Should have 6 edges total (n(n-1)/2 for complete graph)
            Assert.AreEqual(6, graph.EdgeCount);
        }

        [TestMethod]
        public void ComplexScenario_BipartiteGraph_ShouldMaintainBipartiteStructure() {
            // Arrange - Create a simple bipartite graph
            // Set A: {node1, node2}, Set B: {node3, node4}
            // Connections only between sets
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);
            graph.AddNode(node4);

            var edge1 = new Edge<string>(node1, node3);
            var edge2 = new Edge<string>(node1, node4);
            var edge3 = new Edge<string>(node2, node3);
            var edge4 = new Edge<string>(node2, node4);

            graph.AddEdge(edge1);
            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
            graph.AddEdge(edge4);

            // Act & Assert - Verify bipartite structure
            // Nodes in set A should not be connected to each other
            Assert.IsFalse(graph.AreConnected(node1, node2));

            // Nodes in set B should not be connected to each other
            Assert.IsFalse(graph.AreConnected(node3, node4));

            // All cross-set connections should exist
            Assert.IsTrue(graph.AreConnected(node1, node3));
            Assert.IsTrue(graph.AreConnected(node1, node4));
            Assert.IsTrue(graph.AreConnected(node2, node3));
            Assert.IsTrue(graph.AreConnected(node2, node4));

            // Each node should have exactly 2 neighbors
            Assert.AreEqual(2, graph.GetNeighbors(node1).Count());
            Assert.AreEqual(2, graph.GetNeighbors(node2).Count());
            Assert.AreEqual(2, graph.GetNeighbors(node3).Count());
            Assert.AreEqual(2, graph.GetNeighbors(node4).Count());
        }
        #endregion

        #region Performance and Stress Tests
        [TestMethod]
        public void StressTest_LargeGraph_ShouldPerformReasonably() {
            // Arrange - Create a graph with many nodes forming a long path
            var nodes = new List<Node<int>>();
            for (int i = 0; i < 100; i++) {
                var node = new Node<int>();
                nodes.Add(node);
                graph.AddNode(node);
            }

            // Create edges to form a path: node0 -- node1 -- node2 -- ... -- node99
            for (int i = 0; i < 99; i++) {
                var edge = new Edge<string>(nodes[i], nodes[i + 1]);
                graph.AddEdge(edge);
            }

            // Act & Assert
            Assert.AreEqual(100, graph.NodeCount);
            Assert.AreEqual(99, graph.EdgeCount);

            // Test that operations still work correctly
            Assert.AreEqual(1, graph.GetNeighbors(nodes[0]).Count());
            Assert.AreEqual(1, graph.GetNeighbors(nodes[99]).Count());
            Assert.AreEqual(2, graph.GetNeighbors(nodes[50]).Count());

            // Test connectivity
            Assert.IsTrue(graph.AreConnected(nodes[0], nodes[1]));
            Assert.IsTrue(graph.AreConnected(nodes[98], nodes[99]));
            Assert.IsFalse(graph.AreConnected(nodes[0], nodes[99]));
        }

        [TestMethod]
        public void StressTest_DenseGraph_ShouldHandleHighConnectivity() {
            // Arrange - Create a moderately dense graph
            var nodes = new List<Node<int>>();
            for (int i = 0; i < 20; i++) {
                var node = new Node<int>();
                nodes.Add(node);
                graph.AddNode(node);
            }

            // Connect each node to its next 3 neighbors (circular)
            var edgeCount = 0;
            for (int i = 0; i < 20; i++) {
                for (int j = 1; j <= 3; j++) {
                    int targetIndex = (i + j) % 20;
                    if (i < targetIndex) { // Avoid duplicate edges in undirected graph
                        var edge = new Edge<string>(nodes[i], nodes[targetIndex]);
                        graph.AddEdge(edge);
                        edgeCount++;
                    }
                }
            }

            // Act & Assert
            Assert.AreEqual(20, graph.NodeCount);
            Assert.AreEqual(edgeCount, graph.EdgeCount);

            // Each node should have at least 3 neighbors (some may have more due to circularity)
            foreach (var node in nodes) {
                Assert.IsTrue(graph.GetNeighbors(node).Count() >= 3);
            }
        }
        #endregion

        #region Interface Compliance Tests
        [TestMethod]
        public void InterfaceCompliance_IUndirectedGraph_ShouldImplementAllMethods() {
            // Arrange
            IUndirectedGraph undirectedGraph = graph;
            graph.AddNode(node1);
            graph.AddNode(node2);
            var edge = new Edge<string>(node1, node2);
            graph.AddEdge(edge);

            // Act & Assert - All interface methods should be accessible
            Assert.IsNotNull(undirectedGraph.GetNeighbors(node1));
            Assert.IsNotNull(undirectedGraph.GetEdges(node1));
        }

        [TestMethod]
        public void InterfaceCompliance_IGraph_ShouldImplementAllMethods() {
            // Arrange
            IGraph genericGraph = graph;
            graph.AddNode(node1);
            graph.AddNode(node2);
            var edge = new Edge<string>(node1, node2);
            graph.AddEdge(edge);

            // Act & Assert - All base interface methods should be accessible
            Assert.IsTrue(genericGraph.HasNode(node1));
            Assert.IsTrue(genericGraph.HasEdge(edge));
            Assert.IsTrue(genericGraph.AreConnected(node1, node2));
            Assert.IsTrue(genericGraph.AreConnected(node2, node1)); // Bidirectional in undirected
            Assert.IsNotNull(genericGraph.Name);
            Assert.IsNotNull(genericGraph.MetaData);
        }
        #endregion

        #region Comparison with Directed Graph Behavior
        [TestMethod]
        public void ComparisonTest_UndirectedVsDirected_ConnectionsBehaveDifferently() {
            // Arrange
            var directedStorage = new DirectedAdjacencyListStorage();
            var directedGraph = new DirectedGraph(directedStorage);

            // Add same nodes and edges to both graphs
            graph.AddNode(node1);
            graph.AddNode(node2);
            directedGraph.AddNode(node1);
            directedGraph.AddNode(node2);

            var edge = new Edge<string>(node1, node2);
            graph.AddEdge(edge);
            directedGraph.AddEdge(edge);

            // Act & Assert - Undirected should be bidirectional, directed should not
            Assert.IsTrue(graph.AreConnected(node1, node2));
            Assert.IsTrue(graph.AreConnected(node2, node1)); // Bidirectional

            Assert.IsTrue(directedGraph.AreConnected(node1, node2));
            Assert.IsFalse(directedGraph.AreConnected(node2, node1)); // Unidirectional
        }

        [TestMethod]
        public void ComparisonTest_UndirectedVsDirected_NeighborsBehaveDifferently() {
            // Arrange
            var directedStorage = new DirectedAdjacencyListStorage();
            var directedGraph = new DirectedGraph(directedStorage);

            // Create a more complex scenario
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);
            directedGraph.AddNode(node1);
            directedGraph.AddNode(node2);
            directedGraph.AddNode(node3);

            var edge1 = new Edge<string>(node1, node2);
            var edge2 = new Edge<string>(node2, node3);
            graph.AddEdge(edge1);
            graph.AddEdge(edge2);
            directedGraph.AddEdge(edge1);
            directedGraph.AddEdge(edge2);

            // Act
            var undirectedNeighborsNode2 = graph.GetNeighbors(node2).ToList();
            var directedSuccessorsNode2 = directedGraph.GetSuccessors(node2).ToList();
            var directedPredecessorsNode2 = directedGraph.GetPredecessors(node2).ToList();

            // Assert
            // Undirected: node2 should see both node1 and node3 as neighbors
            Assert.AreEqual(2, undirectedNeighborsNode2.Count);
            Assert.IsTrue(undirectedNeighborsNode2.Contains(node1));
            Assert.IsTrue(undirectedNeighborsNode2.Contains(node3));

            // Directed: node2 should have node1 as predecessor and node3 as successor
            Assert.AreEqual(1, directedSuccessorsNode2.Count);
            Assert.AreEqual(1, directedPredecessorsNode2.Count);
            Assert.IsTrue(directedSuccessorsNode2.Contains(node3));
            Assert.IsTrue(directedPredecessorsNode2.Contains(node1));
        }
        #endregion

        #region Edge Cases and Boundary Tests
        [TestMethod]
        public void EdgeCase_SingleNodeGraph_ShouldHandleCorrectly() {
            // Arrange
            graph.AddNode(node1);

            // Act & Assert
            Assert.AreEqual(1, graph.NodeCount);
            Assert.AreEqual(0, graph.EdgeCount);
            Assert.AreEqual(0, graph.GetNeighbors(node1).Count());
            Assert.AreEqual(0, graph.GetEdges(node1).Count());
            Assert.IsFalse(graph.AreConnected(node1, node1));
        }

        [TestMethod]
        public void EdgeCase_TwoNodesNoEdges_ShouldHandleCorrectly() {
            // Arrange
            graph.AddNode(node1);
            graph.AddNode(node2);

            // Act & Assert
            Assert.AreEqual(2, graph.NodeCount);
            Assert.AreEqual(0, graph.EdgeCount);
            Assert.AreEqual(0, graph.GetNeighbors(node1).Count());
            Assert.AreEqual(0, graph.GetNeighbors(node2).Count());
            Assert.IsFalse(graph.AreConnected(node1, node2));
            Assert.IsFalse(graph.AreConnected(node2, node1));
        }

        [TestMethod]
        public void EdgeCase_RemoveAllEdges_ShouldLeaveIsolatedNodes() {
            // Arrange
            SetupLinearGraph();

            // Act - Remove all edges
            graph.RemoveEdge(edge1);
            graph.RemoveEdge(edge2);

            // Assert
            Assert.AreEqual(3, graph.NodeCount);
            Assert.AreEqual(0, graph.EdgeCount);
            Assert.AreEqual(0, graph.GetNeighbors(node1).Count());
            Assert.AreEqual(0, graph.GetNeighbors(node2).Count());
            Assert.AreEqual(0, graph.GetNeighbors(node3).Count());
            Assert.IsFalse(graph.AreConnected(node1, node2));
            Assert.IsFalse(graph.AreConnected(node2, node3));
        }

        [TestMethod]
        public void EdgeCase_RemoveAllNodes_ShouldResultInEmptyGraph() {
            // Arrange
            SetupLinearGraph();

            // Act - Remove all nodes
            graph.RemoveNode(node1);
            graph.RemoveNode(node2);
            graph.RemoveNode(node3);

            // Assert
            Assert.AreEqual(0, graph.NodeCount);
            Assert.AreEqual(0, graph.EdgeCount);
            Assert.IsTrue(graph.IsEmpty);
        }

        [TestMethod]
        public void EdgeCase_AddRemoveAddSameEdge_ShouldWorkCorrectly() {
            // Arrange
            graph.AddNode(node1);
            graph.AddNode(node2);
            var edge = new Edge<string>(node1, node2);

            // Act & Assert - Add edge
            graph.AddEdge(edge);
            Assert.IsTrue(graph.HasEdge(edge));
            Assert.IsTrue(graph.AreConnected(node1, node2));

            // Remove edge
            graph.RemoveEdge(edge);
            Assert.IsFalse(graph.HasEdge(edge));
            Assert.IsFalse(graph.AreConnected(node1, node2));

            // Add edge again
            graph.AddEdge(edge);
            Assert.IsTrue(graph.HasEdge(edge));
            Assert.IsTrue(graph.AreConnected(node1, node2));
        }
        #endregion

        #region Graph Theory Properties Tests
        [TestMethod]
        public void GraphTheory_TreeProperties_ShouldSatisfyTreeConstraints() {
            // Arrange - Create a tree (connected graph with n-1 edges)
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);
            graph.AddNode(node4);
            graph.AddNode(node5);

            // Tree structure: node1 is root, node2 and node3 are children of node1,
            // node4 and node5 are children of node2
            var edge1 = new Edge<string>(node1, node2);
            var edge2 = new Edge<string>(node1, node3);
            var edge3 = new Edge<string>(node2, node4);
            var edge4 = new Edge<string>(node2, node5);

            graph.AddEdge(edge1);
            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
            graph.AddEdge(edge4);

            // Act & Assert - Tree properties
            Assert.AreEqual(5, graph.NodeCount);
            Assert.AreEqual(4, graph.EdgeCount); // n-1 edges for tree

            // Verify tree structure
            Assert.AreEqual(2, graph.GetNeighbors(node1).Count()); // Root has 2 children
            Assert.AreEqual(3, graph.GetNeighbors(node2).Count()); // Internal node: parent + 2 children
            Assert.AreEqual(1, graph.GetNeighbors(node3).Count()); // Leaf node
            Assert.AreEqual(1, graph.GetNeighbors(node4).Count()); // Leaf node
            Assert.AreEqual(1, graph.GetNeighbors(node5).Count()); // Leaf node
        }

        [TestMethod]
        public void GraphTheory_DisconnectedGraph_ShouldHandleComponents() {
            // Arrange - Create two disconnected components
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);
            graph.AddNode(node4);

            // Component 1: node1 -- node2
            var edge1 = new Edge<string>(node1, node2);
            graph.AddEdge(edge1);

            // Component 2: node3 -- node4
            var edge2 = new Edge<string>(node3, node4);
            graph.AddEdge(edge2);

            // Act & Assert
            // Within components should be connected
            Assert.IsTrue(graph.AreConnected(node1, node2));
            Assert.IsTrue(graph.AreConnected(node3, node4));

            // Between components should not be connected
            Assert.IsFalse(graph.AreConnected(node1, node3));
            Assert.IsFalse(graph.AreConnected(node1, node4));
            Assert.IsFalse(graph.AreConnected(node2, node3));
            Assert.IsFalse(graph.AreConnected(node2, node4));

            // Each component should have internal structure
            Assert.AreEqual(1, graph.GetNeighbors(node1).Count());
            Assert.AreEqual(1, graph.GetNeighbors(node2).Count());
            Assert.AreEqual(1, graph.GetNeighbors(node3).Count());
            Assert.AreEqual(1, graph.GetNeighbors(node4).Count());
        }
        #endregion

        #region Regression Tests
        [TestMethod]
        public void RegressionTest_EdgeRemovalUpdatesBothNodes() {
            // This test ensures that when an edge is removed in an undirected graph,
            // it's removed from both nodes' adjacency lists

            // Arrange
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);

            var edge1 = new Edge<string>(node1, node2);
            var edge2 = new Edge<string>(node2, node3);
            graph.AddEdge(edge1);
            graph.AddEdge(edge2);

            // Act - Remove edge1
            graph.RemoveEdge(edge1);

            // Assert - edge1 should be removed from both node1 and node2
            Assert.IsFalse(graph.GetEdges(node1).Contains(edge1));
            Assert.IsFalse(graph.GetEdges(node2).Contains(edge1));
            Assert.IsFalse(graph.GetNeighbors(node1).Contains(node2));
            Assert.IsFalse(graph.GetNeighbors(node2).Contains(node1));

            // But edge2 should still be intact
            Assert.IsTrue(graph.GetEdges(node2).Contains(edge2));
            Assert.IsTrue(graph.GetEdges(node3).Contains(edge2));
            Assert.IsTrue(graph.GetNeighbors(node2).Contains(node3));
            Assert.IsTrue(graph.GetNeighbors(node3).Contains(node2));
        }

        [TestMethod]
        public void RegressionTest_NodeRemovalCleansUpAllEdges() {
            // This test ensures that when a node is removed, all its edges are properly
            // removed from all connected nodes

            // Arrange
            SetupStarGraph(); // node1 connected to node2, node3, node4

            // Act - Remove center node
            graph.RemoveNode(node1);

            // Assert - All leaf nodes should have no neighbors
            Assert.AreEqual(0, graph.GetNeighbors(node2).Count());
            Assert.AreEqual(0, graph.GetNeighbors(node3).Count());
            Assert.AreEqual(0, graph.GetNeighbors(node4).Count());

            // All edges should be removed
            Assert.AreEqual(0, graph.EdgeCount);

            // Only 3 nodes should remain
            Assert.AreEqual(3, graph.NodeCount);
        }
        #endregion
    }
}
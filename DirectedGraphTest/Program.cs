using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunicationNetwork.Graph;

namespace CommunicationNetwork.Graph.Tests {

    [TestClass]
    public class DirectedGraphTests {
        private DirectedGraph graph;
        private DirectedAdjacencyListStorage storage;
        private Node<int> node1;
        private Node<int> node2;
        private Node<int> node3;
        private Node<int> node4;
        private Edge<string> edge1;
        private Edge<string> edge2;
        private Edge<string> edge3;

        [TestInitialize]
        public void Setup() {
            storage = new DirectedAdjacencyListStorage();
            graph = new DirectedGraph(storage);

            node1 = new Node<int>();
            node2 = new Node<int>();
            node3 = new Node<int>();
            node4 = new Node<int>();
        }

        private void SetupComplexGraph() {
            // Create a complex directed graph:
            // node1 -> node2 -> node3
            //   |               ^
            //   v               |
            // node4 ------------+

            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);
            graph.AddNode(node4);

            edge1 = new Edge<string>(node1, node2);
            edge2 = new Edge<string>(node2, node3);
            edge3 = new Edge<string>(node1, node4);
            var edge4 = new Edge<string>(node4, node3);

            graph.AddEdge(edge1);
            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
            graph.AddEdge(edge4);
        }

        #region Constructor Tests
        [TestMethod]
        public void Constructor_ValidStorage_ShouldCreateGraph() {
            // Act
            var testGraph = new DirectedGraph(storage);

            // Assert
            Assert.IsNotNull(testGraph);
            Assert.AreEqual(0, testGraph.NodeCount);
            Assert.AreEqual(0, testGraph.EdgeCount);
            Assert.IsTrue(testGraph.IsEmpty);
        }

        [TestMethod]
        public void Constructor_WithName_ShouldSetName() {
            // Act
            var testGraph = new DirectedGraph(storage, "TestGraph");

            // Assert
            Assert.AreEqual("TestGraph", testGraph.Name);
        }

        [TestMethod]
        public void Constructor_WithoutName_ShouldGenerateDefaultName() {
            // Act
            var testGraph = new DirectedGraph(storage);

            // Assert
            Assert.IsTrue(testGraph.Name.StartsWith("DirectedGraph_"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullStorage_ShouldThrowArgumentNullException() {
            // Act
            new DirectedGraph(null);
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

        #region AreConnected Tests
        [TestMethod]
        public void AreConnected_DirectlyConnectedNodes_ShouldReturnTrue() {
            // Arrange
            graph.AddNode(node1);
            graph.AddNode(node2);
            var edge = new Edge<string>(node1, node2);
            graph.AddEdge(edge);

            // Act & Assert
            Assert.IsTrue(graph.AreConnected(node1, node2));
        }

        [TestMethod]
        public void AreConnected_NotConnectedNodes_ShouldReturnFalse() {
            // Arrange
            graph.AddNode(node1);
            graph.AddNode(node2);
            var edge = new Edge<string>(node1, node2);
            graph.AddEdge(edge);

            // Act & Assert - Directed graph, so reverse direction should be false
            Assert.IsFalse(graph.AreConnected(node2, node1));
        }

        [TestMethod]
        public void AreConnected_IndirectlyConnectedNodes_ShouldReturnFalse() {
            // Arrange
            SetupComplexGraph();

            // Act & Assert - Not directly connected
            Assert.IsFalse(graph.AreConnected(node1, node3));
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

        #region GetPredecessors Tests
        [TestMethod]
        public void GetPredecessors_NodeWithPredecessors_ShouldReturnCorrectNodes() {
            // Arrange
            SetupComplexGraph();

            // Act
            var predecessors = graph.GetPredecessors(node3).ToList();

            // Assert
            Assert.AreEqual(2, predecessors.Count);
            Assert.IsTrue(predecessors.Contains(node2));
            Assert.IsTrue(predecessors.Contains(node4));
        }

        [TestMethod]
        public void GetPredecessors_NodeWithNoPredecessors_ShouldReturnEmptyCollection() {
            // Arrange
            SetupComplexGraph();

            // Act
            var predecessors = graph.GetPredecessors(node1).ToList();

            // Assert
            Assert.AreEqual(0, predecessors.Count);
        }

        [TestMethod]
        public void GetPredecessors_SinglePredecessor_ShouldReturnCorrectNode() {
            // Arrange
            SetupComplexGraph();

            // Act
            var predecessors = graph.GetPredecessors(node2).ToList();

            // Assert
            Assert.AreEqual(1, predecessors.Count);
            Assert.IsTrue(predecessors.Contains(node1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPredecessors_NullNode_ShouldThrowArgumentNullException() {
            // Act
            graph.GetPredecessors(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPredecessors_NodeNotInGraph_ShouldThrowArgumentException() {
            // Act
            graph.GetPredecessors(node1);
        }
        #endregion

        #region GetSuccessors Tests
        [TestMethod]
        public void GetSuccessors_NodeWithSuccessors_ShouldReturnCorrectNodes() {
            // Arrange
            SetupComplexGraph();

            // Act
            var successors = graph.GetSuccessors(node1).ToList();

            // Assert
            Assert.AreEqual(2, successors.Count);
            Assert.IsTrue(successors.Contains(node2));
            Assert.IsTrue(successors.Contains(node4));
        }

        [TestMethod]
        public void GetSuccessors_NodeWithNoSuccessors_ShouldReturnEmptyCollection() {
            // Arrange
            SetupComplexGraph();

            // Act
            var successors = graph.GetSuccessors(node3).ToList();

            // Assert
            Assert.AreEqual(0, successors.Count);
        }

        [TestMethod]
        public void GetSuccessors_SingleSuccessor_ShouldReturnCorrectNode() {
            // Arrange
            SetupComplexGraph();

            // Act
            var successors = graph.GetSuccessors(node2).ToList();

            // Assert
            Assert.AreEqual(1, successors.Count);
            Assert.IsTrue(successors.Contains(node3));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetSuccessors_NullNode_ShouldThrowArgumentNullException() {
            // Act
            graph.GetSuccessors(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetSuccessors_NodeNotInGraph_ShouldThrowArgumentException() {
            // Act
            graph.GetSuccessors(node1);
        }
        #endregion

        #region GetOutgoingEdges Tests
        [TestMethod]
        public void GetOutgoingEdges_NodeWithOutgoingEdges_ShouldReturnCorrectEdges() {
            // Arrange
            SetupComplexGraph();

            // Act
            var outgoingEdges = graph.GetOutgoingEdges(node1).ToList();

            // Assert
            Assert.AreEqual(2, outgoingEdges.Count);
            Assert.IsTrue(outgoingEdges.Contains(edge1));
            Assert.IsTrue(outgoingEdges.Contains(edge3));
        }

        [TestMethod]
        public void GetOutgoingEdges_NodeWithNoOutgoingEdges_ShouldReturnEmptyCollection() {
            // Arrange
            SetupComplexGraph();

            // Act
            var outgoingEdges = graph.GetOutgoingEdges(node3).ToList();

            // Assert
            Assert.AreEqual(0, outgoingEdges.Count);
        }

        [TestMethod]
        public void GetOutgoingEdges_SingleOutgoingEdge_ShouldReturnCorrectEdge() {
            // Arrange
            SetupComplexGraph();

            // Act
            var outgoingEdges = graph.GetOutgoingEdges(node2).ToList();

            // Assert
            Assert.AreEqual(1, outgoingEdges.Count);
            Assert.IsTrue(outgoingEdges.Contains(edge2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetOutgoingEdges_NullNode_ShouldThrowArgumentNullException() {
            // Act
            graph.GetOutgoingEdges(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetOutgoingEdges_NodeNotInGraph_ShouldThrowArgumentException() {
            // Act
            graph.GetOutgoingEdges(node1);
        }
        #endregion

        #region GetIncomingEdges Tests
        [TestMethod]
        public void GetIncomingEdges_NodeWithIncomingEdges_ShouldReturnCorrectEdges() {
            // Arrange
            SetupComplexGraph();

            // Act
            var incomingEdges = graph.GetIncomingEdges(node3).ToList();

            // Assert
            Assert.AreEqual(2, incomingEdges.Count);
            Assert.IsTrue(incomingEdges.Contains(edge2));
            // Should also contain the edge from node4 to node3
            Assert.IsTrue(incomingEdges.Any(e => e.Source == node4 && e.Target == node3));
        }

        [TestMethod]
        public void GetIncomingEdges_NodeWithNoIncomingEdges_ShouldReturnEmptyCollection() {
            // Arrange
            SetupComplexGraph();

            // Act
            var incomingEdges = graph.GetIncomingEdges(node1).ToList();

            // Assert
            Assert.AreEqual(0, incomingEdges.Count);
        }

        [TestMethod]
        public void GetIncomingEdges_SingleIncomingEdge_ShouldReturnCorrectEdge() {
            // Arrange
            SetupComplexGraph();

            // Act
            var incomingEdges = graph.GetIncomingEdges(node2).ToList();

            // Assert
            Assert.AreEqual(1, incomingEdges.Count);
            Assert.IsTrue(incomingEdges.Contains(edge1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetIncomingEdges_NullNode_ShouldThrowArgumentNullException() {
            // Act
            graph.GetIncomingEdges(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetIncomingEdges_NodeNotInGraph_ShouldThrowArgumentException() {
            // Act
            graph.GetIncomingEdges(node1);
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
            Assert.AreEqual(4, graph.EdgeCount);
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
            graph.Name = "MyDirectedGraph";

            // Assert
            Assert.AreEqual("MyDirectedGraph", graph.Name);
        }

        [TestMethod]
        public void SerialNumber_ShouldBeUnique() {
            // Act
            var graph1 = new DirectedGraph(new DirectedAdjacencyListStorage());
            var graph2 = new DirectedGraph(new DirectedAdjacencyListStorage());

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
            graph.Name = "TestGraph";
            graph.AddNode(node1);
            graph.AddNode(node2);
            var edge = new Edge<string>(node1, node2);
            graph.AddEdge(edge);

            // Act
            var result = graph.ToString();

            // Assert
            Assert.IsTrue(result.Contains("DirectedGraph"));
            Assert.IsTrue(result.Contains("TestGraph"));
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
            Assert.IsTrue(graph.GetSuccessors(node1).Contains(node1));
            Assert.IsTrue(graph.GetPredecessors(node1).Contains(node1));
            Assert.IsTrue(graph.GetOutgoingEdges(node1).Contains(selfLoop));
            Assert.IsTrue(graph.GetIncomingEdges(node1).Contains(selfLoop));
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
            var outgoingEdges = graph.GetOutgoingEdges(node1).ToList();
            var incomingEdges = graph.GetIncomingEdges(node2).ToList();
            var successors = graph.GetSuccessors(node1).ToList();

            // Assert
            Assert.AreEqual(2, outgoingEdges.Count);
            Assert.AreEqual(2, incomingEdges.Count);
            Assert.AreEqual(1, successors.Count); // Only one unique successor
            Assert.IsTrue(successors.Contains(node2));
        }

        [TestMethod]
        public void ComplexScenario_CyclicGraph_ShouldHandleCorrectly() {
            // Arrange - Create a cycle: node1 -> node2 -> node3 -> node1
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
            Assert.IsTrue(graph.AreConnected(node1, node2));
            Assert.IsTrue(graph.AreConnected(node2, node3));
            Assert.IsTrue(graph.AreConnected(node3, node1));

            // Each node should have one predecessor and one successor
            Assert.AreEqual(1, graph.GetPredecessors(node1).Count());
            Assert.AreEqual(1, graph.GetSuccessors(node1).Count());
            Assert.AreEqual(1, graph.GetPredecessors(node2).Count());
            Assert.AreEqual(1, graph.GetSuccessors(node2).Count());
            Assert.AreEqual(1, graph.GetPredecessors(node3).Count());
            Assert.AreEqual(1, graph.GetSuccessors(node3).Count());
        }

        [TestMethod]
        public void ComplexScenario_RemoveNodeFromComplexGraph_ShouldMaintainConsistency() {
            // Arrange
            SetupComplexGraph();
            var initialEdgeCount = graph.EdgeCount;

            // Act - Remove node2 which is in the middle of the graph
            graph.RemoveNode(node2);

            // Assert
            Assert.IsFalse(graph.HasNode(node2));
            Assert.IsFalse(graph.HasEdge(edge1)); // edge1 was node1 -> node2
            Assert.IsFalse(graph.HasEdge(edge2)); // edge2 was node2 -> node3

            // Verify remaining nodes and edges are intact
            Assert.IsTrue(graph.HasNode(node1));
            Assert.IsTrue(graph.HasNode(node3));
            Assert.IsTrue(graph.HasNode(node4));
            Assert.IsTrue(graph.HasEdge(edge3)); // edge3 was node1 -> node4

            // Verify successors/predecessors are updated
            Assert.AreEqual(1, graph.GetSuccessors(node1).Count()); // Only node4 now
            Assert.AreEqual(1, graph.GetPredecessors(node3).Count()); // Only node4 now
        }

        [TestMethod]
        public void ComplexScenario_DAG_TopologicalProperties() {
            // Arrange - Create a simple DAG (Directed Acyclic Graph)
            graph.AddNode(node1);
            graph.AddNode(node2);
            graph.AddNode(node3);
            graph.AddNode(node4);

            // Create edges: node1 -> {node2, node3}, node2 -> node4, node3 -> node4
            var edge1 = new Edge<string>(node1, node2);
            var edge2 = new Edge<string>(node1, node3);
            var edge3 = new Edge<string>(node2, node4);
            var edge4 = new Edge<string>(node3, node4);

            graph.AddEdge(edge1);
            graph.AddEdge(edge2);
            graph.AddEdge(edge3);
            graph.AddEdge(edge4);

            // Act & Assert - Verify DAG properties
            // Source node (no predecessors)
            Assert.AreEqual(0, graph.GetPredecessors(node1).Count());
            Assert.AreEqual(2, graph.GetSuccessors(node1).Count());

            // Sink node (no successors)
            Assert.AreEqual(0, graph.GetSuccessors(node4).Count());
            Assert.AreEqual(2, graph.GetPredecessors(node4).Count());

            // Intermediate nodes
            Assert.AreEqual(1, graph.GetPredecessors(node2).Count());
            Assert.AreEqual(1, graph.GetSuccessors(node2).Count());
            Assert.AreEqual(1, graph.GetPredecessors(node3).Count());
            Assert.AreEqual(1, graph.GetSuccessors(node3).Count());
        }
        #endregion

        #region Performance and Stress Tests
        [TestMethod]
        public void StressTest_LargeGraph_ShouldPerformReasonably() {
            // Arrange - Create a graph with many nodes and edges
            var nodes = new List<Node<int>>();
            for (int i = 0; i < 100; i++) {
                var node = new Node<int>();
                nodes.Add(node);
                graph.AddNode(node);
            }

            // Create edges to form a chain: node0 -> node1 -> node2 -> ... -> node99
            for (int i = 0; i < 99; i++) {
                var edge = new Edge<string>(nodes[i], nodes[i + 1]);
                graph.AddEdge(edge);
            }

            // Act & Assert
            Assert.AreEqual(100, graph.NodeCount);
            Assert.AreEqual(99, graph.EdgeCount);

            // Test that operations still work correctly
            Assert.AreEqual(0, graph.GetPredecessors(nodes[0]).Count());
            Assert.AreEqual(0, graph.GetSuccessors(nodes[99]).Count());
            Assert.AreEqual(1, graph.GetSuccessors(nodes[50]).Count());
            Assert.AreEqual(1, graph.GetPredecessors(nodes[50]).Count());
        }
        #endregion

        #region Interface Compliance Tests
        [TestMethod]
        public void InterfaceCompliance_IDirectedGraph_ShouldImplementAllMethods() {
            // Arrange
            IDirectedGraph directedGraph = graph;
            graph.AddNode(node1);
            graph.AddNode(node2);
            var edge = new Edge<string>(node1, node2);
            graph.AddEdge(edge);

            // Act & Assert - All interface methods should be accessible
            Assert.IsNotNull(directedGraph.GetPredecessors(node2));
            Assert.IsNotNull(directedGraph.GetSuccessors(node1));
            Assert.IsNotNull(directedGraph.GetOutgoingEdges(node1));
            Assert.IsNotNull(directedGraph.GetIncomingEdges(node2));
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
            Assert.IsNotNull(genericGraph.Name);
            Assert.IsNotNull(genericGraph.MetaData);
        }
        #endregion
    }
}
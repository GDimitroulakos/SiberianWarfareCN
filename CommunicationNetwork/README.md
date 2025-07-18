 CommunicationNetwork Library Tutorial

## Table of Contents

1. [Getting Started](#getting-started)
2. [Creating Graphs](#creating-graphs)
   - [Creating an Undirected Graph](#creating-an-undirected-graph)
   - [Creating Nodes](#creating-nodes)
   - [Creating Edges](#creating-edges)
   - [Building the Graph](#building-the-graph)
   - [Creating a Directed Graph](#creating-a-directed-graph)
3. [Running Algorithms](#running-algorithms)
   - [Depth-First Search (DFS)](#depth-first-search-dfs)
   - [Breadth-First Search (BFS)](#breadth-first-search-bfs)
   - [Working with Weighted Graphs](#working-with-weighted-graphs)
     - [Step 1: Create Sample Weights](#step-1-create-sample-weights)
     - [Step 2: Run Bellman-Ford Algorithm](#step-2-run-bellman-ford-algorithm)
4. [Understanding the Data Provider Pattern](#understanding-the-data-provider-pattern)
5. [Key Concepts](#key-concepts)
   - [Graph Elements](#graph-elements)
   - [Algorithm Results](#algorithm-results)
   - [Storage Patterns](#storage-patterns)
6. [Best Practices](#best-practices)
7. [Next Steps](#next-steps)

---

## Getting Started

First, include the necessary namespaces:

```csharp
using CommunicationNetwork.Algorithm;
using CommunicationNetwork.Algorithm.TestingAlgorithms;
using CommunicationNetwork.Algorithms;
using CommunicationNetwork.Graph;
using CommunicationNetwork.Graph.GraphvizPrinter;
```

## Creating Graphs

### Creating an Undirected Graph

The library uses a storage-based approach for graphs. First, create a storage instance, then create the graph:

```csharp
UnDirectedGraph graph = new UnDirectedGraph(new UndirectedAdjacencyListStorage(), "test");
```

The constructor takes:
- A storage implementation (`UndirectedAdjacencyListStorage`)
- An optional name for the graph (`"test"`)

### Creating Nodes

Nodes are simple objects that get automatically assigned unique IDs:

```csharp
Node node1 = new Node();
Node node2 = new Node();
Node node3 = new Node();
Node node4 = new Node();
Node node5 = new Node();
```

### Creating Edges

Edges connect two nodes. For undirected graphs, the order doesn't matter:

```csharp
Edge edge1 = new Edge(node1, node2);
Edge edge2 = new Edge(node2, node3);
Edge edge3 = new Edge(node1, node4);
Edge edge4 = new Edge(node1, node5);
Edge edge5 = new Edge(node4, node5);
Edge edge6 = new Edge(node3, node5);
```

### Building the Graph

Add nodes first, then edges:

```csharp
// Add all nodes
graph.AddNode(node1);
graph.AddNode(node2);
graph.AddNode(node3);
graph.AddNode(node4);
graph.AddNode(node5);

// Add edges (only add the edges that exist in your undirected graph)
graph.AddEdge(edge1);
graph.AddEdge(edge2);
graph.AddEdge(edge3);
graph.AddEdge(edge4);
graph.AddEdge(edge5);
```

### Creating a Directed Graph

The process is similar for directed graphs, but use `DirectedGraph` and `DirectedAdjacencyListStorage`:

```csharp
DirectedGraph directedGraph = new DirectedGraph(new DirectedAdjacencyListStorage(), "test_directed");

// Add nodes (same as before)
directedGraph.AddNode(node1);
directedGraph.AddNode(node2);
directedGraph.AddNode(node3);
directedGraph.AddNode(node4);
directedGraph.AddNode(node5);

// Add edges - order matters for directed graphs!
directedGraph.AddEdge(edge1);  // node1 -> node2
directedGraph.AddEdge(edge2);  // node2 -> node3
directedGraph.AddEdge(edge3);  // node1 -> node4
directedGraph.AddEdge(edge4);  // node1 -> node5
directedGraph.AddEdge(edge5);  // node4 -> node5
directedGraph.AddEdge(edge6);  // node3 -> node5
```

## Running Algorithms

### Depth-First Search (DFS)

DFS traverses the graph depth-first and records discovery and finish times:

```csharp
DFS dfsDirected = new DFS("dfs1");
dfsDirected.SetGraph(directedGraph);
dfsDirected.Execute();
```

### Breadth-First Search (BFS)

BFS requires both a graph and a starting node:

```csharp
BFS bfsDirected = new BFS("bfs1");
bfsDirected.SetGraph(directedGraph);
bfsDirected.SetSource(node1);
bfsDirected.Execute();
```

### Working with Weighted Graphs

To use algorithms that require weights (like Bellman-Ford), you first need to add weights to edges:

#### Step 1: Create Sample Weights

The library provides a utility algorithm to add random weights:

```csharp
CreateSampleWeightsAlgorithm SW = new CreateSampleWeightsAlgorithm();
SW.SetGraph(graph);
SW.Execute();
```

This adds random weights (1-10) to all edges in the graph.

#### Step 2: Run Bellman-Ford Algorithm

The Bellman-Ford algorithm finds shortest paths from a source node:

```csharp
BellmanFord bellmanFord = new BellmanFord();
bellmanFord.SetGraph(graph);
bellmanFord.SetStart(node1);
bellmanFord.RegisterInput("WEIGHT", SW, SW.K_WEIGHT);
bellmanFord.Execute();
```

The `RegisterInput` method is crucial - it tells Bellman-Ford where to find the weight data that was added by the `CreateSampleWeightsAlgorithm`.

## Understanding the Data Provider Pattern

The library uses a data provider pattern to share data between algorithms:

1. **Data Provider**: Algorithms that produce data (like `CreateSampleWeightsAlgorithm`) implement `IDataProvider`
2. **Data Consumer**: Algorithms that need data (like `BellmanFord`) implement `IDataConsumer`
3. **Registration**: Consumers register with providers using `RegisterInput(consumerKey, provider, providerKey)`

This pattern ensures:
- Algorithms can share data safely
- Each algorithm instance has unique keys for its data
- Algorithms can verify that required data exists before executing

## Key Concepts

### Graph Elements
- **Nodes**: Basic vertices with unique IDs and metadata storage
- **Edges**: Connections between nodes (directed or undirected)
- **Metadata**: Key-value storage on nodes, edges, and graphs for algorithm results

### Algorithm Results
Algorithms store their results as metadata on graph elements:
- DFS stores: color, discovery time, finish time
- BFS stores: color, distance, parent, paths
- Bellman-Ford stores: distance, parent, paths

### Storage Patterns
The library separates graph logic from storage implementation:
- `IGraphStorage`: Base storage interface
- `IDirectedGraphStorage`: Adds methods for directed graphs
- `IUndirectedGraphStorage`: Adds methods for undirected graphs

This separation allows for different storage implementations (adjacency list, matrix, etc.) without changing the graph API.

## Best Practices

1. **Always add nodes before edges**: The library validates that edge endpoints exist
2. **Check algorithm requirements**: Some algorithms need specific inputs (e.g., BFS needs a source node)
3. **Use the data provider pattern**: When chaining algorithms, properly register data dependencies
4. **Handle exceptions**: The library throws exceptions for invalid operations (e.g., adding duplicate nodes)

## Next Steps

The example code includes commented sections for:
- Topological sorting
- Graph visualization with Graphviz
- Creating visual representations of your graphs

Uncomment these sections to explore additional features of the library.
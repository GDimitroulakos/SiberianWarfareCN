using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Algorithm {
    public class Algorithm {
        // The Algorithm class should have keyed inputs and outputs.
        // It should also have a method to execute the algorithm.
        // Inputs are generic data that are required to run the algorithm.
        // Outputs are the results of the algorithm execution.
        private Dictionary<string, object> inputs;
        private Dictionary<string, object> outputs;

        // If the algorithm process graphs it will also require keys to 
        // information on input graph nodes, edges and the graph. This 
        // information will not have to be stored inside the algorithm
        // because it will be accessible by multiple algorithms through the
        // keys.

        // The graph algorithm processes nodes, edges and the graph. Nodes contain
        // information local to nodes, edges contain information local to edges
        // and the graph contains information local to the graph. Node information
        // is stored under a key that is necessary to access the node information.
        // As the algorithm processes the graph, it will produce temporary information
        // into the nodes, edges and graph. This information will be stored under
        // a key and it will not be necessary to be store after the algorithm 
        // execution is finished. The information is identified using the graph 
        // the key and the element which is stored (graph,key,graphelement)


        // As the algorithm processes the graph, it traverses the nodes and edges
        // and it looks for information that the node and edge has the interface to 
        // provide it. The algorithm requires specific information that is specific
        // to the algorithm and comes with input graphs. There must be a link between
        // the type of data required by the algorithm and the data that is provided
        // by the graph. Thus, the type of data required by the algorithm must linked
        // with a key string to access the data. 
        // ("ALGORITHM INPUT1", KEY1) -> DATA
        // accessing of node information node.Info<DataType>(keys[ALGORITHM INPUT1]) 
        // where from graph algorithm initialization keys[ALGORITHM INPUT1] = KEY1;
        // initialization can be done using a lambda expression, a configuration file
        // or business logic

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Algorithm {
    /// <summary>
    /// Maps algorithm input names to actual data keys in the graph
    /// </summary>
    public class DataMapping {
        /// <summary>
        /// ===PURPOSE=== Dictionary to hold the mapping between algorithm input parameter names
        /// and actual data access keys.The first parameter is formal parameter name
        /// while the second parameter is the actual key used to access data in the graph.
        /// ===EXAMPLE=== When algorithm wants "EDGE_WEIGHT", this may correspond to the access key
        /// for input graph like "road_capacity". 
        /// </summary>
        private readonly Dictionary<string, string> algorithmInputToKey;

        public DataMapping() {
            algorithmInputToKey = new Dictionary<string, string>();
        }

        /// <summary>
        /// ===PURPOSE=== Map an algorithm input parameter to a specific data access key
        /// ===Example===: Map("EDGE_WEIGHT", "road_capacity") creates the link between the
        /// formal parameter EDGE_WEIGHT and the actual data key "road_capacity". 
        /// </summary>
        public DataMapping Map(string algorithmInput, string dataKey) {
            algorithmInputToKey[algorithmInput] = dataKey;
            return this;
        }

        /// <summary>
        /// ===PURPOSE=== Get the actual data key for an algorithm input
        /// ===EXAMPLE=== When algorithm wants "EDGE_WEIGHT", this returns
        /// the real key like "road_capacity".
        /// </summary>
        public string GetKey(string algorithmInput) {
            return algorithmInputToKey.TryGetValue(algorithmInput, 
                out var key) ? key : null;
        }

        /// <summary>
        /// ===PURPOSE=== Check if mapping exists for algorithm input
        /// ===EXAMPLE===: If algorithm input "EDGE_WEIGHT" is mapped to a key,
        /// </summary>
        public bool HasMapping(string algorithmInput) {
            return algorithmInputToKey.ContainsKey(algorithmInput);
        }
    }

    /// <summary>
    /// ===PURPOSE=== Extended graph element interface with typed data access
    /// 
    /// 
    /// 
    /// 
    /// </summary>
    public static class GraphElementDataExtensions {

        /// <summary>
        /// Get typed data from graph element using key
        /// 
        /// YOUR REQUEST: "node.Info<DataType>(keys[ALGORITHM INPUT1])"
        /// 
        /// UTILIZATION: This IS the Info<DataType>() method you described.
        /// Usage: node.Info<double>("road_capacity") returns typed data.
        /// </summary>
        public static T Info<T>(this IGraphElement element, string key) {
            if (element.MetaData.TryGetValue(key, out var value) && value is T typedValue) {
                return typedValue;
            }
            return default(T);
        }

        /// <summary>
        /// Set typed data on graph element
        /// </summary>
        public static void SetInfo<T>(this IGraphElement element, string key, T value) {
            element.MetaData[key] = value;
        }

        /// <summary>
        /// Check if graph element has data for key
        /// </summary>
        public static bool HasInfo(this IGraphElement element, string key) {
            return element.MetaData.ContainsKey(key);
        }
    }
}

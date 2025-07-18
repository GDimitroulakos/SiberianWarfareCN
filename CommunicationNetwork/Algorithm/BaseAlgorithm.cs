using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Algorithm {

    public interface IDataProvider {
        object GetDatakey(string key);
    }

    public interface IDataConsumer {
        void RegisterInput(IGraph graph,string inputkey,
            IDataProvider provider, string PublicProviderkey);
    }

    
    public class InfoKey {
        public enum DIRECTION {
            INPUT,
            OUTPUT
        }
        public string InfoOwner { get; init; } 
        public string AttributeKeyID { get; init; }
        public string KeyDescription { get; init; }
        public DIRECTION Direction { get; init; }
        public string ToStringKeyValue<T>(T value) {
            return $"{AttributeKeyID} : {value.ToString()}";
        }
        public InfoKey(string keyID, string keyDescription, DIRECTION direction, string infoOwner) {
            AttributeKeyID = keyID;
            KeyDescription = keyDescription;
            Direction = direction;
            InfoOwner = infoOwner;
        }
    }

    public abstract class BaseAlgorithm {

        public string AlgorithmName { get; init; }

        public abstract void Execute();
        public abstract void Initialize();

        public BaseAlgorithm(string name) {
            AlgorithmName = name;
        }
    }
}

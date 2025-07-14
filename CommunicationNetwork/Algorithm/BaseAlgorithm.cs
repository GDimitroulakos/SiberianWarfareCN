using CommunicationNetwork.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunicationNetwork.Algorithm {

    public interface IDataProvider {
        object GetDatakey(string key);
    }

    public interface IDataConsumer {
        void RegisterInput(string PublicConsumerKey,
            IDataProvider provider, string PublicProviderkey);
    }

    public abstract class BaseAlgorithm {
        
        public abstract void Execute();
        public abstract void Initialize();

    }
}

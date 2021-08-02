using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    /// <summary>
    /// Handles the data step of the sync process
    /// </summary>
    public class SyncContextIterationData : ISyncContextData
    {
        private readonly OpaClientEmbedded _opaClientEmbedded;
        private readonly List<SyncPolicy> _policies;
        private readonly OpaTransaction _opaTransaction;
        private readonly DataSetNode _tree;

        internal SyncContextIterationData(OpaClientEmbedded opaClientEmbedded, List<SyncPolicy> policies, DataSetNode tree)
        {
            _opaClientEmbedded = opaClientEmbedded;
            _opaTransaction = _opaClientEmbedded.OpaStore.NewTransaction(true);
            _policies = policies;
            _tree = tree;
        }

        public IReadOnlyDictionary<string, DataSetNode> DataSets => _tree.Children;

        public DataSetNode RootDataNode => _tree;

        public void AddData(string path, string content)
        {
            if (!path.StartsWith("/"))
            {
                throw new InvalidOperationException($"Path must start with '/', but got: '{path}'");
            }
            _opaTransaction.WriteJson(path, content);
        }

        /// <summary>
        /// Mark that the sync iteration is done, and the changes will be propagated to the embedded OPA server
        /// </summary>
        public virtual void Done()
        {
            //Write the policies into the store
            foreach (var usedPolicy in _policies)
            {
                _opaTransaction.UpsertPolicy(usedPolicy.PolicyName, usedPolicy.Raw);
            }

            //Commit all the data changes and policies
            _opaTransaction.Commit();
        }
    }
}

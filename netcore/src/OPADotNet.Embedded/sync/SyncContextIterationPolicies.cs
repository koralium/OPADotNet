using OPADotNet.Ast.Models;
using OPADotNet.Embedded.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    public class SyncContextIterationPolicies : ISyncContextPolicies
    {
        private readonly OpaClientEmbedded _opaClientEmbedded;
        private readonly List<SyncPolicyDescriptor> _modules;
        private readonly List<Policy> _addedPolicies = new List<Policy>();
        private readonly Dictionary<string, SyncPolicy> _existingPolices;

        internal SyncContextIterationPolicies(OpaClientEmbedded opaClientEmbedded, List<SyncPolicyDescriptor> modules, Dictionary<string, SyncPolicy> existingPolices)
        {
            _opaClientEmbedded = opaClientEmbedded;
            _modules = modules;
            _existingPolices = existingPolices;
        }

        /// <summary>
        /// Take in a raw policy and get a compiled policy with an AST tree.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="rawText"></param>
        /// <returns></returns>
        public Policy CompilePolicy(string fileName, string rawText)
        {
            return PolicyUtils.GetPolicy(fileName, rawText);
        }

        public void AddPolicy(Policy policy)
        {
            _addedPolicies.Add(policy);
        }

        public void AddPolicies(IEnumerable<Policy> policies)
        {
            _addedPolicies.AddRange(policies);
        }

        /// <summary>
        /// Move to the next step in the sync process
        /// </summary>
        public SyncContextIterationData Next()
        {
            var (usedPolicies, dataSets) = GetUsedPoliciesAndGetDataSets(_addedPolicies);
            var tree = DataSetTreeBuilder.BuildDataSetTree(dataSets);
            return new SyncContextIterationData(_opaClientEmbedded, usedPolicies, tree);
        }

        /// <summary>
        /// Adds policies to the sync, and returns a set of datasets required by the policies to function
        /// </summary>
        /// <param name="policies"></param>
        /// <returns></returns>
        private (List<SyncPolicy>, IReadOnlyCollection<string>) GetUsedPoliciesAndGetDataSets(List<Policy> policies)
        {
            var syncPolicies = GetSyncPolicies(policies);

            HashSet<string> referencedPolicies = new HashSet<string>();
            HashSet<string> dataSets = new HashSet<string>();
            foreach (var module in _modules)
            {
                var moduleName = RemoveData(module.PolicyName);
                if (!syncPolicies.TryGetValue(moduleName, out var syncPolicy))
                {
                    continue;
                }
                module.Found = true; //Mark the policy as found
                var referencedDataSets = FindDataSets(syncPolicy, referencedPolicies, syncPolicies);
                referencedDataSets.RemoveWhere(x => x.StartsWith(module.Unknown)); //Remove the unknown from the referenced datasets
                dataSets.UnionWith(referencedDataSets);
            }

            var usedPolicies = GetReferencedPolicies(referencedPolicies, syncPolicies);
            return (usedPolicies, dataSets);
        }

        private string RemoveData(string name)
        {
            if (name.StartsWith("data."))
            {
                return name[5..];
            }
            return name;
        }

        private Dictionary<string, SyncPolicy> GetSyncPolicies(List<Policy> policies)
        {
            var visitor = new SyncPolicyVisitor();

            foreach (var policy in policies)
            {
                var syncPolicy = visitor.Visit(policy.Ast);
                syncPolicy.Raw = policy.Raw;

                if (_existingPolices.ContainsKey(syncPolicy.PolicyName))
                {
                    _existingPolices[syncPolicy.PolicyName] = syncPolicy;
                }
                else
                {
                    _existingPolices.Add(syncPolicy.PolicyName, syncPolicy);
                }
            }

            return _existingPolices;
        }

        private HashSet<string> FindDataSets(SyncPolicy policy, HashSet<string> visitedPolicies, IReadOnlyDictionary<string, SyncPolicy> policies)
        {
            if (visitedPolicies.Contains(policy.PolicyName))
            {
                return new HashSet<string>();
            }

            visitedPolicies.Add(policy.PolicyName);
            HashSet<string> datasets = new HashSet<string>(policy.DataSets);
            foreach (var dataSet in policy.DataSets)
            {
                var referencedPolicy = policies.FirstOrDefault(x => dataSet.StartsWith(x.Key));
                if (referencedPolicy.Value != null)
                {
                    var otherPolicy = referencedPolicy.Value;
                    var otherDatasets = FindDataSets(otherPolicy, visitedPolicies, policies);
                    foreach (var otherDataSet in otherDatasets)
                    {
                        datasets.Add(otherDataSet);
                    }
                    //Remove the policy as a dataset
                    datasets.RemoveWhere(x => x.StartsWith(referencedPolicy.Key));
                }
            }

            return datasets;
        }

        private List<SyncPolicy> GetReferencedPolicies(IEnumerable<string> policies, IReadOnlyDictionary<string, SyncPolicy> policyLookup)
        {
            List<SyncPolicy> output = new List<SyncPolicy>();
            foreach (var policyName in policies)
            {
                var moduleName = policyName;
                if (moduleName.StartsWith("data."))
                {
                    moduleName = policyName[5..];
                }
                if (policyLookup.TryGetValue(moduleName, out var syncPolicy))
                {
                    output.Add(syncPolicy);
                }
                else
                {
                    throw new InvalidOperationException($"No policy exists with the name: '{policyName}'");
                }
            }
            return output;
        }
    }
}

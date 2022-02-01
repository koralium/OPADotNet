using OPADotNet.Ast;
using OPADotNet.Core.Models;
using OPADotNet.Embedded.Internal;
using OPADotNet.Embedded.sync;
using OPADotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.utils
{
    /// <summary>
    /// Utility class to help do a full partial evaluation of a policy.
    /// </summary>
    internal class FullPartialUtils
    {
        public static async Task<PartialResult> FullPartial(OpaClientEmbedded opaClientEmbedded, string query)
        {
            var usedDataSets = GetUsedDataSets((await opaClientEmbedded.GetPolicies()).ToList());

            var dataSets = usedDataSets.Select(x =>
            {
                var index = x.IndexOf(".$0");
                if (index >= 0)
                {
                    return x.Substring(0, index);
                }
                return x;
            }).Select(x => $"data.{x}").ToList();
            dataSets.Add("input");

            var unknowns = dataSets.ToArray();
            int result = RegoWrapper.FullPartial(opaClientEmbedded.OpaStore.GetCompiler().compilerId, opaClientEmbedded.OpaStore._storeId, query, unknowns, unknowns.Length);

            if (result < 0)
            {
                string err = RegoWrapper.GetString(result);
                RegoWrapper.FreeString(result);
                throw new InvalidOperationException(err);
            }

            var content = RegoWrapper.GetString(result);

            return PartialJsonConverter.ReadPartialResult(content);
        }

        private static HashSet<string> GetUsedDataSets(List<Policy> policies)
        {
            var syncPolicies = GetSyncPolicies(policies);
            HashSet<string> dataSets = new HashSet<string>();
            HashSet<string> referencedPolicies = new HashSet<string>();
            foreach (var p in syncPolicies.Values)
            {
                var referencedDataSets = FindDataSets(p, referencedPolicies, syncPolicies);
                dataSets.UnionWith(referencedDataSets);
            }

            return dataSets;
        }

        private static Dictionary<string, SyncPolicy> GetSyncPolicies(List<Policy> policies)
        {
            var visitor = new SyncPolicyVisitor();

            Dictionary<string, SyncPolicy> existingPolices = new Dictionary<string, SyncPolicy>();
            foreach (var policy in policies)
            {
                var syncPolicy = visitor.Visit(policy.Ast);
                syncPolicy.Raw = policy.Raw;

                if (existingPolices.ContainsKey(syncPolicy.PolicyName))
                {
                    existingPolices[syncPolicy.PolicyName] = syncPolicy;
                }
                else
                {
                    existingPolices.Add(syncPolicy.PolicyName, syncPolicy);
                }
            }

            return existingPolices;
        }

        private static HashSet<string> FindDataSets(SyncPolicy policy, HashSet<string> visitedPolicies, IReadOnlyDictionary<string, SyncPolicy> policies)
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
    }
}

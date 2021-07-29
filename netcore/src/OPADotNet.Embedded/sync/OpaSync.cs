using OPADotNet.Ast.Models;
using OPADotNet.RestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.sync
{
    public class OpaSync
    {
        private readonly RestOpaClient restOpaClient;
        private readonly OpaClientEmbedded opaClientEmbedded;
        private readonly List<SyncPolicyDescriptor> modules;
        private readonly TimeSpan syncTime;
        private readonly List<Policy> localModules;

        private Task _task;
        private CancellationTokenSource cancellationTokenSource;

        internal OpaSync(
            RestOpaClient restOpaClient, 
            OpaClientEmbedded opaClientEmbedded, 
            List<SyncPolicyDescriptor> modules,
            TimeSpan syncTime,
            List<Policy> localModules)
        {
            this.restOpaClient = restOpaClient;
            this.opaClientEmbedded = opaClientEmbedded;
            this.modules = modules;
            this.syncTime = syncTime;
            this.localModules = localModules;
        }

        private async Task<IReadOnlyDictionary<string, SyncPolicy>> GetPolicies()
        {
            var policies = await restOpaClient.GetPolicies();
            var visitor = new SyncPolicyVisitor();

            Dictionary<string, SyncPolicy> syncPolicies = new Dictionary<string, SyncPolicy>();
            foreach (var policy in policies)
            {
                var syncPolicy = visitor.Visit(policy.Ast);
                syncPolicy.Raw = policy.Raw;
                syncPolicies.Add(syncPolicy.PolicyName, syncPolicy);
            }

            foreach (var policy in localModules)
            {
                var syncPolicy = visitor.Visit(policy.Ast);
                syncPolicy.Raw = policy.Raw;
                if (syncPolicies.ContainsKey(syncPolicy.PolicyName))
                {
                    syncPolicies[syncPolicy.PolicyName] = syncPolicy;
                }
                else
                {
                    syncPolicies.Add(syncPolicy.PolicyName, syncPolicy);
                }
                
            }

            return syncPolicies;
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
                if (policies.TryGetValue(dataSet, out var otherPolicy))
                {
                    var otherDatasets = FindDataSets(otherPolicy, visitedPolicies, policies);
                    foreach(var otherDataSet in otherDatasets)
                    {
                        datasets.Add(otherDataSet);
                    }
                    //Remove the policy as a dataset
                    datasets.Remove(dataSet);
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

        private string RemoveData(string name)
        {
            if (name.StartsWith("data."))
            {
                return name[5..];
            }
            return name;
        }

        private async Task RunIteration()
        {
            var syncPolicies = await GetPolicies();

            //Get the policies that was added for the sync

            HashSet<string> referencedPolicies = new HashSet<string>();
            HashSet<string> dataSets = new HashSet<string>();
            foreach (var module in modules)
            {
                var moduleName = RemoveData(module.PolicyName);
                if (!syncPolicies.TryGetValue(moduleName, out var syncPolicy))
                {
                    throw new InvalidOperationException($"No policy exists with the name: '{module.PolicyName}'");
                }

                var referencedDataSets = FindDataSets(syncPolicy, referencedPolicies, syncPolicies);
                referencedDataSets.Remove(RemoveData(module.Unknown)); //Remove the unknown from the referenced datasets
                dataSets.UnionWith(referencedDataSets);
            }

            //Get the new list of policies including the references that they use.
            var usedPolicies = GetReferencedPolicies(referencedPolicies, syncPolicies);

            //Write the policies into the store
            var txn = opaClientEmbedded.OpaStore.NewTransaction();
            foreach (var usedPolicy in usedPolicies)
            {
                txn.UpsertPolicy(usedPolicy.PolicyName, usedPolicy.Raw);
            }

            //Fetch all the datasets required for the policies to function

            txn.Commit();
        }

        public async Task Start()
        {
            await RunIteration();
            cancellationTokenSource = new CancellationTokenSource();
            _task = Task.Factory.StartNew(async () =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await Task.Delay(syncTime, cancellationTokenSource.Token);
                    try
                    {
                        await RunIteration();
                    }
                    catch (Exception e)
                    {
                        //Log
                    }
                }
                
            }, TaskCreationOptions.LongRunning)
                .Unwrap();
        }

        public Task Stop()
        {
            if (_task != null)
            {
                cancellationTokenSource.Cancel();
                return _task;
            }
            return Task.CompletedTask;
        }
    }
}

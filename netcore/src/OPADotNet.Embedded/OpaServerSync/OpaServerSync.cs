using OPADotNet.Embedded.sync;
using OPADotNet.RestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Sync
{
    internal class OpaServerSync : SyncServiceBase
    {
        private readonly OpaServerSyncOptions _opaServerSyncOptions;
        private readonly RestOpaClient _restOpaClient;

        public OpaServerSync(OpaServerSyncOptions opaServerSyncOptions)
        {
            _opaServerSyncOptions = opaServerSyncOptions;
            _restOpaClient = new RestOpaClient(opaServerSyncOptions.OpaServerUrl);
        }

        public override async Task BackgroundRun(SyncContext syncContext, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(_opaServerSyncOptions.Interval);
                await FullLoad(syncContext, cancellationToken);
            }
        }

        private async Task GetDataSet(DataSetNode node, ISyncContextData syncContext)
        {
            string path = $"/{node.Name}";
            await GetDataSet(node, path, syncContext);
        }

        private async Task GetDataSet(DataSetNode node, string path, ISyncContextData syncContext)
        {
            // If there is no more children or there is a child that is a variable, end there and get all the data
            if (node.Children.Count == 0 || node.Children.Any(x => x.Value.IsVariable))
            {
                //Fetch data
                await GetData(path, syncContext);
            }

            foreach (var child in node.Children)
            {
                var childPath = $"{path}/{child.Key}";
                await GetDataSet(child.Value, childPath, syncContext);
            }
        }

        private async Task GetData(string path, ISyncContextData syncContext)
        {
            var content = await _restOpaClient.GetDataJson(path);

            if (content.Equals("{}"))
            {
                return;
            }

            syncContext.AddData(path, content);
        }

        public override async Task LoadPolices(ISyncContextPolicies syncContextPolicies, CancellationToken cancellationToken)
        {
            var policies = await _restOpaClient.GetPolicies();
            syncContextPolicies.AddPolicies(policies);
        }

        public override async Task LoadData(ISyncContextData syncContextData, CancellationToken cancellationToken)
        {
            foreach (var dataSet in syncContextData.DataSets)
            {
                await GetDataSet(dataSet.Value, syncContextData);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace OPADotNet.Embedded.Sync
{
    public class LocalSyncOptions
    {
        internal List<string> Policies { get; } = new List<string>();

        internal List<LocalData> LocalDatas { get; } = new List<LocalData>();

        public LocalSyncOptions AddPolicy(string policyText)
        {
            Policies.Add(policyText);
            return this;
        }

        public LocalSyncOptions AddData(string path, string content)
        {
            LocalDatas.Add(new LocalData(path, content));
            return this;
        }

        public LocalSyncOptions AddData<T>(string path, T input)
        {
            string content = JsonSerializer.Serialize(input);
            return AddData(path, content);
        }
    }
}

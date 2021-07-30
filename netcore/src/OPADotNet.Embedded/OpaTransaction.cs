using OPADotNet.Embedded.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OPADotNet.Embedded
{
    public class OpaTransaction : IDisposable
    {
        private class PolicyTransaction
        {
            public string PolicyName { get; set; }
            public string PolicyData { get; set; }
        }

        private readonly OpaStore _store;
        private readonly int _transactionId;
        private readonly List<PolicyTransaction> upsertPolicies = new List<PolicyTransaction>(); 
        private bool disposedValue;
        

        internal OpaTransaction(OpaStore store, int transactionId)
        {
            _store = store;
            _transactionId = transactionId;
        }

        public void Commit()
        {
            int result = RegoWrapper.CommitTransaction(_store._storeId, _transactionId);

            if (result < 0)
            {
                var error = RegoWrapper.GetString(result);
                throw new InvalidOperationException(error);
            }

            Dictionary<string, string> newModules = new Dictionary<string, string>(_store._modules);
            foreach (var policy in upsertPolicies)
            {
                if (newModules.ContainsKey($"{policy.PolicyName}.rego"))
                {
                    newModules[$"{policy.PolicyName}.rego"] = policy.PolicyData;
                }
                else
                {
                    newModules.Add($"{policy.PolicyName}.rego", policy.PolicyData);
                }
            }

            //Check if the modules has changed, then create a new compiler
            if (!new DictionaryComparer<string, string>().Equals(newModules, _store._modules))
            {
                _store._modules = newModules;
                _store.NewCompiler();
            }
            

            upsertPolicies.Clear();

            Dispose(disposing: false);
        }

        public void Write(string path, object data)
        {
            var jsonData = JsonSerializer.Serialize(data);
            RegoWrapper.WriteToStore(_store._storeId, _transactionId, path, jsonData);
        }

        public void WriteJson(string path, string json)
        {
            int result = RegoWrapper.WriteToStore(_store._storeId, _transactionId, path, json);

            if (result < 0)
            {
                var error = RegoWrapper.GetString(result);
                throw new InvalidOperationException(error);
            }
        }

        public string Read(string path)
        {
            var result = RegoWrapper.ReadFromStore(_store._storeId, _transactionId, path);

            if (result < 0)
            {
                var error = RegoWrapper.GetString(result);
                throw new InvalidOperationException(error);
            }

            var content = RegoWrapper.GetString(result);
            return content;
        }

        public T Read<T>(string path)
        {
            var str = Read(path);
            return JsonSerializer.Deserialize<T>(str);
        }

        public void UpsertPolicy(string policyName, string policyData)
        {
            int upsertResult = RegoWrapper.UpsertPolicy(_store._storeId, _transactionId, policyName, policyData);

            if (upsertResult != 0)
            {
                var error = RegoWrapper.GetString(upsertResult);
                throw new InvalidOperationException(error);
            }

            upsertPolicies.Add(new PolicyTransaction()
            {
                PolicyName = policyName,
                PolicyData = policyData
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                RegoWrapper.RemoveTransaction(_transactionId);
                disposedValue = true;
            }
        }

        ~OpaTransaction()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

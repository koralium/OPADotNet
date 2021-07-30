using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.sync
{
    public interface ISyncContextData
    {
        void AddData(string path, string content);

        IReadOnlyDictionary<string, DataSetNode> DataSets { get; }

        DataSetNode RootDataNode { get; }
    }
}

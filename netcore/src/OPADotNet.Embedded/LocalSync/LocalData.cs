using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.Sync
{
    internal class LocalData
    {
        public string Path { get; }

        public string Content { get; }

        public LocalData(string path, string content)
        {
            Path = path;
            Content = content;
        }
    }
}

﻿/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.Sync
{
    class LocalTarGzSync : TarGzSync
    {
        private readonly string _filePath;

        public LocalTarGzSync(string filePath)
        {
            _filePath = filePath;
        }

        public override Task BackgroundRun(SyncContext syncContext, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public override async Task GetTarGzStream(Func<Stream, Task> addStream)
        {
            using var stream = File.OpenRead(_filePath);
            await addStream(stream);
        }
    }
}

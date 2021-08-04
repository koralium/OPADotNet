/*
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
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OPADotNet.Embedded.sync
{
    public interface ISyncService
    {

        Task Initialize(IServiceProvider serviceProvider);

        /// <summary>
        /// This function must load in all the data from the sync source, the task should return when the data is loaded.
        /// </summary>
        /// <param name="syncContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task FullLoad(SyncContext syncContext, CancellationToken cancellationToken);

        Task LoadPolices(ISyncContextPolicies syncContextPolicies, CancellationToken cancellationToken);

        Task LoadData(ISyncContextData syncContextData, CancellationToken cancellationToken);

        Task BackgroundRun(SyncContext syncContext, CancellationToken cancellationToken);
    }
}

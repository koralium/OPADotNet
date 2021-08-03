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
using Microsoft.Extensions.DependencyInjection;
using OPADotNet.Embedded.sync;
using System;
using System.Collections.Generic;
using System.Text;

namespace OPADotNet.Embedded.Sync
{
    /// <summary>
    /// Internal sync holder that contains the sync services, used to keep the ordering correct.
    /// </summary>
    internal abstract class SyncServiceHolder
    {
        public Guid Id { get; }

        protected SyncServiceHolder()
        {
            Id = Guid.NewGuid();
        }

        public abstract void AddToServices(IServiceCollection services);

        public abstract ISyncService GetService(IServiceProvider serviceProvider);
    }

    internal class SyncServiceHolderObject : SyncServiceHolder
    {
        private readonly ISyncService _service;

        public SyncServiceHolderObject(ISyncService service)
        {
            _service = service;
        }

        public override void AddToServices(IServiceCollection services)
        {
            //Nothing required
        }

        public override ISyncService GetService(IServiceProvider serviceProvider)
        {
            return _service;
        }
    }

    internal class SyncServiceHolderType : SyncServiceHolder
    {
        private readonly Type _type;

        public SyncServiceHolderType(Type type)
        {
            _type = type;
        }

        public override void AddToServices(IServiceCollection services)
        {
            services.AddSingleton(_type);
        }

        public override ISyncService GetService(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetService(_type) as ISyncService;
        }
    }
}

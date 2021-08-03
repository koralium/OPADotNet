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
using OPADotNet.Embedded.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.AspNetCore.Builder
{
    internal class SyncBuilder : ISyncBuilder
    {
        private readonly List<SyncServiceHolder> _syncServices = new List<SyncServiceHolder>();

        public IServiceCollection Services { get; }

        public SyncBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public ISyncBuilder AddSyncService<T>(T service) where T : ISyncService
        {
            _syncServices.Add(new SyncServiceHolderObject(service));
            return this;
        }

        public ISyncBuilder AddSyncService<T>() where T : ISyncService
        {
            return AddSyncService(typeof(T));
        }

        public ISyncBuilder AddSyncService(Type type)
        {
            if (!typeof(ISyncService).IsAssignableFrom(type))
            {
                throw new ArgumentException($"'{type.Name}' does not implement the ISyncService interface", nameof(type));
            }
            _syncServices.Add(new SyncServiceHolderType(type));
            return this;
        }

        internal SyncOptions Build()
        {
            return new SyncOptions(_syncServices.ToList());
        }
    }
}

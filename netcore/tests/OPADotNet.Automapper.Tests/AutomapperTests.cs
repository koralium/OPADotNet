using AutoMapper;
using AutoMapper.Configuration;
using AutoMapper.Extensions.ExpressionMapping;
using AutoMapper.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OPADotNet.AspNetCore;
using OPADotNet.AspNetCore.Builder;
using OPADotNet.Embedded.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OPADotNet.Automapper.Tests
{
    public class AutomapperTests
    {
        private IMapper SetupOpaWithMapper(string policy, Action<IMapperConfigurationExpression> mapperConfig, Action<AuthorizationOptions> authOptions)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddLogging();
            services.AddAutoMapper(opt =>
            {
                mapperConfig(opt);
            });

            services.AddOpa(opt =>
            {
                opt.AddAutomapperSupport<TestUserProvider>();
                opt.AddSync(b =>
                {
                    b.UseLocal(l =>
                    {
                        l.AddPolicy(policy);
                    });
                });
            });

            services.AddAuthorization(o =>
            {
                authOptions(o);
            });

            var serviceProvider = services.BuildServiceProvider();

            var opaWorker = new OpaWorker(
                serviceProvider.GetRequiredService<PreparedPartialStore>(),
                serviceProvider.GetRequiredService<OpaOptions>(), 
                serviceProvider,
                serviceProvider.GetRequiredService<DiscoveryHandler>(),
                serviceProvider.GetRequiredService<ILogger<OpaWorker>>());
            opaWorker.StartAsync(default).Wait();

            return serviceProvider.GetRequiredService<IMapper>();
        }

        private class TestDbModel
        {
            public string DbName { get; set; }
        }

        private class TestModel
        {
            public string Name { get; set; }

            public bool PolicyValue { get; set; }
        }

        [Test]
        public void TestAddAutoMapperSupportWithoutAutomapperThrowsError()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                ServiceCollection services = new ServiceCollection();
                services.AddOpa(o =>
                {
                    o.AddAutomapperSupport();
                });
            });
            Assert.AreEqual("AutoMapper must be added to services before Opa AutoMapper support can be added.", exception.Message);
        }

        [Test]
        public void TestMapFromSource()
        {
            var mapper = SetupOpaWithMapper(
                @"
                package test
                            
                allow {
                    some i
                    input.subject
                    data.testdata[i].dbname = ""test""
                }",
                m => m.CreateMap<TestDbModel, TestModel>(AutoMapper.MemberList.Destination)
                    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.DbName))
                    .ForMember(x => x.PolicyValue, opt => opt.MapFromPolicy("test")),
                a => a.AddPolicy("test", r => r.RequireOpaPolicy("test", "testdata", "GET"))
                );

            var mapped = mapper.Map<TestModel>(new TestDbModel() { DbName = "test" });
            Assert.IsTrue(mapped.PolicyValue);

            mapped = mapper.Map<TestModel>(new TestDbModel() { DbName = "test2" });
            Assert.IsFalse(mapped.PolicyValue);
        }

        [Test]
        public void TestMapFromDestination()
        {
            var mapper = SetupOpaWithMapper(
                @"
                package test
                            
                allow {
                    some i
                    input.subject
                    data.testdata[i].name = ""test""
                }",
                m => m.CreateMap<TestDbModel, TestModel>(AutoMapper.MemberList.Destination)
                    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.DbName))
                    .ForMember(x => x.PolicyValue, opt => opt.MapFromPolicyBasedOnDestination("test")),
                a => a.AddPolicy("test", r => r.RequireOpaPolicy("test", "testdata", "GET"))
                );

            var mapped = mapper.Map<TestModel>(new TestDbModel() { DbName = "test" });
            Assert.IsTrue(mapped.PolicyValue);

            mapped = mapper.Map<TestModel>(new TestDbModel() { DbName = "test2" });
            Assert.IsFalse(mapped.PolicyValue);
        }

        [Test]
        public void TestProjectToFromSource()
        {
            var mapper = SetupOpaWithMapper(
                @"
                package test
                            
                allow {
                    some i
                    input.subject
                    data.testdata[i].dbname = ""test""
                }",
                m => m.CreateMap<TestDbModel, TestModel>(AutoMapper.MemberList.Destination)
                    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.DbName))
                    .ForMember(x => x.PolicyValue, opt => opt.MapFromPolicy("test")),
                a => a.AddPolicy("test", r => r.RequireOpaPolicy("test", "testdata", "GET"))
                );

            var list = new List<TestDbModel>()
            {
                new TestDbModel() { DbName = "test" },
                new TestDbModel() { DbName = "test2" }
            };

            var actual = mapper.ProjectTo<TestModel>(list.AsQueryable()).Select(x => x.PolicyValue).ToList();

            Assert.AreEqual(new List<bool>() { true, false }, actual);
        }

        [Test]
        public void TestProjectToFromDestination()
        {
            var mapper = SetupOpaWithMapper(
                @"
                package test
                            
                allow {
                    some i
                    input.subject
                    data.testdata[i].name = ""test""
                }",
                m => m.CreateMap<TestDbModel, TestModel>(AutoMapper.MemberList.Destination)
                    .ForMember(x => x.Name, opt => opt.MapFrom(src => src.DbName))
                    .ForMember(x => x.PolicyValue, opt => opt.MapFromPolicyBasedOnDestination("test")),
                a => a.AddPolicy("test", r => r.RequireOpaPolicy("test", "testdata", "GET"))
                );

            var list = new List<TestDbModel>()
            {
                new TestDbModel() { DbName = "test" },
                new TestDbModel() { DbName = "test2" }
            };

            var actual = mapper.ProjectTo<TestModel>(list.AsQueryable()).Select(x => x.PolicyValue).ToList();

            Assert.AreEqual(new List<bool>() { true, false }, actual);
        }
    }
}
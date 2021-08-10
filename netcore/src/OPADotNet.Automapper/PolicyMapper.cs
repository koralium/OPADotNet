using AutoMapper;
using ExpressionPowerTools.Core.Hosts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ExpressionPowerTools.Core.Extensions;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.AspNetCore.Authorization;
using OPADotNet.Automapper.Internal;

namespace OPADotNet.Automapper
{
    public class PolicyMapper : IMapper
    {
        private readonly IMapper _mapper;
        public PolicyMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IConfigurationProvider ConfigurationProvider => _mapper.ConfigurationProvider;

        public Func<Type, object> ServiceCtor => _mapper.ServiceCtor;

        public TDestination Map<TDestination>(object source, Action<IMappingOperationOptions<object, TDestination>> opts)
        {
            return _mapper.Map(source, opts);
        }

        public TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> opts)
        {
            return _mapper.Map(source, opts);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> opts)
        {
            return _mapper.Map(source, destination, opts);
        }

        public object Map(object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions<object, object>> opts)
        {
            return _mapper.Map(source, sourceType, destinationType);
        }

        public object Map(object source, object destination, Type sourceType, Type destinationType, Action<IMappingOperationOptions<object, object>> opts)
        {
            return _mapper.Map(source, destination, sourceType, destinationType, opts);
        }

        public TDestination Map<TDestination>(object source)
        {
            return _mapper.Map<TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TSource, TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return _mapper.Map<TSource, TDestination>(source, destination);
        }

        public object Map(object source, Type sourceType, Type destinationType)
        {
            return _mapper.Map(source, sourceType, destinationType);
        }

        public object Map(object source, object destination, Type sourceType, Type destinationType)
        {
            return _mapper.Map(source, destination, sourceType, destinationType);
        }

        public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source, object parameters = null, params Expression<Func<TDestination, object>>[] membersToExpand)
        {
            return ProjectTo_Internal(_mapper.ProjectTo<TDestination>(source, parameters, membersToExpand));
        }

        public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source, IDictionary<string, object> parameters, params string[] membersToExpand)
        {
            return ProjectTo_Internal(_mapper.ProjectTo<TDestination>(source, parameters, membersToExpand));
        }

        public IQueryable ProjectTo(IQueryable source, Type destinationType, IDictionary<string, object> parameters = null, params string[] membersToExpand)
        {
            return _mapper.ProjectTo(source, destinationType, parameters, membersToExpand);
        }

        private IQueryable<TDestination> ProjectTo_Internal<TDestination>(IQueryable<TDestination> queryable)
        {
            var authService = _mapper.ServiceCtor(typeof(IAuthorizationService)) as IAuthorizationService;
            var userProvider = _mapper.ServiceCtor(typeof(IOpaAutoMapperUserProvider)) as IOpaAutoMapperUserProvider;

            if (authService == null)
            {
                throw new InvalidOperationException("Could not get IAuthorizationService.");
            }
            if (userProvider == null)
            {
                throw new InvalidOperationException("Could not get IOpaAutoMapperUserProvider.");
            }

            return queryable.CreateInterceptedQueryable(exp =>
            {
                var containsOpaFilterVisitor = new ContainsOpaFilterVisitor();
                containsOpaFilterVisitor.Visit(exp);
                if (containsOpaFilterVisitor.ContainsOpaFilter)
                {
                    return new ExpressionReplacementVisitor(_mapper, authService, userProvider, typeof(TDestination)).Visit(exp);
                }
                return exp;
            });
        }
    }
}

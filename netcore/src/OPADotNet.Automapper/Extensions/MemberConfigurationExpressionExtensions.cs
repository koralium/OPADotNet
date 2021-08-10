using AutoMapper;
using OPADotNet.Automapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMapper
{
    public static class MemberConfigurationExpressionExtensions
    {

        private static bool FromPolicy(string name)
        {
            return false;
        }

        private static bool FromPolicyBasedOnDestination(string name)
        {
            return false;
        }

        public static void MapFromPolicy<TSource, TDestination>(this IMemberConfigurationExpression<TSource, TDestination, bool> memberConfigurationExpression, string policyName)
        {
            memberConfigurationExpression.SetMappingOrder(int.MaxValue); //Set the mapping order to last so the other values will be set for destination.

            //Add mapping to be used during runtime
            memberConfigurationExpression.MapFrom(new PolicyValueResolver<TSource, TDestination, object>(policyName, false),(x) => FromPolicy(policyName));

            //Add mapping to be used for projectTo.
            memberConfigurationExpression.MapFrom(src => FromPolicy(policyName));
        }

        /// <summary>
        /// Uses the destination type instead of the source type when resolving property names from the policy.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="memberConfigurationExpression"></param>
        /// <param name="policyName"></param>
        public static void MapFromPolicyBasedOnDestination<TSource, TDestination>(this IMemberConfigurationExpression<TSource, TDestination, bool> memberConfigurationExpression, string policyName)
        {
            memberConfigurationExpression.SetMappingOrder(int.MaxValue); //Set the mapping order to last so the other values will be set for destination.

            //Add mapping to be used during runtime
            memberConfigurationExpression.MapFrom(new PolicyValueResolver<TSource, TDestination, object>(policyName, true), (x) => FromPolicyBasedOnDestination(policyName));

            //Add mapping to be used for projectTo.
            memberConfigurationExpression.MapFrom(src => FromPolicyBasedOnDestination(policyName));
        }
    }
}

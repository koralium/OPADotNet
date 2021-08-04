using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPADotNet.AspNetCore
{
    public class AuthorizeQueryableResult<T>
    {
        public IQueryable<T> Queryable { get; }

        public bool Succeeded { get; }

        public AuthorizationFailure Failure { get; }

        internal AuthorizeQueryableResult(AuthorizationResult authorizationResult, IQueryable<T> queryable)
        {
            //AuthorizationResult = authorizationResult;
            Queryable = queryable;
            Succeeded = authorizationResult.Succeeded;
            Failure = authorizationResult.Failure;
        }
    }
}

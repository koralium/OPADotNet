using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace OPADotNet.TestFramework
{
    class InternalControllerFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            var isInternalController = !typeInfo.IsAbstract && typeof(ControllerBase).IsAssignableFrom(typeInfo);
            return isInternalController || base.IsController(typeInfo);
        }
    }
}

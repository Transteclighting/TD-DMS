using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace Cj.web.protal.App_Start
{
    public class RemoveAuthenticationFilter : FilterAttribute, IOverrideFilter
    {
            public Type FiltersToOverride
            {
                get
                {
                    return typeof(AuthenticationFilter);
                }
            }
    }
}
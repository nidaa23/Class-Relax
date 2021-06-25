using System;
using System.Web.Mvc;
using MvcAuthorization;

[assembly: WebActivatorEx.PreApplicationStartMethod(
    typeof(Class_Relax.App_Start.MvcAuthorization), "PreStart")]

namespace Class_Relax.App_Start {
    public static class MvcAuthorization {
        public static void PreStart() {
            GlobalFilters.Filters.Add(new AuthorizeFilter());
        }
    }
}
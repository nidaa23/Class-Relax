using System;
using System.Web.Mvc;
using MvcAuthorization;

[assembly: WebActivatorEx.PreApplicationStartMethod(
    typeof($rootnamespace$.App_Start.MvcAuthorization), "PreStart")]

namespace $rootnamespace$.App_Start {
    public static class MvcAuthorization {
        public static void PreStart() {
            GlobalFilters.Filters.Add(new AuthorizeFilter());
        }
    }
}
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.Conventions;
using Nancy.TinyIoc;
using Nancy.ViewEngines;
using System;
using System.Collections.Generic;
using System.Text;

namespace DnsServer.Web {
    class Bootstrapper : DefaultNancyBootstrapper {
        Database.Database db;

        public Bootstrapper(Database.Database database) {
            db = database;
        }

        protected override void ConfigureApplicationContainer(TinyIoCContainer container) {
            base.ConfigureApplicationContainer(container);
            container.Register<Database.IDatabase>(db);

            ResourceViewLocationProvider.RootNamespaces.Add(typeof(Bootstrapper).Assembly, "DnsServer.Web.Pages");
        }

        protected override Func<ITypeCatalog, NancyInternalConfiguration> InternalConfiguration {
            get {
                return NancyInternalConfiguration.WithOverrides(
                    cfg => {
                        cfg.ViewLocationProvider = typeof(ResourceViewLocationProvider);
                    });
            }
        }

        //protected override void ConfigureConventions(NancyConventions nancyConventions) {
        //    ConfigureViews(nancyConventions);
        //}

        //private static void ConfigureViews(NancyConventions nancyConventions) {
        //    nancyConventions.ViewLocationConventions.Clear();
        //    //nancyConventions.ViewLocationConventions.Insert(0, (view, model, ctx) => "{1}/{0}".Fmt(view, ctx.ModulePath));
        //    nancyConventions.ViewLocationConventions.Add((view, model, ctx) => {
        //        //var tp = (model.GetType() as Type);
        //        //var asm = tp.Assembly.GetName().Name;
        //        //var path = tp.Namespace.Substring(asm.Length + 1).Replace('.', '/');

        //        //path is model namespace without the assembly name
        //        return "{0}/{1}".Fmt("Web/Pages", view);
        //    });
        //}
    }
}

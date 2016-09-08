using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Web.Framework.Mvc;
using Nop.Plugin.Payments.TodoPago.Data;
using Nop.Plugin.Payments.TodoPago.Services;
using Nop.Plugin.Payments.TodoPago.Domain;

namespace Nop.Plugin.Payments.TodoPago.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {

        private const string CONTEXT_NAME = "nop_object_context_todopago_transaction";

        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<TodoPagoTransactionService>().As<ITodoPagoTransactionService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<TodoPagoTransactionObjectContex>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<TodoPagoTransactionRecord>>()
                .As<IRepository<TodoPagoTransactionRecord>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

        }

        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<TodoPagoTransactionService>().As<ITodoPagoTransactionService>().InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<TodoPagoTransactionObjectContex>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<TodoPagoTransactionRecord>>()
                .As<IRepository<TodoPagoTransactionRecord>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 2; }
        }
    }
}

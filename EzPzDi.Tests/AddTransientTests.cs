using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EzPzDi.Tests
{
    [AddTransient]
    public class ExampleConcreteTransient { }

    [AddTransient]
    public class ExampleUnspecifiedTransient : IUnspecifiedInterface, IOtherInterface { }

    [AddTransient(ServiceTypes = new [] { typeof(IExplicitInterface) })]
    public class ExampleSpecifiedTransient : IExplicitInterface, IOtherInterface { }

    [TestClass]
    public class AddTransientTests
    {
        [TestMethod]
        public void WithConcreteServiceTypes()
        {
            var sc = new ServiceCollection()
                .AddEzPzDi();

            var sp = sc.BuildServiceProvider();
            
            Assert.IsNotNull(sp.GetService<ExampleConcreteTransient>());
        }

        [TestMethod]
        public void WithUnspecifiedInterfaceServiceTypes()
        {
            var implementationType = typeof(ExampleUnspecifiedTransient);
            var name = nameof(ExampleUnspecifiedTransient);

            var sc = new ServiceCollection()
                .AddEzPzDi();

            var sp = sc.BuildServiceProvider();

            var found = sp.GetServices<IUnspecifiedInterface>().Where(c => c.GetType() == implementationType);
            Assert.AreEqual(name, found.Single().GetType().Name);

            var otherFound = sp.GetServices<IOtherInterface>().Where(c => c.GetType() == implementationType);
            Assert.AreEqual(name, otherFound.Single().GetType().Name);
        }

        [TestMethod]
        public void WithSpecifiedInterfaceServiceTypes()
        {
            var implementationType = typeof(ExampleSpecifiedTransient);
            var name = nameof(ExampleSpecifiedTransient);

            var sc = new ServiceCollection()
                .AddEzPzDi();

            var actual = sc.Where(s => s.ImplementationType == implementationType
                && s.Lifetime == ServiceLifetime.Transient);
            Assert.AreEqual(1, actual.Count());

            var sp = sc.BuildServiceProvider();

            var found = sp.GetServices<IExplicitInterface>().Where(c => c.GetType() == implementationType);
            Assert.AreEqual(name, found.Single().GetType().Name);

            // IOtherInterface wasn't specifed in the attribute but
            // another was, so it shoudln't show up.
            var otherFound = sp.GetServices<IOtherInterface>().Where(c => c.GetType() == implementationType);
            Assert.AreEqual(0, otherFound.Count());
        }
    }
}
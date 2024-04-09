using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection.Metadata.Ecma335;

namespace EzPzDi.Tests
{
    [AddSingleton]
    public class ExampleConcreteSingleton { }

    [AddSingleton]
    public class ExampleUnspecifiedSingleton : IUnspecifiedInterface, IOtherInterface { }

    [AddSingleton(ServiceTypes = new [] { typeof(IExplicitInterface) })]
    public class ExampleSpecifiedSingleton : IExplicitInterface, IOtherInterface { }

    [TestClass]
    public class AddSingletonTests
    {
        [TestMethod]
        public void WithConcreteServiceTypes()
        {
            var sc = new ServiceCollection()
                .AddEzPzDi();

            var sp = sc.BuildServiceProvider();
            
            Assert.IsNotNull(sp.GetService<ExampleConcreteSingleton>());
        }

        [TestMethod]
        public void WithUnspecifiedInterfaceServiceTypes()
        {
            var implementationType = typeof(ExampleUnspecifiedSingleton);

            var sc = new ServiceCollection()
                .AddEzPzDi();

            var actual = sc.Where(s => s.ImplementationType == implementationType
                && s.Lifetime == ServiceLifetime.Singleton);
            Assert.AreEqual(2, actual.Count());

            var sp = sc.BuildServiceProvider();

            var found = sp.GetServices<IUnspecifiedInterface>().Where(c => c.GetType() == implementationType);
            Assert.AreEqual(nameof(ExampleUnspecifiedSingleton), found.Single().GetType().Name);

            var otherFound = sp.GetServices<IOtherInterface>().Where(c => c.GetType() == implementationType);
            Assert.AreEqual(nameof(ExampleUnspecifiedSingleton), otherFound.Single().GetType().Name);
        }

        [TestMethod]
        public void WithSpecifiedInterfaceServiceTypes()
        {
            var implementationType = typeof(ExampleSpecifiedSingleton);
            var name = nameof(ExampleSpecifiedSingleton);


            var sc = new ServiceCollection()
                .AddEzPzDi();

            var actual = sc.Where(s => s.ImplementationType == implementationType
                && s.Lifetime == ServiceLifetime.Singleton);
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
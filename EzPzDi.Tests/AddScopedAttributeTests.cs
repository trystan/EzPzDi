using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EzPzDi.Tests
{
    [AddScoped]
    public class ExampleConcreteScoped { }

    [AddScoped]
    public class ExampleUnspecifiedScoped : IUnspecifiedInterface, IOtherInterface { }

    [AddScoped(ServiceTypes = new [] { typeof(IExplicitInterface) })]
    public class ExampleSpecifiedScoped : IExplicitInterface, IOtherInterface { }

    public class ExampleScopedFactoryMethod : IAnotherInterface { }

    [AddScoped(StaticFactoryMethod = nameof(Test))]
    public class ExampleScopedFactoryMethodFactory
    {
        public static IAnotherInterface Test(IServiceProvider serviceProvider) { return new ExampleScopedFactoryMethod(); }
    }

    [TestClass]
    public class AddScopedAttributeTests
    {
        [TestMethod]
        public void ShouldRegisterConcreteServiceTypes()
        {
            var implementationType = typeof(ExampleConcreteScoped);

            var sc = new ServiceCollection()
                .AddEzPzDi();

            var sp = sc.BuildServiceProvider();

            var found = sp.GetService<ExampleConcreteScoped>();

            Assert.AreEqual(nameof(ExampleConcreteScoped), found.GetType().Name);
        }

        [TestMethod]
        public void ShouldRegisterUnspecifiedInterfaceServiceTypes()
        {
            var implementationType = typeof(ExampleUnspecifiedScoped);

            var sc = new ServiceCollection()
                .AddEzPzDi();

            var actual = sc.Where(s => s.ImplementationType == implementationType
                && s.Lifetime == ServiceLifetime.Scoped);
            Assert.AreEqual(2, actual.Count());

            var sp = sc.BuildServiceProvider();

            var found = sp.GetServices<IUnspecifiedInterface>().Single(c => c.GetType() == implementationType);
            Assert.AreEqual(nameof(ExampleUnspecifiedScoped), found.GetType().Name);

            var otherFound = sp.GetServices<IOtherInterface>().Single(c => c.GetType() == implementationType);
            Assert.AreEqual(nameof(ExampleUnspecifiedScoped), otherFound.GetType().Name);
        }

        [TestMethod]
        public void ShouldRegisterSpecifiedInterfaceServiceTypes()
        {
            var implementationType = typeof(ExampleSpecifiedScoped);
            var name = nameof(ExampleSpecifiedScoped);

            var sc = new ServiceCollection()
                .AddEzPzDi();

            var actual = sc.Where(s => s.ImplementationType == implementationType
                && s.Lifetime == ServiceLifetime.Scoped);
            Assert.AreEqual(1, actual.Count());

            var sp = sc.BuildServiceProvider();

            var found = sp.GetServices<IExplicitInterface>().Single(c => c.GetType() == implementationType);
            Assert.AreEqual(name, found.GetType().Name);

            // IOtherInterface wasn't specifed in the attribute but
            // another was, so it shouldn't show up.
            var otherFound = sp.GetServices<IOtherInterface>().Where(c => c.GetType() == implementationType);
            Assert.AreEqual(0, otherFound.Count());
        }

        [TestMethod]
        public void ShouldRegisterFactory()
        {
            var implementationType = typeof(ExampleScopedFactoryMethod);

            var sc = new ServiceCollection()
                .AddEzPzDi();

            var sp = sc.BuildServiceProvider();

            var found = sp.GetServices<IAnotherInterface>().Single(c => c.GetType() == implementationType);
            Assert.AreEqual(nameof(ExampleScopedFactoryMethod), found.GetType().Name);
        }
    }
}
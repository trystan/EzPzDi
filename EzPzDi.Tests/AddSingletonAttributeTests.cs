using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EzPzDi.Tests
{
    [AddSingleton]
    public class ExampleConcreteSingleton { }

    [AddSingleton]
    public class ExampleUnspecifiedSingleton : IUnspecifiedInterface, IOtherInterface { }

    [AddSingleton(ServiceTypes = new [] { typeof(IExplicitInterface) })]
    public class ExampleSpecifiedSingleton : IExplicitInterface, IOtherInterface { }

    public class ExampleSingletonFactoryMethod : IAnotherInterface { }

    [AddSingleton(StaticFactoryMethod = nameof(Test))]
    public class ExampleSingletonFactoryMethodFactory
    {
        public static IAnotherInterface Test(IServiceProvider serviceProvider) { return new ExampleSingletonFactoryMethod(); }
    }

    [TestClass]
    public class AddSingletonAttributeTests
    {
        [TestMethod]
        public void ShouldRegisterConcreteServiceTypes()
        {
            var sc = new ServiceCollection()
                .AddEzPzDi();

            var sp = sc.BuildServiceProvider();
            
            Assert.IsNotNull(sp.GetService<ExampleConcreteSingleton>());
        }

        [TestMethod]
        public void ShouldRegisterUnspecifiedInterfaceServiceTypes()
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
        public void ShouldRegisterSpecifiedInterfaceServiceTypes()
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
            // another was, so it shouldn't show up.
            var otherFound = sp.GetServices<IOtherInterface>().Where(c => c.GetType() == implementationType);
            Assert.AreEqual(0, otherFound.Count());
        }

        [TestMethod]
        public void ShouldRegisterFactory()
        {
            var implementationType = typeof(ExampleSingletonFactoryMethod);

            var sc = new ServiceCollection()
                .AddEzPzDi();

            var sp = sc.BuildServiceProvider();

            var found = sp.GetServices<IAnotherInterface>().Single(c => c.GetType() == implementationType);
            Assert.AreEqual(nameof(ExampleSingletonFactoryMethod), found.GetType().Name);
        }
    }
}
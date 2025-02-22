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

    public class ExampleTransientFactoryMethod : IAnotherInterface { }

    [AddTransient(StaticFactoryMethod = nameof(Test))]
    public class ExampleTransientFactoryMethodFactory
    {
        public static IAnotherInterface Test(IServiceProvider serviceProvider) { return new ExampleTransientFactoryMethod(); }
    }

    [TestClass]
    public class AddTransientAttributeTests
    {
        [TestMethod]
        public void ShouldRegisterConcreteServiceTypes()
        {
            var sc = new ServiceCollection()
                .AddEzPzDi();

            var sp = sc.BuildServiceProvider();
            
            Assert.IsNotNull(sp.GetService<ExampleConcreteTransient>());
        }

        [TestMethod]
        public void ShouldRegisterUnspecifiedInterfaceServiceTypes()
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
        public void ShouldRegisterSpecifiedInterfaceServiceTypes()
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
            // another was, so it shouldn't show up.
            var otherFound = sp.GetServices<IOtherInterface>().Where(c => c.GetType() == implementationType);
            Assert.AreEqual(0, otherFound.Count());
        }

        [TestMethod]
        public void ShouldRegisterFactory()
        {
            var implementationType = typeof(ExampleTransientFactoryMethod);

            var sc = new ServiceCollection()
                .AddEzPzDi();

            var sp = sc.BuildServiceProvider();

            var found = sp.GetServices<IAnotherInterface>().Single(c => c.GetType() == implementationType);
            Assert.AreEqual(nameof(ExampleTransientFactoryMethod), found.GetType().Name);
        }
    }
}
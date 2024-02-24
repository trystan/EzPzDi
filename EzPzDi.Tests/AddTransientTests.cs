using Microsoft.Extensions.DependencyInjection;

namespace EzPzDi.Tests
{
    public interface IUnspecifiedInterface { }

    public interface ISpecifiedInterface { }

    public interface IExplicitInterface { }

    public interface IOtherInterface { }

    [AddTransient]
    public class ExampleConcreteTransient { }

    [AddTransient]
    public class ExampleUnspecifiedTransient : IUnspecifiedInterface, IOtherInterface { }

    [AddTransient(ServiceTypes = new [] { typeof(IExplicitInterface) })]
    public class ExampleExplicitTransient : IExplicitInterface, IOtherInterface { }

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
        public void WithImpliedInterfaceServiceTypes()
        {
            var sc = new ServiceCollection()
                .AddEzPzDi();

            var sp = sc.BuildServiceProvider();

            Assert.IsNotNull(sp.GetService<IUnspecifiedInterface>());

            // IOtherInterface wasn't specifed in the attribute but
            // no other's were, so it should show up here.
            Assert.AreEqual(1, sp.GetServices<IOtherInterface>().Count());
            Assert.AreEqual("ExampleUnspecifiedTransient", sp.GetService<IOtherInterface>().GetType().Name);
        }

        [TestMethod]
        public void WithExplicitInterfaceServiceTypes()
        {
            var sc = new ServiceCollection()
                .AddEzPzDi();

            var sp = sc.BuildServiceProvider();

            Assert.AreEqual(1, sp.GetServices<IExplicitInterface>().Count());

            // IOtherInterface wasn't specifed in the attribute but
            // another was, so it shoudln't show up.
            Assert.AreEqual(1, sp.GetServices<IOtherInterface>().Count());
            Assert.AreEqual("ExampleUnspecifiedTransient", sp.GetService<IOtherInterface>().GetType().Name);
        }
    }
}
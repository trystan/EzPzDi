using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EzPzDi.Tests
{
    [TestClass]
    public class AddEzPzDiTests
    {
        [TestMethod]
        public void DoesNotAddTheSameTypeTwice()
        {
            var assemblyA = typeof(AssemblyA.Implementation).Assembly;

            var sp = new ServiceCollection()
                .AddEzPzDi(c =>
                {
                    c.FromAssemblies = new[] { assemblyA, assemblyA };
                })
                .BuildServiceProvider();

            var found = sp.GetServices<IUnspecifiedInterface>().ToArray();

            Assert.AreEqual(1, found.Length);
            Assert.AreEqual("EzPzDi.Tests.AssemblyA.Implementation", found[0].GetType().FullName);
        }

        [TestMethod]
        public void CanBeConfiguredToLoadFromSpecificAssembly()
        {
            var assemblyA = typeof(AssemblyA.Implementation).Assembly;
            
            var sp = new ServiceCollection()
                .AddEzPzDi(c =>
                {
                    c.FromAssemblies = new[] { assemblyA };
                })
                .BuildServiceProvider();

            var found = sp.GetServices<IUnspecifiedInterface>().ToArray();

            Assert.AreEqual(1, found.Length);
            Assert.AreEqual("EzPzDi.Tests.AssemblyA.Implementation", found[0].GetType().FullName);
        }

        [TestMethod]
        public void CanBeConfiguredToLoadFromSpecificAssemblies()
        {
            var assemblyA = typeof(AssemblyA.Implementation).Assembly;
            var assemblyB = typeof(AssemblyB.Implementation).Assembly;

            var sp = new ServiceCollection()
                .AddEzPzDi(c =>
                {
                    c.FromAssemblies = new[] { assemblyA, assemblyB };
                })
                .BuildServiceProvider();

            var found = sp.GetServices<IUnspecifiedInterface>().ToArray();

            Assert.AreEqual(2, found.Length);
            Assert.AreEqual("EzPzDi.Tests.AssemblyA.Implementation", found[0].GetType().FullName);
            Assert.AreEqual("EzPzDi.Tests.AssemblyB.Implementation", found[1].GetType().FullName);
        }

        [TestMethod]
        public void CanBeConfiguredToLoadFromEmptyListOfAssemblies()
        {
            var assemblyA = typeof(AssemblyA.Implementation).Assembly;
            var assemblyB = typeof(AssemblyB.Implementation).Assembly;

            var sp = new ServiceCollection()
                .AddEzPzDi(c =>
                {
                    c.FromAssemblies = Array.Empty<System.Reflection.Assembly>();
                })
                .BuildServiceProvider();

            var found = sp.GetServices<IUnspecifiedInterface>().ToArray();

            Assert.AreEqual(0, found.Length);
        }

        [TestMethod]
        public void ShouldLoadFromAllAssembliesByDefault()
        {
            // ensure all test assemblies have been loaded
            var assemblies = new[] { typeof(AssemblyA.Implementation).Assembly, typeof(AssemblyB.Implementation).Assembly };

            var sp = new ServiceCollection()
                .AddEzPzDi()
                .BuildServiceProvider();

            var found = sp.GetServices<IUnspecifiedInterface>().ToArray();
            var fullNames = found.Select(s => s.GetType().FullName).Order().ToArray();

            Assert.AreEqual(5, found.Length);
            Assert.AreEqual("EzPzDi.Tests.AssemblyA.Implementation", fullNames[0]);
            Assert.AreEqual("EzPzDi.Tests.AssemblyB.Implementation", fullNames[1]);
            Assert.AreEqual("EzPzDi.Tests.ExampleUnspecifiedScoped", fullNames[2]);
            Assert.AreEqual("EzPzDi.Tests.ExampleUnspecifiedSingleton", fullNames[3]);
            Assert.AreEqual("EzPzDi.Tests.ExampleUnspecifiedTransient", fullNames[4]);
        }
    }
}

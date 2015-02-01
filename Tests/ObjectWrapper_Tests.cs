using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shiva;
using Shiva.Sample.Models;
using Shiva.Sample.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class ObjectWrapper_Tests
    {
        [TestMethod]
        public void Ctor_Tests()
        {
            try
            {
                var ow = new ObjectWrapper<Person, PersonDialogViewModel>(null);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual(e.ParamName, "sourceGetterFunction");
            }

            Func<Person> f = new Func<Person>(() => new Person());
            var ow2 = new ObjectWrapper<Person, PersonDialogViewModel>(f);
            Assert.AreEqual(ow2.SourceGetterFunction, f);
        }

        [TestMethod]
        public void Reset_Tests()
        {
            Person p = null;
            ObjectWrapper<Person, PersonDialogViewModel> ow;

            p = new Person { FirstName = "A" };
            ow = new ObjectWrapper<Person, PersonDialogViewModel>(() => p);
            Assert.AreEqual((ow.Value as ViewModelProxy<Person>).Model, p);

            p = new Person { FirstName = "B" };
            ow .Reset();
            Assert.AreEqual((ow.Value as ViewModelProxy<Person>).Model, p);
        }
    }
}

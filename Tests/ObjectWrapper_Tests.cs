using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shiva;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models = Shiva.Sample.Models;
using ViewModels = Shiva.Sample.ViewModels;

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
                var ow = new ObjectWrapperByModel<Models.Person, ViewModels.PersonDialog>(null);
                Assert.Fail();
            }
            catch (ArgumentNullException e)
            {
                Assert.AreEqual(e.ParamName, "sourceGetterFunction");
            }

            Func<Models.Person> f = new Func<Models.Person>(() => new Models.Person());
            var ow2 = new ObjectWrapperByModel<Models.Person, ViewModels.PersonDialog>(f);
            Assert.AreEqual(ow2.SourceGetterFunction, f);
        }

        [TestMethod]
        public void Reset_Tests()
        {
            Models.Person p = null;
            ObjectWrapperByModel<Models.Person, ViewModels.PersonDialog> ow;

            p = new Models.Person { FirstName = "A" };
            ow = new ObjectWrapperByModel<Models.Person, ViewModels.PersonDialog>(() => p);
            Assert.AreEqual((ow.Value as ViewModelProxy<Models.Person>).Model, p);

            p = new Models.Person { FirstName = "B" };
            ow.Reset();
            Assert.AreEqual((ow.Value as ViewModelProxy<Models.Person>).Model, p);
        }
    }
}

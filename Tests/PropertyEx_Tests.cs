using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class PropertyEx_Tests
    {
        [TestMethod]
        public void Name_WhenSelectorExpressionArgumentIsNull_ThrowArgumentNullException()
        {
            // arrange
            // act
            try
            {
                Expression<Func<string>> selectorExpression = null;
                Shiva.PropertyEx.Name(selectorExpression);
            }
            catch (ArgumentNullException e)
            {
                // assert
                Assert.AreEqual(e.ParamName, "selectorExpression");
            }
        }

        [TestMethod]
        public void Name_WhenSelectorExpressionArgumentIsNotMemberExpression_ThrowArgumentException()
        {
            // arrange
            // act
            try
            {
                int x = 0;
                Shiva.PropertyEx.Name(() => x);
            }
            catch (ArgumentException e)
            {
                // assert
                Assert.AreEqual(e.ParamName, "selectorExpression");
            }
        }

        public int MyProperty { get; set; }

        [TestMethod]
        public void Name_WhenArgumentsAreOk_ShouldReturnCorrectly()
        {
            // arrange
            // act
            // assert
            Assert.AreEqual(Shiva.PropertyEx.Name(() => MyProperty), "MyProperty");
        }
    }
}

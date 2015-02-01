using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace Tests
{
    [TestClass]
    public class ConfigurationItem_Tests
    {
        [TestMethod]
        public void Enforce_WhenRuleArgumentIsNull_ThrowArgumentNullException()
        {
            // arrange
            var configItem = new Shiva.ConfigurationItem<string>();

            // act
            try
            {
                configItem.Enforce(null, "something");
            }
            catch (ArgumentNullException e)
            {

                // assert
                Assert.AreEqual(e.ParamName, "rule");
            }
        }

        [TestMethod]
        public void Enforce_WhenMessageArgumentIsNull_ThrowArgumentNullException()
        {
            // arrange
            var configItem = new Shiva.ConfigurationItem<string>();

            // act
            try
            {
                configItem.Enforce(s => true, null);
            }
            catch (ArgumentNullException e)
            {
                // assert
                Assert.AreEqual(e.ParamName, "message");
            }
        }

        [TestMethod]
        public void Enforce_WhenArgumentsAreOk_EnforceItemShouldAddedCorrectly()
        {
            // arrange
            var configItem = new Shiva.ConfigurationItem<string>();

            // act
            configItem.Enforce(s => true, "something");

            // assert
            Assert.AreEqual(configItem.Rules.Count, 1);
        }

        [TestMethod]
        public void DependsOn_WhenSelectorExpressionArgumentIsNull_ThrowArgumentNullException()
        {
            // arrange
            var configItem = new Shiva.ConfigurationItem<string>();

            // act
            try
            {
                Expression<Func<int>> exp = null;
                configItem.DependsOn(exp);
            }
            catch (ArgumentNullException e)
            {
                // assert
                Assert.AreEqual(e.ParamName, "selectorExpression");
            }
        }

        [TestMethod]
        public void DependsOn_WhenArgumentsAreOk_DependsOnItemShouldAddedCorrectly()
        {
            // arrange
            var configItem = new Shiva.ConfigurationItem<string>();

            // act
            configItem.DependsOn(() => configItem);

            // assert
            Assert.AreEqual(configItem.Dependencies.Count, 1);
        }
    }
}

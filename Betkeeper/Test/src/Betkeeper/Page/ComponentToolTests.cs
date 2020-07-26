using System;
using System.Collections.Generic;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using NUnit.Framework;

namespace Betkeeper.Test.Page
{

    [TestFixture]
    public class ComponentToolTests
    {
        [Test]
        public void DeleteComponent_DeletesComponenFromSingleLevelList()
        {
            var components = new List<Component>
            {
                new Field("key", "label", FieldType.DateTime),
                new PageActionButton("action", new List<string>(), "text"),
                new Container(new List<Component>(), "compKeyToDelete"),
                new PageActionButton("action", new List<string>(), "text"),
            };

            ComponentTools.DeleteComponent(components, "compKeyToDelete");
            Assert.AreEqual(3, components.Count);
        }

        [Test]
        public void DeleteComponent_DeletesComponenFromMultiLevelList()
        {
            var components = new List<Component>
            {
                new Field("key", "label", FieldType.DateTime),
                new PageActionButton("action", new List<string>(), "text"),
                new Container(
                    new List<Component>
                    {
                        new Container(
                            new List<Component>
                            {
                                new Field("deleteThis", "label", FieldType.DateTime),
                                // Not deleted because second match
                                new Field("deleteThis", "label", FieldType.DateTime),
                            })
                    }, 
                    "key"),
                new PageActionButton("action", new List<string>(), "text"),
            };

            ComponentTools.DeleteComponent(components, "deleteThis");
            // Nothing deleted on highest level
            Assert.AreEqual(4, components.Count);

            var targetContainer = ((components[2] as Container).Children[0] as Container);
            Assert.AreEqual(1, targetContainer.Children.Count);
        }
    }
}

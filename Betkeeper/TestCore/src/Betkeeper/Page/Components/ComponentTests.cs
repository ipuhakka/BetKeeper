using System;
using System.Collections.Generic;
using System.Linq;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace Betkeeper.Test.Page.Components
{
    [TestFixture]
    public class ComponentTests
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

            Component.DeleteComponent(components, "compKeyToDelete");
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

            Component.DeleteComponent(components, "deleteThis");
            // Nothing deleted on highest level
            Assert.AreEqual(4, components.Count);

            var targetContainer = ((components[2] as Container).Children[0] as Container);
            Assert.AreEqual(1, targetContainer.Children.Count);
        }

        [Test]
        public void ParseComponent_ParsesPageActionButtonCorrectly()
        {
            var button = new PageActionButton(
                "action",
                new List<string> { "actionKey1", "actionKey2" },
                "text",
                "style",
                true,
                "navigateTo",
                new List<string> { "include1", "include2" });

            var result = Component
                .ParseComponent(SerializeAsCamelCase(button)) as PageActionButton;

            Assert.AreEqual(button.Action, result.Action);
            Assert.AreEqual(button.ActionDataKeys.Count, result.ActionDataKeys.Count);
            Assert.AreEqual(button.Text, result.Text);
            Assert.AreEqual(button.ButtonStyle, result.ButtonStyle);
            Assert.IsTrue(result.RequireConfirm);
            Assert.AreEqual(button.NavigateTo, result.NavigateTo);
        }

        [Test]
        public void ParseComponent_ParsesModalActionButtonCorrectly()
        {
            var pageActionButton = new PageActionButton(
                "action",
                new List<string> { "actionKey1", "actionKey2" },
                "text",
                "style",
                true,
                "navigateTo",
                new List<string> { "include1", "include2" });

            var navigationButton = new NavigationButton("navigateTo", "text", "style");

            var button = new ModalActionButton(
                "action",
                new List<Component>
                {
                    pageActionButton,
                    navigationButton
                },
                "text",
                "style",
                true,
                "navigateTo");

            var result = Component
                .ParseComponent(SerializeAsCamelCase(button)) as ModalActionButton;

            Assert.AreEqual(button.Action, result.Action);
            Assert.AreEqual(button.Text, result.Text);
            Assert.AreEqual(button.ButtonStyle, result.ButtonStyle);
            Assert.IsTrue(result.RequireConfirm);
            Assert.AreEqual(button.NavigateTo, result.NavigateTo);

            // Assert pageactionbutton
            var resultAsPageAction = result.Components[0] as PageActionButton;

            Assert.AreEqual(button.Action, resultAsPageAction.Action);
            Assert.AreEqual(pageActionButton.ActionDataKeys.Count, resultAsPageAction.ActionDataKeys.Count);
            Assert.AreEqual(pageActionButton.Text, resultAsPageAction.Text);
            Assert.AreEqual(pageActionButton.ButtonStyle, resultAsPageAction.ButtonStyle);
            Assert.IsTrue(resultAsPageAction.RequireConfirm);
            Assert.AreEqual(pageActionButton.NavigateTo, resultAsPageAction.NavigateTo);

            // Assert navigation button
            var resultNavigationButton = result.Components[1] as NavigationButton;
            Assert.AreEqual(navigationButton.Text, resultNavigationButton.Text);
            Assert.AreEqual(navigationButton.ButtonStyle, resultNavigationButton.ButtonStyle);
            Assert.IsFalse(resultNavigationButton.RequireConfirm);
            Assert.AreEqual(navigationButton.NavigateTo, resultNavigationButton.NavigateTo);

        }

        [Test]
        public void ParseComponent_ParsesNavigationButtonCorrectly()
        {
            var button = new NavigationButton("navigateTo", "text", "style");

            var result = Component
                .ParseComponent(SerializeAsCamelCase(button)) as NavigationButton;

            Assert.AreEqual(button.Text, result.Text);
            Assert.AreEqual(button.ButtonStyle, result.ButtonStyle);
            Assert.IsFalse(result.RequireConfirm);
            Assert.AreEqual(button.NavigateTo, result.NavigateTo);
        }

        [Test]
        public void ParseComponent_ParsesFieldsCorrectly()
        {
            var fields = new List<Field>
            {
                new Field("key1", "label1", true, FieldType.DateTime, "dataKey1"),
                new Field("key2", "label2", true, FieldType.Double, "dataKey2"),
                new Field("key3", "label3", true, FieldType.Integer, "dataKey3"),
                new Field("key4", "label4", true, FieldType.TextArea, "dataKey4"),
                new Field("key5", "label5", false, FieldType.TextBox, "dataKey5")
            };

            var results = Component
                .ParseComponents(SerializeAsCamelCase(fields))
                .Select(component => component as Field)
                .ToList();

            for (int i = 0; i < fields.Count; i++)
            {
                Assert.AreEqual(fields[i].ComponentKey, results[i].ComponentKey);
                Assert.AreEqual(fields[i].Label, results[i].Label);
                Assert.AreEqual(fields[i].ReadOnly, results[i].ReadOnly);
                Assert.AreEqual(fields[i].FieldType, results[i].FieldType);
                Assert.AreEqual(fields[i].DataKey, results[i].DataKey);
            }
        }

        [Test]
        public void ParseComponent_ParsesDropdownCorrectly()
        {
            var dropdown = new Dropdown("key1", "label1", new List<Option>
                {
                    new Option("option1", "value1"),
                    new Option("option2", "value2")
                });

            var result = Component.ParseComponent(SerializeAsCamelCase(dropdown)) as Dropdown;

            Assert.AreEqual(dropdown.ComponentKey, result.ComponentKey);
            Assert.AreEqual(dropdown.Label, result.Label);

            Assert.AreEqual(dropdown.Options[0].Key, result.Options[0].Key);
            Assert.AreEqual(dropdown.Options[0].Value, result.Options[0].Value);
            Assert.AreEqual(dropdown.Options[1].Key, result.Options[1].Key);
            Assert.AreEqual(dropdown.Options[1].Value, result.Options[1].Value);
        }

        [Test]
        public void ParseComponent_ParsesDateTimeInputCorretly()
        {
            var now = DateTime.Now;
            var dateTimeInput = new DateTimeInput("key", "label", now);

            var result = Component.ParseComponent(SerializeAsCamelCase(dateTimeInput)) as DateTimeInput;

            Assert.AreEqual(dateTimeInput.ComponentKey, result.ComponentKey);
            Assert.AreEqual(dateTimeInput.Label, result.Label);
            Assert.AreEqual(dateTimeInput.FieldType, result.FieldType);
            Assert.AreEqual(dateTimeInput.MinimumDateTime, result.MinimumDateTime);
        }

        [Test]
        public void ParseComponent_ParsesTableCorrectly()
        {
            var tableToTest = new Table(
                "dataKey",
                new List<ItemField>
                {
                    new ItemField("key1", DataType.DateTime),
                    new ItemField("key2", DataType.Double),
                    new ItemField("key3", DataType.Integer),
                    new ItemField("key4", DataType.String)
                },
                "navigation key");

            var result = Component.ParseComponent(SerializeAsCamelCase(tableToTest)) as Table;

            Assert.AreEqual(tableToTest.DataKey, result.DataKey);
            Assert.AreEqual(tableToTest.NavigationKey, result.NavigationKey);

            for (var i = 0; i < tableToTest.Columns.Count; i++)
            {
                Assert.AreEqual(tableToTest.Columns[i].FieldKey, result.Columns[i].FieldKey);
                Assert.AreEqual(tableToTest.Columns[i].FieldType, result.Columns[i].FieldType);
            }
        }

        [Test]
        public void ParseComponent_ParsesTabCorrectly()
        {
            var tab = new Tab("tabKey", "title", new List<Component>
            {
                new Table("tableKey", new List<ItemField>{ new ItemField("dataFieldKey", DataType.DateTime)} ),
                new ModalActionButton(
                    "modalAction",
                    new List<Component>
                    {
                        new NavigationButton("to", "text", "somestyle")
                    },
                    "buttonText")
            });

            var result = Component.ParseComponent(SerializeAsCamelCase(tab)) as Tab;

            Assert.AreEqual(tab.ComponentKey, result.ComponentKey);
            Assert.AreEqual(tab.Title, result.Title);

            // Test table
            var originalTable = tab.TabContent[0] as Table;
            var table = result.TabContent[0] as Table;

            Assert.AreEqual(originalTable.DataKey, table.DataKey);
            Assert.AreEqual(originalTable.NavigationKey, table.NavigationKey);

            for (var i = 0; i < originalTable.Columns.Count; i++)
            {
                Assert.AreEqual(originalTable.Columns[i].FieldKey, table.Columns[i].FieldKey);
                Assert.AreEqual(originalTable.Columns[i].FieldType, table.Columns[i].FieldType);
            }

            // Test modal action button
            var originalModalActionButton = tab.TabContent[1] as ModalActionButton;
            var modalActionButton = result.TabContent[1] as ModalActionButton;

            Assert.AreEqual(originalModalActionButton.Action, modalActionButton.Action);
            Assert.AreEqual(originalModalActionButton.Text, modalActionButton.Text);

            var originalNavigationButton = originalModalActionButton.Components[0] as NavigationButton;
            var navigationButton = modalActionButton.Components[0] as NavigationButton;

            Assert.AreEqual(originalNavigationButton.NavigateTo, navigationButton.NavigateTo);
            Assert.AreEqual(originalNavigationButton.Text, navigationButton.Text);
            Assert.AreEqual(originalNavigationButton.ButtonStyle, navigationButton.ButtonStyle);
        }

        [Test]
        public void ParseComponent_ParsesContainerCorrectly()
        {
            var container = new Container(
                new List<Component>
                {
                    new Field("key1", "label1", FieldType.Double)
                });

            var result = Component.ParseComponent(SerializeAsCamelCase(container)) as Container;

            var originalChildAsField = container.Children[0] as Field;
            var childAsField = result.Children[0] as Field;

            Assert.AreEqual(originalChildAsField.ComponentKey, childAsField.ComponentKey);
            Assert.AreEqual(originalChildAsField.Label, childAsField.Label);
            Assert.AreEqual(originalChildAsField.FieldType, childAsField.FieldType);
        }

        /// <summary>
        /// Serializes object in camelCase.
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        private static string SerializeAsCamelCase(object component)
        {
            return JsonConvert.SerializeObject(
                component,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }
    }
}

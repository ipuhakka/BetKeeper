using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using NUnit.Framework;

namespace Betkeeper.Test.Page
{
    [TestFixture]
    public class ComponentParserTests
    {

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
                new List<string> { "include1", "include2"});

            var result = ComponentParser
                .ParseComponent(JsonConvert.SerializeObject(button)) as PageActionButton;

            Assert.AreEqual(button.Action, result.Action);
            Assert.AreEqual(button.ActionDataKeys.Count, result.ActionDataKeys.Count);
            Assert.AreEqual(button.Text, result.Text);
            Assert.AreEqual(button.Style, result.Style);
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

            var result = ComponentParser
                .ParseComponent(JsonConvert.SerializeObject(button)) as ModalActionButton;

            Assert.AreEqual(button.Action, result.Action);
            Assert.AreEqual(button.Text, result.Text);
            Assert.AreEqual(button.Style, result.Style);
            Assert.IsTrue(result.RequireConfirm);
            Assert.AreEqual(button.NavigateTo, result.NavigateTo);

            // Assert pageactionbutton
            var resultAsPageAction = result.Components[0] as PageActionButton;

            Assert.AreEqual(button.Action, resultAsPageAction.Action);
            Assert.AreEqual(pageActionButton.ActionDataKeys.Count, resultAsPageAction.ActionDataKeys.Count);
            Assert.AreEqual(pageActionButton.Text, resultAsPageAction.Text);
            Assert.AreEqual(pageActionButton.Style, resultAsPageAction.Style);
            Assert.IsTrue(resultAsPageAction.RequireConfirm);
            Assert.AreEqual(pageActionButton.NavigateTo, resultAsPageAction.NavigateTo);

            // Assert navigation button
            var resultNavigationButton = result.Components[1] as NavigationButton;
            Assert.AreEqual(navigationButton.Text, resultNavigationButton.Text);
            Assert.AreEqual(navigationButton.Style, resultNavigationButton.Style);
            Assert.IsFalse(resultNavigationButton.RequireConfirm);
            Assert.AreEqual(navigationButton.NavigateTo, resultNavigationButton.NavigateTo);

        }

        [Test]
        public void ParseComponent_ParsesNavigationButtonCorrectly()
        {
            var button = new NavigationButton("navigateTo", "text", "style");

            var result = ComponentParser
                .ParseComponent(JsonConvert.SerializeObject(button)) as NavigationButton;

            Assert.AreEqual(button.Text, result.Text);
            Assert.AreEqual(button.Style, result.Style);
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

            var results = ComponentParser
                .ParseComponents(JsonConvert.SerializeObject(fields))
                .Select(component => component as Field)
                .ToList();

            for (int i = 0; i < fields.Count; i++)
            {
                Assert.AreEqual(fields[i].Key, results[i].Key);
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

            var result = ComponentParser.ParseComponent(JsonConvert.SerializeObject(dropdown)) as Dropdown;

            Assert.AreEqual(dropdown.Key, result.Key);
            Assert.AreEqual(dropdown.Label, result.Label);

            Assert.AreEqual(dropdown.Options[0].Key, result.Options[0].Key);
            Assert.AreEqual(dropdown.Options[0].Value, result.Options[0].Value);
            Assert.AreEqual(dropdown.Options[1].Key, result.Options[1].Key);
            Assert.AreEqual(dropdown.Options[1].Value, result.Options[1].Value);
        }
    }
}

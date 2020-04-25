using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Betkeeper.Classes;
using Betkeeper.Enums;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using Newtonsoft.Json.Linq;

namespace Betkeeper.Pages
{

    /// <summary>
    /// Code related to CompetitionPage's Manage Bets-tab.
    /// </summary>
    public partial class CompetitionPage
    {
        private Tab GetManageBetsTab()
        {
            // TODO: Hae jo olemassa olevat targetit ja 
            // luo niille databoundeilla fieldiellä varustetu containerit
            return new Tab(
                "manageBets",
                "Manage bets",
                new List<Component>
                {
                    new Container(
                        new List<Component>(),
                        componentKey: "betTargets"),
                    new PageActionButton(
                        "AddBetContainer",
                        new List<string>{ "betTargets" },
                        "Add bet",
                        componentsToInclude: new List<string>{ "betTargets" })
                });
        }

        /// <summary>
        /// Adds a new target to bet container.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private HttpResponseMessage AddBetContainer(PageAction action)
        {
            // TODO: Initialvalueta ei voida asettaa bet-type-dropdownille koska
            // Edellisen arvoa ei tiedetä. Näin ollen uusi container luodaan aina result-tyyppisenä.
            // Kun sisäkkäiset data-objektit mahdollistetaan tämä tulisi muuttaa katsomaan edellinen valinta datasta,
            // ei edellisestä komponentista.
            var components = JObject.Parse(action.Parameters["components"].ToString());

            var betTargetsAsJObject = components["betTargets"];

            var betTargetContainer = ComponentParser
                .ParseComponent(betTargetsAsJObject.ToString()) as Container;

            if (betTargetContainer.Children.Count == 0)
            {
                var defaultBetTarget = CreateTargetContainer(0, TargetType.OpenQuestion);

                betTargetContainer.Children.Add(new Container(
                    new List<Component>
                    {
                        // TODO: DataKeyt
                        new PageActionButton(
                            "cancelBetTargetsUpdate",
                            new List<string>(),
                            "Cancel bet targets update",
                            style: "outline-danger",
                            requireConfirm: true),
                        new PageActionButton(
                            "saveBetTargets",
                            new List<string>(),
                            "Save bet targets",
                            requireConfirm: true)
                    }));

                betTargetContainer.Children.Add(defaultBetTarget);
            }
            else
            {
                // Remove Save-cancel container from count
                var newIndex = betTargetContainer.Children.Count - 1;

                var previousBetTarget = betTargetContainer.Children.Last() as Container;

                var newBetTarget = Component.CloneComponent<Container>(previousBetTarget);
                newBetTarget.ComponentKey = $"bet-target-{newIndex}";

                newBetTarget.Children.ForEach(child =>
                {
                    child.ComponentKey = child
                    .ComponentKey
                    .Replace(
                        (newIndex - 1).ToString(),
                        newIndex.ToString()
                    );
                });

                var betTypeDropdown = newBetTarget.Children.First() as Dropdown;

                betTypeDropdown.ComponentsToUpdate = new List<string> { newBetTarget.ComponentKey };

                betTargetContainer.Children.Add(newBetTarget);
            }

            return Http.CreateResponse(HttpStatusCode.OK, new PageActionResponse(betTargetContainer));
        }

        /// <summary>
        /// Creates a target. Called from HandleDropdownUpdate.
        /// </summary>
        /// <param name="index"></param>
        private Container CreateTargetContainer(int index, TargetType targetType)
        {
            var options = new List<Option>
                {
                    new Option("result", "Result"),
                    new Option("selection", "Selection"),
                    new Option("openQuestion", "Open question")
                };

            var betTypeDropdown = new Dropdown(
                $"bet-type-{index}",
                "Bet type",
                options,
                new List<string> { $"bet-target-{index}" });

            // TODO: Luo oikeat komponentit
            switch (targetType)
            {
                default:
                    throw new NotImplementedException($"{targetType} container not implemented");

                case TargetType.OpenQuestion:
                    return new Container(
                     new List<Component>
                     {
                        betTypeDropdown,
                        new Field($"question-{index}", "Bet", FieldType.TextArea),
                        new Field($"scoring-{index}", "Points for correct answer", FieldType.Double)
                     },
                     $"bet-target-{index}");

                case TargetType.Result:
                    return new Container(
                     new List<Component>
                     {
                        betTypeDropdown,
                        new Field($"question-{index}", "Bet", FieldType.TextBox),
                        new Field($"scoring-{index}", "Points for correct result", FieldType.Double),
                        new Field($"winner-{index}", "Points for correct winner", FieldType.Double)
                     },
                     $"bet-target-{index}");

                case TargetType.Selection:
                    // TODO: Modaali jolla lisätään valinnat
                    return new Container(
                     new List<Component>
                     {
                        betTypeDropdown,
                        new Field($"question-{index}", "Bet", FieldType.TextBox),
                        new Dropdown($"selection-{index}", "Selections", new List<Option>()),
                        new Field($"scoring-{index}", "Points for correct answer", FieldType.Double)
                     },
                     $"bet-target-{index}");
            }
        }
    }
}

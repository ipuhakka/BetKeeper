using Betkeeper.Page;
using Betkeeper.Page.Components;
using Betkeeper.Models;
using Betkeeper.Actions;
using System.Collections.Generic;
using System;
using Betkeeper.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Linq;
using Betkeeper.Extensions;
using Newtonsoft.Json.Linq;
using Betkeeper.Classes;

namespace Betkeeper.Pages.BetsPage
{
    public class BetsPage : PageBase
    {
        public override PageResponse GetPage(string pageKey, int userId)
        {
            var folderOptions = new List<string>
            {
                "Overview"
            };
            folderOptions.AddRange(new FolderAction().GetUsersFolders(userId));

            return new PageResponse(
                new List<Component>
                {
                    GetAddBetButton(),
                    new Dropdown(
                        "FolderSelection",
                        "Folder",
                        folderOptions,
                        componentsToUpdate: new List<string>
                        {
                            "betsListGroup"
                        }),
                    GetBetListGroup(new BetAction().GetBets(userId))
                })
                {
                    PageKey = pageKey,
                    Data = new Dictionary<string, object>()
                };
        }

        public override PageActionResponse HandleAction(PageAction action)
        {
            switch (action.ActionName)
            {
                case "modifyBet":
                    return HandleModifyBet(action);

                case "deleteBet":
                    return HandleDeleteBet(action);

                case "addBet":
                    return HandleAddBet(action);
            }

            throw new NotImplementedException($"Not implemented action {action.ActionName} called");
        }

        public override PageResponse HandleDropdownUpdate(DropdownUpdateParameters parameters)
        {
            var key = parameters.Data.GetString("key");

            if (key == "FolderSelection")
            {
                var folderParameter = parameters.Data.GetString("value");

                var bets = new BetAction().GetBets(
                    parameters.UserId, 
                    folder: folderParameter == "Overview"
                        ? null: folderParameter);

                return new PageResponse(new List<Component> { GetBetListGroup(bets) });
            }

            throw new NotImplementedException($"Not implemented for {key}");
        }

        /// <summary>
        /// Get bet list group with specified bets
        /// </summary>
        /// <param name="bets"></param>
        /// <returns></returns>
        private ListGroup<BetListGroupData> GetBetListGroup(List<Bet> bets)
        {
            return new ExpandableListGroup<BetListGroupData>(
                        bets.Select(bet => new BetListGroupData(bet))
                            .OrderByDescending(bet => bet.BetId)
                            .ToList(),
                        "betId",
                        new List<ItemField>
                        {
                            new ItemField("name", TypeCode.String),
                            new ItemField("playedDate", TypeCode.DateTime),
                            new ItemField("betResult", TypeCode.String)
                        },
                        new List<Field> 
                        {
                            new Field("name", "Name", FieldType.TextBox),
                            new Dropdown("betResult", "Result", new List<string>
                            {
                                "Unresolved",
                                "Won",
                                "Lost"
                            }),
                            new Field("stake", "Bet", FieldType.Double),
                            new Field("odd", "Odd", FieldType.Double)
                        },
                        new List<ItemField>
                        {
                            new ItemField("stake", TypeCode.Double, "Bet"),
                            new ItemField("odd", TypeCode.Double, "Odd")
                        },
                        componentKey: "betsListGroup",
                        itemActions: new List<Button> 
                        {
                            // Item actions specify data keys on client side
                            new PageActionButton(
                                "modifyBet", 
                                new List<string>{ "FolderSelection" }, 
                                "", 
                                displayType: DisplayType.Icon, 
                                componentsToInclude: new List<string>{ "betsListGroup" })
                            {
                                IconName = "far fa-save"
                            },
                            new PageActionButton(
                                "deleteBet",
                                new List<string>{ "FolderSelection" },
                                "",
                                style: "outline-danger",
                                displayType: DisplayType.Icon,
                                componentsToInclude: new List<string>{ "betsListGroup" })
                            {
                                IconName = "far fa-trash-alt"
                            },
                        });
        }

        /// <summary>
        /// Returns bet list group based on action's folder parameter
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private ListGroup<BetListGroupData> GetBetListGroup(PageAction action)
        {
            var folderParameter = action.Parameters.GetString("FolderSelection");
            var folder = string.IsNullOrEmpty(folderParameter) || folderParameter == "Overview"
                ? null
                : folderParameter;

            return GetBetListGroup(new BetAction().GetBets(action.UserId, folder: folder));
        }

        private ModalActionButton GetAddBetButton()
        {
            return new ModalActionButton(
                "addBet",
                new List<Component>
                {
                    new Field("name", "Name", FieldType.TextBox),
                    new Dropdown("betResult", "Result", new List<string>
                    {
                        "Unresolved",
                        "Won",
                        "Lost"
                    }),
                    new Field("stake", "Bet", FieldType.Double),
                    new Field("odd", "Odd", FieldType.Double)
                },
                "Create a new bet");
        }

        /// <summary>
        /// Modifies an existing bet
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private PageActionResponse HandleModifyBet(PageAction action)
        {
            var dataKey = action.Parameters.GetKeyLike("listGroup-");

            var betParameters = (action.Parameters[dataKey] as JObject).ToObject<Dictionary<string, object>>();

            new BetAction().ModifyBet(
                (int)action.Parameters.GetIdentifierFromKeyLike("listGroup-"),
                action.UserId,
                EnumHelper.FromString<BetResult>(betParameters.GetString("betResult")),
                betParameters.GetDouble("stake"),
                betParameters.GetDouble("odd"),
                betParameters.GetString("name"));

            return new PageActionResponse(new List<Component> 
            {
                GetBetListGroup(action)
            });
        }

        /// <summary>
        /// Handles deleting a bet
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private PageActionResponse HandleDeleteBet(PageAction action)
        {
            new BetAction().DeleteBet(
                (int)action.Parameters.GetIdentifierFromKeyLike("listGroup-"),
                action.UserId);

            return new PageActionResponse(new List<Component>
            {
                GetBetListGroup(action)
            });
        }

        private PageActionResponse HandleAddBet(PageAction action)
        {
            var parameters = action.Parameters;

            double? odd = parameters.GetDouble("odd");
            double? stake = parameters.GetDouble("stake");

            if (odd == null || stake == null)
            {
                return new PageActionResponse(ActionResultType.InvalidInput, "Need to input odd and stake");
            }

            new BetAction().CreateBet(
                EnumHelper.FromString<BetResult>(parameters.GetString("betResult")),
                parameters.GetString("name"),
                (double)odd,
                (double)stake,
                DateTime.UtcNow,
                action.UserId);

            return new PageActionResponse(ActionResultType.Created, "Bet created successfully", refresh: true);
        }

        private class BetListGroupData
        {
            public int BetId { get; }

            public string Name { get; }

            [JsonConverter(typeof(StringEnumConverter))]
            public BetResult BetResult { get; }

            public double? Stake { get; }

            public double? Odd { get; }

            public DateTime PlayedDate { get; }

            public BetListGroupData(Bet bet)
            {
                BetId = bet.BetId;
                BetResult = bet.BetResult;
                Stake = bet.Stake;
                Odd = bet.Odd;
                PlayedDate = bet.PlayedDate;
                Name = bet.Name;
            }
        }
    }
}

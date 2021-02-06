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
        public override string PageKey => "bets";

        public override PageResponse GetPage(string pageKey, int userId)
        {
            var usersFolders = new FolderAction().GetUsersFolders(userId);
            var folderOptions = new List<string>
            {
                "Overview"
            };
            folderOptions.AddRange(usersFolders);

            return new PageResponse(
                new List<Component>
                {
                    GetAddBetButton(usersFolders),
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
        /// Handle expanding list group item
        /// </summary>
        /// <param name="expandParameters"></param>
        /// <returns></returns>
        public override ItemContent ExpandListGroupItem(ListGroupItemExpandParameters expandParameters)
        {
            switch (expandParameters.ComponentKey)
            {
                case "betsListGroup":
                    return GetBetItemContent(expandParameters);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Get bet list group with specified bets
        /// </summary>
        /// <param name="bets"></param>
        /// <returns></returns>
        private static ListGroup<BetListGroupData> GetBetListGroup(List<Bet> bets)
        {
            return new ExpandableListGroup<BetListGroupData>(
                        bets.Select(bet => new BetListGroupData(bet))
                            .OrderByDescending(bet => bet.BetId)
                            .ToList(),
                        "betId",
                        new List<ItemField>
                        {
                            new ItemField("name", DataType.String),
                            new ItemField("playedDate", DataType.DateTime),
                            new ItemField("betResult", DataType.String)
                        },
                        new List<ItemField>
                        {
                            new ItemField("stake", DataType.Double, "Bet"),
                            new ItemField("odd", DataType.Double, "Odd")
                        },
                        componentKey: "betsListGroup");
        }

        /// <summary>
        /// Returns bet list group based on action's folder parameter
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private static ListGroup<BetListGroupData> GetBetListGroup(PageAction action)
        {
            var folderParameter = action.Parameters.GetString("FolderSelection");
            var folder = string.IsNullOrEmpty(folderParameter) || folderParameter == "Overview"
                ? null
                : folderParameter;

            return GetBetListGroup(new BetAction().GetBets(action.UserId, folder: folder));
        }

        private static ModalActionButton GetAddBetButton(List<string> folders)
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
                    new Field("odd", "Odd", FieldType.Double),
                    new Dropdown("folders", "Add to folders", folders){ MultipleSelection = true }
                },
                "Create a new bet");
        }

        /// <summary>
        /// Return content for expanded list group item
        /// </summary>
        /// <param name="expandParameters"></param>
        /// <returns></returns>
        private static ItemContent GetBetItemContent(ListGroupItemExpandParameters expandParameters)
        {
            var betId = Convert.ToInt32(expandParameters.ItemIdentifier);
            var bet = new BetAction().GetBet(betId, expandParameters.UserId);
            var folderAction = new FolderAction();
            var allFolders = folderAction.GetUsersFolders(expandParameters.UserId);
            var betsFolders = folderAction.GetUsersFolders(expandParameters.UserId, betId);

            var options = allFolders.Select(folder =>
                new Option(folder, folder, initialValue: betsFolders.Contains(folder)));

            var fields = new List<Field>
            {
                new Field("name", "Name", FieldType.TextBox),
                new Dropdown(
                    "folders",
                    "Add to folders",
                    allFolders
                        .Select(folder =>
                            new Option(folder, folder, initialValue: betsFolders.Contains(folder)))
                        .ToList()) { MultipleSelection = true },
                new Dropdown("betResult", "Result", new List<string>
                {
                    "Unresolved",
                    "Won",
                    "Lost"
                }),
                new Field("stake", "Bet", FieldType.Double),
                new Field("odd", "Odd", FieldType.Double)
            };

            var actions = new List<Button>
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
                    buttonStyle: "outline-danger",
                    displayType: DisplayType.Icon,
                    componentsToInclude: new List<string>{ "betsListGroup" })
                {
                    IconName = "far fa-trash-alt"
                }
            };

            return new ItemContent(fields, actions, new BetListGroupData(bet, betsFolders));
        }

        /// <summary>
        /// Modifies an existing bet
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private static PageActionResponse HandleModifyBet(PageAction action)
        {
            var dataKey = action.Parameters.GetKeyLike("listGroup-");

            var betParameters = (action.Parameters[dataKey] as JObject).ToObject<Dictionary<string, object>>();
            var betId = (int)action.Parameters.GetIdentifierFromKeyLike("listGroup-");

            new BetAction().ModifyBet(
                betId,
                action.UserId,
                EnumHelper.FromString<BetResult>(betParameters.GetString("betResult")),
                betParameters.GetDouble("stake"),
                betParameters.GetDouble("odd"),
                betParameters.GetString("name"));

            var folders = betParameters.ContainsKey("folders") && betParameters["folders"] != null
                ? (betParameters["folders"] as JArray).ToObject<List<string>>()
                : new List<string>();

            var folderAction = new FolderAction();

            var foldersOfBet = folderAction.GetUsersFolders(action.UserId, betId);

            // Add only to folders in which bet is not already
            var foldersToAdd = folders
                .Except(foldersOfBet)
                .ToList();

            // Remove from folders in which bet is not anymore
            var foldersToDelete = foldersOfBet
                .Except(folders)
                .ToList();

            if (foldersToAdd.Count > 0)
            {
                folderAction.AddBetToFolders(action.UserId, betId, foldersToAdd);
            }

            if (foldersToDelete.Count > 0)
            {
                folderAction.DeleteBetFromFolders(action.UserId, betId, foldersToDelete);
            }

            return new PageActionResponse(ActionResultType.OK, "Bet updated")
            {
                Components = new List<Component>
                {
                    GetBetListGroup(action)
                }
            };
        }

        /// <summary>
        /// Handles deleting a bet
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private static PageActionResponse HandleDeleteBet(PageAction action)
        {
            new BetAction().DeleteBet(
                (int)action.Parameters.GetIdentifierFromKeyLike("listGroup-"),
                action.UserId);

            return new PageActionResponse(ActionResultType.OK, "Bet deleted")
            {
                Components = new List<Component>
                {
                    GetBetListGroup(action)
                }
            };
        }

        private static PageActionResponse HandleAddBet(PageAction action)
        {
            var parameters = action.Parameters;

            double? odd = parameters.GetDouble("odd");
            double? stake = parameters.GetDouble("stake");

            if (odd == null || stake == null)
            {
                return new PageActionResponse(ActionResultType.InvalidInput, "Need to input odd and stake");
            }

            var betId = new BetAction().CreateBet(
                EnumHelper.FromString<BetResult>(parameters.GetString("betResult")),
                parameters.GetString("name"),
                (double)odd,
                (double)stake,
                DateTime.UtcNow,
                action.UserId);

            var folders = parameters.ContainsKey("folders") && parameters["folders"] != null
                ? (parameters["folders"] as JArray).ToObject<List<string>>()
                : null;

            if (folders?.Count > 0)
            {
                new FolderAction().AddBetToFolders(action.UserId, betId, folders);
            }

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

            public List<string> Folders { get; }

            public BetListGroupData(Bet bet)
            {
                BetId = bet.BetId;
                BetResult = bet.BetResult;
                Stake = bet.Stake;
                Odd = bet.Odd;
                PlayedDate = bet.PlayedDate;
                Name = bet.Name;
            }

            public BetListGroupData(Bet bet, List<string> folders)
                : this(bet)
            {
                Folders = folders;
            }
        }
    }
}

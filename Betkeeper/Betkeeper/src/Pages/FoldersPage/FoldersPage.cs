using Betkeeper.Actions;
using Betkeeper.Page;
using Betkeeper.Page.Components;
using Betkeeper.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Betkeeper.Pages.FoldersPage
{
    public class FoldersPage : PageBase
    {
        private FolderAction FolderAction { get; set; }

        public FoldersPage()
        {
            FolderAction = new FolderAction();
        }

        public override PageResponse GetPage(string pageKey, int userId)
        {
            var data = new FolderAction()
                .GetUsersFolders(userId)
                .Select(folder => new FolderListGroupData 
                {
                    Folder = folder
                })
                .ToList();

            return new PageResponse(
                new List<Component>
                {
                    new ModalActionButton(
                        "addFolder",
                        new List<Component>
                        {
                            new Field("folderName", "Name", FieldType.TextBox)
                        },
                        "Create folder"),
                    new PageActionButton(
                        "deleteFolders",
                        new List<string>{ "folderListGroup" },
                        "Delete selected folders",
                        "outline-danger",
                        requireConfirm: true),
                    new ListGroup<FolderListGroupData>(
                        ListGroupMode.Selectable,
                        data,
                        "folder",
                        new List<string>{ "folder" },
                        componentKey: "folderListGroup")
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
                case "deleteFolders":
                    return DeleteFolders(action);

                case "addFolder":
                    return AddFolder(action);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes specified folders from user
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private PageActionResponse DeleteFolders(PageAction action)
        {
            var jArray = action.Parameters.ContainsKey("folderListGroup")
                        ? action.Parameters["folderListGroup"] as JArray
                        : null;

            if (jArray == null || jArray.Count == 0)
            {
                return new PageActionResponse(Enums.ActionResultType.InvalidInput, "No folders selected");
            }

            var foldersToDelete = jArray.Select(folder => folder.ToString()).ToList();

            foldersToDelete.ForEach(folder =>
            {
                FolderAction.DeleteFolder(action.UserId, folder);
            });

            return new PageActionResponse(
                Enums.ActionResultType.OK,
                $"Deleted folders: {string.Join(",", foldersToDelete)}",
                refresh: true);
        }

        /// <summary>
        /// Adds a new folder for user if user hasn't got folder with same name
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private PageActionResponse AddFolder(PageAction action)
        {
            var folderName = action.Parameters.GetString("folderName");
            FolderAction.AddFolder(action.UserId, folderName);

            return new PageActionResponse(
                Enums.ActionResultType.Created,
                $"Created folder {folderName}",
                refresh: true);
        }

        public class FolderListGroupData
        {
            public string Folder { get; set; }
        }
    }
}

export const FETCH_FOLDERS = "FETCH_FOLDERS";
export const FETCH_FOLDERS_SUCCESS = "FETCH_FOLDERS_SUCCESS";
export const POST_FOLDER = "POST_FOLDER";
export const DELETE_FOLDER = "DELETE_FOLDER";

export const fetchFolders = () => ({
  type: "FETCH_FOLDERS"
});

export const fetchFoldersSuccess = (folders) => ({
  type: "FETCH_FOLDERS_SUCCESS",
  folders
});

export const postFolder = (newFolderName) => ({
  type: "POST_FOLDER",
  newFolderName
});

export const deleteFolder = (folderToDelete) => ({
  type: "DELETE_FOLDER",
  folderToDelete
});

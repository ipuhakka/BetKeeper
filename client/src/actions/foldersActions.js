export const FETCH_FOLDERS = "FETCH_FOLDERS";
export const FETCH_FOLDERS_SUCCESS = "FETCH_FOLDERS_SUCCESS";

export const fetchFolders = () => ({
  type: "FETCH_FOLDERS"
});

export const fetchFoldersSuccess = (folders) => ({
  type: "FETCH_FOLDERS_SUCCESS",
  folders
});

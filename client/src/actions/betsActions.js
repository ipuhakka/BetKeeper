export const FETCH_BETS = "FETCH_BETS";
export const FETCH_BETS_SUCCESS = "FETCH_BETS_SUCCESS";
export const FETCH_BETS_FROM_FOLDER = "FETCH_BETS_FROM_FOLDER";
export const FETCH_BETS_FROM_FOLDER_SUCCESS = "FETCH_BETS_FROM_FOLDER_SUCCESS";
export const FETCH_BETS_FROM_ALL_FOLDERS = "FETCH_BETS_FROM_ALL_FOLDERS";
export const FETCH_BETS_FROM_ALL_FOLDERS_SUCCESS = "FETCH_BETS_FROM_ALL_FOLDERS_SUCCESS";
export const FETCH_UNRESOLVED_BETS = "FETCH_UNRESOLVED_BETS";
export const FETCH_UNRESOLVED_BETS_SUCCESS = "FETCH_UNRESOLVED_BETS_SUCCESS";
export const FETCH_FINISHED_BETS = "FETCH_FINISHED_BETS";
export const FETCH_FINISHED_BETS_SUCCESS = "FETCH_FINISHED_BETS_SUCCESS";
export const POST_BET = "POST_BET";
export const PUT_BET = "PUT_BET";
export const PUT_BETS = "PUT_BETS";
export const DELETE_BET = "DELETE_BET";

export const fetchBets = () => ({
  type: "FETCH_BETS"
});

export const fetchBetsSuccess = (bets) => ({
  type: "FETCH_BETS_SUCCESS",
  bets
});

export const fetchBetsFromFolder = (folder) => ({
  type: "FETCH_BETS_FROM_FOLDER",
  folder
});

export const fetchBetsFromFolderSuccess = (bets) => ({
  type: "FETCH_BETS_FROM_FOLDER_SUCCESS",
  bets
});

export const fetchBetsFromAllFolders = () => ({
  type: "FETCH_BETS_FROM_ALL_FOLDERS"
});

export const fetchBetsFromAllFoldersSuccess = (betFolders) => ({
  type: "FETCH_BETS_FROM_ALL_FOLDERS_SUCCESS",
  betFolders
});

export const fetchUnresolvedBets = () => ({
  type: "FETCH_UNRESOLVED_BETS"
});

export const fetchUnresolvedBetsSuccess = (bets) => ({
  type: "FETCH_UNRESOLVED_BETS_SUCCESS",
  bets
});

export const fetchFinishedBets = () => ({
  type: "FETCH_FINISHED_BETS"
});

export const fetchFinishedBetsSuccess = (bets) => ({
  type: "FETCH_FINISHED_BETS_SUCCESS",
  bets
});

export const postBet = (bet) => ({
  type: "POST_BET",
  bet
});

export const putBet = (betId, data) => ({
  type: "PUT_BET",
  data
});

export const putBets = (betIds, betsData) => ({
  type: "PUT_BETS",
  payload: {
    betIds,
    betsData
  }
});

export const deleteBet = (betId, folders) => ({
  type: "DELETE_BET",
  betId, folders
});

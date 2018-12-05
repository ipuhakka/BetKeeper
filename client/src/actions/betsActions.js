export const FETCH_BETS = "FETCH_BETS";
export const FETCH_BETS_SUCCESS = "FETCH_BETS_SUCCESS";
export const FETCH_BETS_FROM_FOLDER = "FETCH_BETS_FROM_FOLDER";
export const FETCH_BETS_FROM_FOLDER_SUCCESS = "FETCH_BETS_FROM_FOLDER_SUCCESS";

export const fetchBets = () => ({
  type: "FETCH_BETS"
});

export const fetchBetsSuccess = (bets) => ({
  type: "FETCH_BETS_SUCCESS",
  bets
});

export const fetchBetsFromFolder = () => ({
  type: "FETCH_BETS_FROM_FOLDER"
});

export const fetchBetsFromFolderSuccess = (bets) => ({
  type: "FETCH_BETS_FROM_FOLDER_SUCCESS",
  bets
});

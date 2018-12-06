export const FETCH_BETS = "FETCH_BETS";
export const FETCH_BETS_SUCCESS = "FETCH_BETS_SUCCESS";
export const FETCH_BETS_FROM_FOLDER = "FETCH_BETS_FROM_FOLDER";
export const FETCH_BETS_FROM_FOLDER_SUCCESS = "FETCH_BETS_FROM_FOLDER_SUCCESS";
export const FETCH_UNRESOLVED_BETS = "FETCH_UNRESOLVED_BETS";
export const FETCH_UNRESOLVED_BETS_SUCCESS = "FETCH_UNRESOLVED_BETS_SUCCESS";
export const POST_BET = "POST_BET";
export const PUT_BET = "PUT_BET";

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

export const fetchUnresolvedBets = () => ({
  type: "FETCH_UNRESOLVED_BETS"
});

export const fetchUnresolvedBetsSuccess = (bets) => ({
  type: "FETCH_UNRESOLVED_BETS_SUCCESS",
  bets
});

export const postBet = (bet) => ({
  type: "POST_BET",
  bet
});

export const putBet = (data) => ({
  type: "PUT_BET",
  data
});

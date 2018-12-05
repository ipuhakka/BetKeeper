export const FETCH_BETS = "FETCH_BETS";
export const FETCH_BETS_SUCCESS = "FETCH_BETS_SUCCESS";

export const fetchBets = () => ({
  type: "FETCH_BETS"
});

export const fetchBetsSuccess = (bets) => ({
  type: "FETCH_BETS_SUCCESS",
  bets
});

import React, { Component } from 'react';
import { connect } from 'react-redux';
import _ from 'lodash';
import store from '../store';
import {fetchFolders} from '../actions/foldersActions';
import {fetchFinishedBets} from '../actions/betsActions';
import Statistics from '../views/Statistics/Statistics.jsx';
import * as Stats from '../js/stats.js';

class StatisticsContainer extends Component{

  componentDidMount()
  {
		this.props.fetchFinishedBets();
		this.props.fetchFolders();
	}

  constructor(props){
    super(props);

    let graphOptions = [
      {labelName:"Money returned", variableName: "moneyReturned", key: 0},
      {labelName:"Verified return", variableName: "verifiedReturn", key: 1},
      {labelName:"Money played", variableName: "moneyPlayed", key: 2},
      {labelName: "Money won", variableName: "moneyWon", key: 3},
      {labelName: "Average return", variableName: "avgReturn", key: 4},
      {labelName:"Won bets", variableName: "wonBets", key: 5},
      {labelName:"Played bets", variableName: "playedBets", key: 6},
      {labelName:"Win percentage", variableName: "winPercentage", key: 7},
      {labelName: "Expected return", variableName: "expectedReturn", key: 8},
      {labelName:"Odd median", variableName: "oddMedian", key: 9},
      {labelName:"Bet median", variableName: "betMedian", key: 10},
      {labelName: "Odd mean", variableName: "oddMean", key: 11},
      {labelName: "Bet mean", variableName: "betMean", key: 12}];

    this.state = {
      graphOptions: graphOptions,
      betStatistics: []
    };
  }

  componentDidUpdate(prevProps)
  {
    const { folders, betsFromAllFolders, finishedBets} = this.props;
    if (!_.isEqual(folders, prevProps.folders))
    {
			this.getBetsFromFolders(folders);
		}

    if (!_.isEqual(betsFromAllFolders, prevProps.betsFromAllFolders))
    {
			betsFromAllFolders.forEach(folder => {
				this.updateTable(folder);
			});
		}

    if (!_.isEqual(finishedBets, prevProps.finishedBets))
    {
			this.handleGetAllFinishedBets(finishedBets);
		}
  }

  render(){
    return <Statistics {...this.props} {...this.state}/>;
  }

  /*takes a betFolder as parameter. Folder = {
    folder: "name",
    bets: []
  } */
  updateTable = (betFolder) => {
    var moneyWon, moneyPlayed, moneyReturned, wonBets, playedBets, winPercentage, avgReturn, expectedReturn, verifiedReturn,
     oddMedian, oddMean, betMedian, betMean;
    let name = betFolder.folder;
    moneyWon = Stats.roundByTwo(Stats.moneyWon(betFolder.bets));
    moneyPlayed = Stats.roundByTwo(Stats.moneyPlayed(betFolder.bets));
    moneyReturned = Stats.roundByTwo(Stats.moneyReturned(betFolder.bets));
    wonBets = Stats.roundByTwo(Stats.wonBets(betFolder.bets));
    playedBets = Stats.roundByTwo(Stats.playedBets(betFolder.bets));
    winPercentage = Stats.roundByTwo(Stats.winPercentage(betFolder.bets));
    avgReturn = Stats.roundByTwo(Stats.avgReturn(betFolder.bets));
    expectedReturn = Stats.roundByTwo(Stats.expectedReturn(betFolder.bets));
    verifiedReturn = Stats.roundByTwo(Stats.verifiedReturn(betFolder.bets));
    oddMedian = Stats.roundByTwo(Stats.median(betFolder.bets, "odd"));
    oddMean = Stats.roundByTwo(Stats.mean(betFolder.bets, "odd"));
    betMedian = Stats.roundByTwo(Stats.median(betFolder.bets, "stake"));
    betMean = Stats.roundByTwo(Stats.mean(betFolder.bets, "stake"));

    let folderStats = {
        folder: name,
        moneyWon: moneyWon,
        moneyPlayed: moneyPlayed,
        moneyReturned: moneyReturned,
        wonBets: wonBets,
        playedBets: playedBets,
        winPercentage: winPercentage,
        avgReturn: avgReturn,
        expectedReturn: expectedReturn,
        verifiedReturn: verifiedReturn,
        oddMedian: oddMedian,
        oddMean: oddMean,
        betMedian: betMedian,
        betMean: betMean
    }

    let stats = this.state.betStatistics;
    if (!stats.some(e => e.folder === name)){
      stats.push(folderStats);
      this.setState({
        betStatistics: stats
      });
    }
  }

  handleGetAllFinishedBets = (data) => {
    this.updateTable({folder: "Overview", bets: data});
  }

  getBetsFromFolders = (folders) => {
    store.dispatch({type: 'FETCH_BETS_FROM_ALL_FOLDERS', payload: {
        folders: folders
      }
    });
  }

};

const mapStateToProps = (state, ownProps) => {
  return { ...state.folders, ...state.bets}
};

const mapDispatchToProps = (dispatch) => ({
  fetchFolders: () => dispatch(fetchFolders()),
	fetchFinishedBets: () => dispatch(fetchFinishedBets())
});

export default connect(mapStateToProps, mapDispatchToProps)(StatisticsContainer);

import React, { Component } from 'react';
import { connect } from 'react-redux';
import store from '../../store';
import {fetchFolders} from '../../actions/foldersActions';
import {fetchFinishedBets} from '../../actions/betsActions';
import Row from 'react-bootstrap/Row';
import Col from 'react-bootstrap/Col';
import BarGraph from '../../components/BarGraph/BarGraph.jsx';
import Dropdown from '../../components/Dropdown/Dropdown.jsx';
import Header from '../../components/Header/Header.jsx';
import Info from '../../components/Info/Info.jsx';
import Menu from '../../components/Menu/Menu.jsx';
import ObjectTable from '../../components/ObjectTable/ObjectTable.jsx';
import OverviewTable from '../../components/OverviewTable/OverviewTable.jsx';
import ScatterPlot from '../../components/ScatterPlot/ScatterPlot.jsx';
import * as Stats from '../../js/stats.js';
import './Statistics.css';

class Statistics extends Component{
	constructor(props){
		super(props);

		let graphOptions = [{labelName:"Money returned", variableName: "moneyReturned", key: 0},
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
			disabled: [false, false, true, false, false],
			folderSelected: 0,
			graphOptions: graphOptions,
			betStatistics: [],
			sortedBetStatistics: [],
			sortOrderHigh: false,
			lastSortedKey: ''
		};
	}

	componentWillReceiveProps(nextProps){
		if (this.props.folders !== nextProps.folders){
			this.getBetsFromFolders(nextProps.folders);
		}

		if (this.props.betsFromAllFolders !== nextProps.betsFromAllFolders){
			nextProps.betsFromAllFolders.forEach(folder => {
				this.updateTable(folder);
			});
		}

		if (this.props.finishedBets !== nextProps.finishedBets){
			this.handleGetAllFinishedBets(nextProps.finishedBets);
		}
	}

	render(){
		const {state} = this;

		return(
			<div className="content" onLoad={this.onLoad}>
				<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
				<Menu disable={state.disabled}></Menu>
				<Info></Info>
				<div>
					<Row>
						<Col md={6} xs={12}>
							<BarGraph data={state.betStatistics} optionLabels={this.graphOptionLabels()}
								graphOptions={state.graphOptions} />
							<div>
								<OverviewTable overviewItems={state.sortedBetStatistics}/>
							</div>
						</Col>
						<Col md={6} xs={12}>
							<Dropdown
								variant="primary"
								title={"Show folder"}
								id={2}
								data={this.folders()}
								onUpdate={this.setSelectedDropdownItem.bind(this)}
								stateKey={"folderSelected"}>
							</Dropdown>
							{this.renderTable()}
						</Col>
					</Row>
					<Row>
						<Col>
								<ScatterPlot optionLabels={this.graphOptionLabels()} data={this.state.betStatistics}
								graphOptions={this.state.graphOptions}></ScatterPlot>
						</Col>
					</Row>
				</div>
			</div>
		);
	}

	renderTable = () => {
		const {folderSelected, graphOptions, sortedBetStatistics} = this.state;

		const titleOptions = graphOptions.map(option => {
			return ({
				key: option.variableName,
				value: option.labelName
			});
		})

		return sortedBetStatistics.length > 0 ?
				<ObjectTable
						tableTitle={sortedBetStatistics[folderSelected].folder}
						data={sortedBetStatistics[folderSelected]}
						titles={titleOptions}/> : null;
	}

	folders = () => {
		return this.state.betStatistics.map(folder => {
			return folder.folder;
		})
	};

	graphOptionLabels = () => {
		return this.state.graphOptions.map(option => {
			return option.labelName;
		});
	}

	setSelectedDropdownItem(key, stateKey){
		this.setState({
			[stateKey]: key
		});
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
		betMedian = Stats.roundByTwo(Stats.median(betFolder.bets, "bet"));
		betMean = Stats.roundByTwo(Stats.mean(betFolder.bets, "bet"));

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
				betStatistics: stats,
				sortedBetStatistics: [...stats] //initialize sortedBetStatistics with the same list
			});
		}
	}

	onLoad = () => {
		this.props.fetchFinishedBets();
		this.props.fetchFolders();
	}

	getBetsFromFolders = (folders) => {
		store.dispatch({type: 'FETCH_BETS_FROM_ALL_FOLDERS', payload: {
				folders: folders
			}
		});
	}

	handleGetAllFinishedBets = (data) => {
		this.updateTable({folder: "Overview", bets: data});
	}

	updateBetStatistics(betFolders){
		betFolders.forEach(folder => {
			this.updateTable(folder);
		});
	}
}

const mapStateToProps = (state, ownProps) => {
  return { ...state.folders, ...state.bets}
};

const mapDispatchToProps = (dispatch) => ({
  fetchFolders: () => dispatch(fetchFolders()),
	fetchFinishedBets: () => dispatch(fetchFinishedBets())
});

export default connect(mapStateToProps, mapDispatchToProps)(Statistics);

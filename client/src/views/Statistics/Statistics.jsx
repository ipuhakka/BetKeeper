import React, { Component } from 'react';
import { connect } from 'react-redux';
import store from '../../store';
import {fetchFolders} from '../../actions/foldersActions';
import {fetchFinishedBets} from '../../actions/betsActions';
import Table from 'react-bootstrap/lib/Table';
import Row from 'react-bootstrap/lib/Grid';
import Col from 'react-bootstrap/lib/Grid';
import BarGraph from '../../components/BarGraph/BarGraph.jsx';
import Dropdown from '../../components/Dropdown/Dropdown.jsx';
import Header from '../../components/Header/Header.jsx';
import Info from '../../components/Info/Info.jsx';
import Menu from '../../components/Menu/Menu.jsx';
import ScatterPlot from '../../components/ScatterPlot/ScatterPlot.jsx';
import * as Stats from '../../js/Stats.js';
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
			betStatistics: []
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
		var table = this.renderTable();
		var overview = this.renderOverviewTable();

		return(
			<div className="content" onLoad={this.onLoad}>
				<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
				<Menu disable={this.state.disabled}></Menu>
				<Info></Info>
				<div>
					<Row className="show-grid">
						<Col className="col-md-6 col-xs-12">
							<BarGraph data={this.state.betStatistics} optionLabels={this.graphOptionLabels()} graphOptions={this.state.graphOptions} />
							<div>
								{overview}
							</div>
						</Col>
						<Col className="col-md-6 col-xs-12">
							<Dropdown
								bsStyle="primary"
								title={"Show folder"}
								id={2}
								data={this.folders()}
								onUpdate={this.setSelectedDropdownItem.bind(this)}
								stateKey={"folderSelected"}>
							</Dropdown>
							{table}
						</Col>
					</Row>
					<Row>
						<Col>
								<ScatterPlot optionLabels={this.graphOptionLabels()} data={this.state.betStatistics} graphOptions={this.state.graphOptions}></ScatterPlot>
						</Col>
					</Row>
				</div>
			</div>
		);
	}

	renderOverviewTable = () => {
		var tableItems = [];
		var overviewItems = this.state.betStatistics;
		for (var i = 0; i < this.state.betStatistics.length; i++){
			tableItems.push(<tr key={i}>
								<td>{overviewItems[i]["folder"]}</td>
								<td className={overviewItems[i]["moneyReturned"] >= 0 ? 'tableGreen' : 'tableRed'}>{overviewItems[i]["moneyReturned"]}</td>
								<td className={overviewItems[i]["verifiedReturn"] >= 1 ? 'tableGreen' : 'tableRed'}>{overviewItems[i]["verifiedReturn"]}</td>
								<td>{overviewItems[i]["winPercentage"]}</td>
							</tr>)
		}

		return(<Table>
					<thead>
						<tr>
							<th className="clickable" onClick={this.sort.bind(this, Stats.sortAlphabetically, "folder")}>{"Folder"}</th>
							<th className="clickable" onClick={this.sort.bind(this, Stats.sortByHighest, "moneyReturned")}>{"Money returned"}</th>
							<th className="clickable" onClick={this.sort.bind(this,Stats.sortByHighest, "verifiedReturn")}>{"Return percentage"}</th>
							<th className="clickable" onClick={this.sort.bind(this, Stats.sortByHighest, "winPercentage")}>{"Win percentage"}</th>
						</tr>
					</thead>
					<tbody>{tableItems}</tbody>
				</Table>);
	}

	renderTable = () => {
		var index = this.state.folderSelected;
		if (this.state.betStatistics.length > 0){
			return (
			<Table>
				<thead>
					<tr>
						<th>{this.state.betStatistics[index].folder}</th>
					</tr>
				</thead>
				<tbody>
					<tr>
						<td>{"Money played"}</td>
						<td>{this.state.betStatistics[index].moneyPlayed}</td>
					</tr>
					<tr>
						<td>{"Money won"}</td>
						<td>{this.state.betStatistics[index].moneyWon}</td>
					</tr>
						<tr className={this.state.betStatistics[index].moneyReturned >= 0 ? 'tableGreen' : 'tableRed'}>
							<td>{"Return"}</td>
							<td>{this.state.betStatistics[index].moneyReturned}</td>
						</tr>
						<tr>
							<td>{"Won/played"}</td>
							<td>{this.state.betStatistics[index].wonBets + "/" + this.state.betStatistics[index].playedBets + "	" + (Stats.roundByTwo(this.state.betStatistics[index].winPercentage) * 100) + "%"}</td>
						</tr>
						<tr className={this.state.betStatistics[index].avgReturn >= 0 ? 'tableGreen' : 'tableRed'}>
							<td>{"Average return (cash)"}</td>
							<td>{this.state.betStatistics[index].avgReturn}</td>
						</tr>
						<tr>
							<td>{"Expected return"}</td>
							<td>{this.state.betStatistics[index].expectedReturn}</td>
						</tr>
						<tr className={this.state.betStatistics[index].verifiedReturn >= 1 ? 'tableGreen' : 'tableRed'}>
							<td>{"Verified return"}</td>
							<td>{this.state.betStatistics[index].verifiedReturn}</td>
						</tr>
						<tr>
							<td>{"Odd median"}</td>
							<td>{this.state.betStatistics[index].oddMedian}</td>
						</tr>
						<tr>
							<td>{"Odd mean"}</td>
							<td>{this.state.betStatistics[index].oddMean}</td>
						</tr>
						<tr>
							<td>{"Bet median"}</td>
							<td>{this.state.betStatistics[index].betMedian}</td>
						</tr>
						<tr>
							<td>{"Bet mean"}</td>
							<td>{this.state.betStatistics[index].betMean}</td>
						</tr>
				</tbody>
			</Table>
			);
		} else {
			return null;
		}
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
				betStatistics: stats
			});
		}
	}

	sort = (func, param) => {
		var sorted = func(this.state.overviewItems, param);

		this.setState({
			overviewItems: sorted
		});
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
		})
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

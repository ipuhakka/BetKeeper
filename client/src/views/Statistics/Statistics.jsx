import React, { Component } from 'react';
import { connect } from 'react-redux';
import store from '../../store';
import {fetchFolders} from '../../actions/foldersActions';
import {fetchFinishedBets} from '../../actions/betsActions';
import { Scatter, ScatterChart, BarChart, Bar, CartesianGrid, XAxis, YAxis, Legend, ResponsiveContainer } from 'recharts';
import MenuItem from 'react-bootstrap/lib/MenuItem';
import Table from 'react-bootstrap/lib/Table';
import DropdownButton from 'react-bootstrap/lib/DropdownButton';
import Row from 'react-bootstrap/lib/Grid';
import Col from 'react-bootstrap/lib/Grid';
import Header from '../../components/Header/Header.jsx';
import Info from '../../components/Info/Info.jsx';
import Menu from '../../components/Menu/Menu.jsx';
import * as Stats from '../../js/Stats.js';
import './Statistics.css';

class Statistics extends Component{
	constructor(props){
		super(props);

		let graphOptions = [{labelName:"Money returned", variableName: "moneyReturned", key: 0},
			{labelName:"Return coefficient", variableName: "verifiedReturn", key: 1},
			{labelName:"Win percentage", variableName: "winPercentage", key: 2}];

		this.state = {
			disabled: [false, false, true, false, false],
			folderSelected: 0,
			graphOptions: graphOptions,
			selectedGraphVariable: 0,
			moneyPlayed: 0,
			moneyWon: 0,
			moneyReturned: 0,
			wonBets: 0,
			playedBets: 0,
			winPercentage: 0,
			avgReturn: 0,
			expectedReturn: 1,
			verifiedReturn: 1,
			oddMean: 0,
			oddMedian: 0,
			betMean: 0,
			betMedian: 0,
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
		var menuItems = this.renderDropdown();
		var table = this.renderTable();
		var overview = this.renderOverviewTable();
		var scatter = this.renderScatterPlot();

		return(
			<div className="content" onLoad={this.onLoad}>
				<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
				<Menu disable={this.state.disabled}></Menu>
				<Info></Info>
				<div>
					<Row className="show-grid">
						<Col className="col-md-6 col-xs-12">
							<DropdownButton
								bsStyle="primary"
								title={"Overview"}
								id={1}>
								{this.graphDropDown()}
							</DropdownButton>
							{this.barGraph()}
							<div>
								{overview}
							</div>
						</Col>
						<Col className="col-md-6 col-xs-12">
							<DropdownButton
								bsStyle="primary"
								title={"Show folder"}
								id={1}>
								{menuItems}
							</DropdownButton>
							{table}
						</Col>
					</Row>
					<Row>
						<Col>{scatter}</Col>
					</Row>
				</div>
			</div>
		);
	}

	renderScatterPlot = () => {
		return (
			<div className="chart">
				<ResponsiveContainer>
					<ScatterChart>
						<CartesianGrid strokeDasharray="3 3" />
						<XAxis dataKey="name" />
						<YAxis />
						<Legend />
					</ScatterChart>
				</ResponsiveContainer>
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
						<tr className={this.state.avgReturn >= 0 ? 'tableGreen' : 'tableRed'}>
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

	renderDropdown = () => {
		var menuItems = [];
		var active = false;
		for (var k = 0; k < this.state.betStatistics.length; k++){
			active = false;
			if (k === this.state.folderSelected)
				active = true;
			menuItems.push(<MenuItem onClick={this.showFromFolder.bind(this, k)} key={k} active={active} eventKey={k}>{this.state.betStatistics[k].folder}</MenuItem>);
		}
		return menuItems;
	}

	graphDropDown = () => {
		var menuItems = [];
		menuItems.push(<MenuItem onClick={this.setGraphVariable.bind(this, 0)} key={0} active={this.state.selectedGraphVariable === 0} eventKey={0}>{"Money returned"}</MenuItem>);
		menuItems.push(<MenuItem onClick={this.setGraphVariable.bind(this, 1)} key={1} active={this.state.selectedGraphVariable === 1} eventKey={1}>{"Return coefficient"}</MenuItem>);
		menuItems.push(<MenuItem onClick={this.setGraphVariable.bind(this, 2)} key={2} active={this.state.selectedGraphVariable === 2} eventKey={2}>{"Win percentage"}</MenuItem>);
		return menuItems;
	}

	//slicing data prop for BarChart apparently triggers chart redraw, without it fails to render correctly
	barGraph = () => {
		return(
			<div className="chart">
				<ResponsiveContainer>
					<BarChart barSize={20} barGap={2} data={this.state.betStatistics.slice()}>
						<CartesianGrid strokeDasharray="3 3" />
						<XAxis dataKey="folder" />
						<YAxis />
						<Legend />
						<Bar name={this.state.graphOptions[this.state.selectedGraphVariable].labelName} dataKey={this.state.graphOptions[this.state.selectedGraphVariable].variableName} fill="#8884d8" />
					</BarChart>
				</ResponsiveContainer>
			</div>
		);
	}

	setGraphVariable(key){
		this.setState({
			selectedGraphVariable: key
		});
	}

	showFromFolder = (key) => {
		this.setState({
			folderSelected: key
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

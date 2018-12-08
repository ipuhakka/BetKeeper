import React, { Component } from 'react';
import { connect } from 'react-redux';
import store from '../../store';
import {fetchFolders} from '../../actions/foldersActions';
import {fetchFinishedBets} from '../../actions/betsActions';
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

		this.state = {
			disabled: [false, false, true, false, false],
			folderSelected: -1,
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
			overviewItems: []
		};
	}

	componentWillReceiveProps(nextProps){
		if (this.props.folders !== nextProps.folders){
			this.getBetsFromFolders(nextProps.folders);
		}

		if (this.props.betsFromAllFolders !== nextProps.betsFromAllFolders){
			nextProps.betsFromAllFolders.forEach(folder => {
				this.calculateOverviewValues(folder);
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

		return(
			<div className="content" onLoad={this.onLoad}>
				<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
				<Menu disable={this.state.disabled}></Menu>
				<Info></Info>
				<div>
					<Row className="show-grid">
						<Col className="col-md-6 col-xs-12">
							{overview}
						</Col>
						<Col className="col-md-6 col-xs-12">
							<DropdownButton
								bsStyle="info"
								title={"Show folder"}
								id={1}>
								{menuItems}
							</DropdownButton>
							{table}
						</Col>
					</Row>
				</div>
			</div>
		);
	}

	renderOverviewTable = () => {
		var tableItems = [];
		var overviewItems = this.state.overviewItems;
		for (var i = 0; i < this.state.overviewItems.length; i++){
			tableItems.push(<tr key={i}>
								<td>{overviewItems[i]["name"]}</td>
								<td className={overviewItems[i]["moneyReturned"] >= 0 ? 'tableGreen' : 'tableRed'}>{overviewItems[i]["moneyReturned"]}</td>
								<td className={overviewItems[i]["verifiedReturn"] >= 1 ? 'tableGreen' : 'tableRed'}>{overviewItems[i]["verifiedReturn"]}</td>
								<td>{overviewItems[i]["winPercentage"]}</td>
							</tr>)
		}

		return(<Table>
					<thead>
						<tr>
							<th className="clickable" onClick={this.sort.bind(this, Stats.sortAlphabetically, "name")}>{"Name"}</th>
							<th className="clickable" onClick={this.sort.bind(this, Stats.sortByHighest, "moneyReturned")}>{"Money returned"}</th>
							<th className="clickable" onClick={this.sort.bind(this,Stats.sortByHighest, "verifiedReturn")}>{"Return percentage"}</th>
							<th className="clickable" onClick={this.sort.bind(this, Stats.sortByHighest, "winPercentage")}>{"Win percentage"}</th>
						</tr>
					</thead>
					<tbody>{tableItems}</tbody>
				</Table>);
	}

	renderTable = () => {
		var title = "Overview";

		if (this.state.folderSelected !== -1)
			title = this.props.betsFromAllFolders[this.state.folderSelected]["folder"];

		return (
		<Table>
			<thead>
				<tr>
					<th>{title}</th>
				</tr>
			</thead>
			<tbody>
				<tr>
					<td>{"Money played"}</td>
					<td>{this.state.moneyPlayed}</td>
				</tr>
				<tr>
					<td>{"Money won"}</td>
					<td>{this.state.moneyWon}</td>
				</tr>
					<tr className={this.state.moneyReturned >= 0 ? 'tableGreen' : 'tableRed'}>
						<td>{"Return"}</td>
						<td>{this.state.moneyReturned}</td>
					</tr>
					<tr>
						<td>{"Won/played"}</td>
						<td>{this.state.wonBets + "/" + this.state.playedBets + "	" + (Stats.roundByTwo(this.state.winPercentage) * 100) + "%"}</td>
					</tr>
					<tr className={this.state.avgReturn >= 0 ? 'tableGreen' : 'tableRed'}>
						<td>{"Average return (cash)"}</td>
						<td>{this.state.avgReturn}</td>
					</tr>
					<tr>
						<td>{"Expected return"}</td>
						<td>{this.state.expectedReturn}</td>
					</tr>
					<tr className={this.state.verifiedReturn >= 1 ? 'tableGreen' : 'tableRed'}>
						<td>{"Verified return"}</td>
						<td>{this.state.verifiedReturn}</td>
					</tr>
					<tr>
						<td>{"Odd median"}</td>
						<td>{this.state.oddMedian}</td>
					</tr>
					<tr>
						<td>{"Odd mean"}</td>
						<td>{this.state.oddMean}</td>
					</tr>
					<tr>
						<td>{"Bet median"}</td>
						<td>{this.state.betMedian}</td>
					</tr>
					<tr>
						<td>{"Bet mean"}</td>
						<td>{this.state.betMean}</td>
					</tr>
			</tbody>
		</Table>
		);
	}

	renderDropdown = () => {
		var menuItems = [];
		menuItems.push(<MenuItem onClick={this.showFromFolder.bind(this, -1)} key={-1} active={this.state.folderSelected === -1} eventKey={-1}>{"Overview"}</MenuItem>);
		var active = false;
		for (var k = 0; k < this.props.folders.length; k++){
			active = false;
			if (k === this.state.folderSelected)
				active = true;
			menuItems.push(<MenuItem onClick={this.showFromFolder.bind(this, k)} key={k} active={active} eventKey={k}>{this.props.folders[k]}</MenuItem>);
		}
		return menuItems;
	}

	showFromFolder = (key) => {
		this.setState({
			folderSelected: key
		}, () => {this.updateTable(this.props.finishedBets)});
	}

	//Calculates the values that are used in the overview table. Function is performed after a bet folder has been received.
	calculateOverviewValues = (betFolder) => {
		var overviewItems = this.state.overviewItems;
		for (var i = 0; i < overviewItems.length; i++){
			if (overviewItems[i].name === betFolder.folder){
				return;
			}
		}

		var name = betFolder.folder;
		var moneyReturned = Stats.roundByTwo(Stats.moneyReturned(betFolder.bets));
		var verifiedReturn = Stats.roundByTwo(Stats.verifiedReturn(betFolder.bets));
		var winPercentage = Stats.roundByTwo(Stats.winPercentage(betFolder.bets));

		overviewItems.push({
			name: name,
			moneyReturned: moneyReturned,
			verifiedReturn: verifiedReturn,
			winPercentage: winPercentage
		});

		this.setState({
			overviewItems: overviewItems
		});
	}

	updateTable = (allBets) => {
		var moneyWon, moneyPlayed, moneyReturned, wonBets, playedBets, winPercentage, avgReturn, expectedReturn, verifiedReturn, oddMedian, oddMean,
		betMedian, betMean;
		var param;
		if (this.state.folderSelected === -1)
			param = allBets;
		else
			param = this.props.betsFromAllFolders[this.state.folderSelected]["bets"];

		moneyWon = Stats.roundByTwo(Stats.moneyWon(param));
		moneyPlayed = Stats.roundByTwo(Stats.moneyPlayed(param));
		moneyReturned = Stats.roundByTwo(Stats.moneyReturned(param));
		wonBets = Stats.roundByTwo(Stats.wonBets(param));
		playedBets = Stats.roundByTwo(Stats.playedBets(param));
		winPercentage = Stats.roundByTwo(Stats.winPercentage(param));
		avgReturn = Stats.roundByTwo(Stats.avgReturn(param));
		expectedReturn = Stats.roundByTwo(Stats.expectedReturn(param));
		verifiedReturn = Stats.roundByTwo(Stats.verifiedReturn(param));
		oddMedian = Stats.roundByTwo(Stats.median(param, "odd"));
		oddMean = Stats.roundByTwo(Stats.mean(param, "odd"));
		betMedian = Stats.roundByTwo(Stats.median(param, "bet"));
		betMean = Stats.roundByTwo(Stats.mean(param, "bet"));

		this.setState({
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
		});
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

	/*
	Callback for GET-folders request.
	For every folder, get-request is made to get bets
	in that folder.
	*/
	getBetsFromFolders = (folders) => {
		store.dispatch({type: 'FETCH_BETS_FROM_ALL_FOLDERS', payload: {
				folders: folders
			},
		});
	}

	handleGetAllFinishedBets = (data) => {
			this.updateTable(data);
			this.calculateOverviewValues({name: "", bets: data});
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

import React, { Component } from 'react';
import MenuItem from 'react-bootstrap/lib/MenuItem';
import Table from 'react-bootstrap/lib/Table';
import DropdownButton from 'react-bootstrap/lib/DropdownButton';
import Row from 'react-bootstrap/lib/Grid';
import Col from 'react-bootstrap/lib/Grid';
import Header from '../../Header/Header.jsx';
import Info from '../../Info/Info.jsx';
import Menu from '../../Menu/Menu.jsx';
import * as Stats from '../../../js/Stats.js';
import {getFolders} from '../../../js/Requests/Folders.js';
import {getBetsFromFolder, getFinishedBets} from '../../../js/Requests/Bets.js';
import './Statistics.css';

class Statistics extends Component{
	constructor(props){
		super(props);

		this.state = {
			disabled: [false, false, true, false, false],
			allBets: [],
			folderSelected: -1,
			alertState: null,
			alertText: "",
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
			betFolders: [],
			overviewItems: []
		};

		this.onLoad = this.onLoad.bind(this);
		this.getAllFinishedBets = this.getAllFinishedBets.bind(this);
		this.dismissAlert = this.dismissAlert.bind(this);
		this.renderDropdown = this.renderDropdown.bind(this);
		this.renderTable = this.renderTable.bind(this);
		this.showFromFolder = this.showFromFolder.bind(this);
		this.updateTable = this.updateTable.bind(this);
		this.renderOverviewTable = this.renderOverviewTable.bind(this);
		this.calculateOverviewValues = this.calculateOverviewValues.bind(this);
		this.handleGetFolders = this.handleGetFolders.bind(this);
		this.handleGetBetsFromFolder = this.handleGetBetsFromFolder.bind(this);
		this.handleGetAllFinishedBets = this.handleGetAllFinishedBets.bind(this);
	}

	render(){
		var menuItems = this.renderDropdown();
		var table = this.renderTable();
		var overview = this.renderOverviewTable();

		return(
			<div className="content" onLoad={this.onLoad}>
				<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
				<Menu disable={this.state.disabled}></Menu>
				<Info alertState={this.state.alertState} alertText={this.state.alertText} dismiss={this.dismissAlert}></Info>
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

	renderOverviewTable(){
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

	renderTable(){
		var title = "Overview";

		if (this.state.folderSelected !== -1)
			title = this.state.betFolders[this.state.folderSelected]["name"];

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

	renderDropdown(){
		var menuItems = [];
		menuItems.push(<MenuItem onClick={this.showFromFolder.bind(this, -1)} key={-1} active={this.state.folderSelected === -1} eventKey={-1}>{"Overview"}</MenuItem>);
		var active = false;
		for (var k = 0; k < this.state.betFolders.length; k++){
			active = false;
			if (k === this.state.folderSelected)
				active = true;
			menuItems.push(<MenuItem onClick={this.showFromFolder.bind(this, k)} key={k} active={active} eventKey={k}>{this.state.betFolders[k]["name"]}</MenuItem>);
		}
		return menuItems;
	}

	showFromFolder(key){
		this.setState({
			folderSelected: key
		}, () => {this.updateTable()});
	}

	//Calculates the values that are used in the overview table. Function is performed after a bet folder has been received.
	calculateOverviewValues(betFolder){
		var name = betFolder.name;
		var moneyReturned = Stats.roundByTwo(Stats.moneyReturned(betFolder.bets));
		var verifiedReturn = Stats.roundByTwo(Stats.verifiedReturn(betFolder.bets));
		var winPercentage = Stats.roundByTwo(Stats.winPercentage(betFolder.bets));

		var overviewItems = this.state.overviewItems;
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

	updateTable(){
		var moneyWon, moneyPlayed, moneyReturned, wonBets, playedBets, winPercentage, avgReturn, expectedReturn, verifiedReturn, oddMedian, oddMean,
		betMedian, betMean;
		var param;

		if (this.state.folderSelected === -1)
			param = this.state.allBets;

		else
			param = this.state.betFolders[this.state.folderSelected]["bets"];

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

	sort(func, param){
		var sorted = func(this.state.overviewItems, param);

		this.setState({
			overviewItems: sorted
		});
	}

	dismissAlert(){
		this.setState({
			alertState: null,
			alertText: ""
		});
	}

	onLoad(){
		this.getAllFinishedBets();
		getFolders(this.handleGetFolders);
	}

	/*
	Callback for GET-folders request.
	For every folder, get-request is made to get bets
	in that folder.
	*/
	handleGetFolders(status, folders){
		folders = JSON.parse(folders);
		if (status === 200){
			for (var i = 0; i < folders.length; i++){ /* Loops through folderNames array to get bets for each of the folders.*/
				getBetsFromFolder(folders[i], this.handleGetBetsFromFolder);
			}
		}
		else if (status === 401){
			this.setState({
				alertState: status,
				alertText: "Session expired, please login again"
			});
		}
	}

	//gets a list of users bets that have finished. On receiving data, adds data to overviewItems.
	getAllFinishedBets(){
		getFinishedBets(this.handleGetAllFinishedBets);
	}

	handleGetAllFinishedBets(status, data){
		if (status === 200){
			this.setState({
				allBets: JSON.parse(data)
			},() => {
				this.updateTable();
				this.calculateOverviewValues({name: "", bets: this.state.allBets});
			});
		}
		else if (status === 401){
			this.setState({
				alertState: status,
				alertText: "Session expired, please login again"
			});
		}
	}

	/*
	Callback function, done after receiving bets from a specific folder.
	After data has been received, pushes an object with bets array and
	name of the folder into betFolders array.
	*/
	handleGetBetsFromFolder(status, data, folderName){
		if (status === 200){
			var betFolders = this.state.betFolders;
			var bets = JSON.parse(data);
			var item = {
				name: folderName,
				bets: bets
			}
			betFolders.push(item);
			this.calculateOverviewValues(item);
			this.setState({
				betFolders: betFolders
			});
		}
		else if (status === 401){
			this.setState({
				alertState: status,
				alertText: "Session expired, please login again"
			});
		}
	}
}

export default Statistics;

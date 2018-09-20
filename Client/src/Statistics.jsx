import React, { Component } from 'react';
import logo from './icon.svg';
import './css/App.css';
import Menu from './Menu.jsx';
import MenuItem from 'react-bootstrap/lib/MenuItem';
import Table from 'react-bootstrap/lib/Table';
import Button from 'react-bootstrap/lib/Button';
import Alert from 'react-bootstrap/lib/Alert';
import DropdownButton from 'react-bootstrap/lib/DropdownButton';
import Row from 'react-bootstrap/lib/Grid';
import Col from 'react-bootstrap/lib/Grid';
import * as Stats from './js/Stats.js';
import ConstVars from './js/Consts.js';

class Statistics extends Component{
	constructor(props){
		super(props);
		
		this.state = {
			disabled: [false, false, true, false, false],
			allBets: [],
			folderSelected: -1,
			alertState: null,
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
		this.getAllBets = this.getAllBets.bind(this);
		this.getFolders = this.getFolders.bind(this);
		this.dismissAlert = this.dismissAlert.bind(this);
		this.renderDropdown = this.renderDropdown.bind(this);
		this.renderTable = this.renderTable.bind(this);
		this.showFromFolder = this.showFromFolder.bind(this);
		this.updateTable = this.updateTable.bind(this);
		this.renderOverviewTable = this.renderOverviewTable.bind(this);
		this.calculateOverviewValues = this.calculateOverviewValues.bind(this);
	}
	
	render(){
		var alert = this.getAlert();
		var menuItems = this.renderDropdown();
		var table = this.renderTable();
		var overview = this.renderOverviewTable();
		
		return(
			<div className="App" onLoad={this.onLoad}>
				<header className="App-header">
					<img src={logo} className="App-logo" alt="logo"/>
					<h1 className="App-title">{"Logged in as " + window.sessionStorage.getItem('loggedUser')}</h1>
				</header>
				<Menu disable={this.state.disabled}></Menu>
				<div>{alert}</div>
				<div className="dropDownDiv">
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
			alertState: null
		});
	}
	
	onLoad(){
		this.getAllBets();
		this.getFolders();
	}
	
	getAlert(){
		switch(this.state.alertState){
			case "Unauthorized":
				return(<Alert bsStyle="danger" onDismiss={this.dismissAddAlert}><p>{"Session expired, please login again"}</p>
						<Button onClick={this.dismissAlert}>{"Hide"}</Button></Alert>);
			default:
				return;
		}
	}
		
	getFolders(){
		var xmlHttp = new XMLHttpRequest();	
		
		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					console.log(xmlHttp.status);
					var folderNames = JSON.parse(xmlHttp.responseText);
					for (var i = 0; i < folderNames.length; i++){ /* Loops through folderNames array to get bets for each of the folders.*/
						this.getBets(folderNames[i]);
					}
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					console.log(xmlHttp.status);
					this.setState({
						alertState: "Unauthorized"
					});
				}	

        });
		xmlHttp.open("GET", ConstVars.URI + 'folders/');
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.send();
	}
	
	//gets a list of users bets that have finished. On receiving data, adds data to overviewItems.
	getAllBets(){
		var xmlHttp = new XMLHttpRequest();	
		
		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					console.log(xmlHttp.status);
					this.setState({
						allBets: JSON.parse(xmlHttp.responseText)
					},() => {
						this.updateTable();
						this.calculateOverviewValues({name: "", bets: this.state.allBets});
					});
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					console.log(xmlHttp.status);
					this.setState({
						alertState: "Unauthorized"
					});
				}	

        });
		xmlHttp.open("GET", ConstVars.URI + 'bets?finished=true');
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.send();
	}
	
	/*getBets is performed after folder names has been received in a loop for each folder name.
	after data has been received, pushes an object with bets array and name of the folder into betFolders array.*/
	getBets(folderName){	
		var xmlHttp = new XMLHttpRequest();	
		
		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					console.log(xmlHttp.status);
					var betFolders = this.state.betFolders;
					var bets = JSON.parse(xmlHttp.responseText);
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
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					console.log(xmlHttp.status);
					this.setState({
						alertState: "Unauthorized"
					});
				}	
		});
		xmlHttp.open("GET", ConstVars.URI + 'bets?folder=' + folderName + "&finished=true");
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
		xmlHttp.send();
	}
}

export default Statistics;
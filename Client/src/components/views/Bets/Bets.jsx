import React, { Component } from 'react';
import Tab from 'react-bootstrap/lib/Tab';
import Tabs from 'react-bootstrap/lib/Tabs';
import AddBets from './AddBets/AddBets.jsx';
import DeleteBets from './DeleteBets/DeleteBets.jsx';
import Header from '../../Header/Header.jsx';
import Info from '../../Info/Info.jsx';
import Menu from '../../Menu/Menu.jsx';
import ConstVars from '../../../js/Consts.js';
import './Bets.css';

class Bets extends Component{
	constructor(props){
		super(props);

		this.state = {
			menuDisabled: [false, true, false, false, false],
			deleteBetsList: [],
			folders: [],
			unresolvedBets: [],
			alertState: null,
			alertText: ""
		};

		this.getAllBets = this.getAllBets.bind(this);
		this.getFolders = this.getFolders.bind(this);
		this.getBets = this.getBets.bind(this);
		this.getUnresolvedBets = this.getUnresolvedBets.bind(this);
		this.updateData = this.updateData.bind(this);
		this.dismissAlert = this.dismissAlert.bind(this);
	}

	render(){
		return(
		<div className="content" onLoad={this.updateData}>
			<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
			<Menu disable={this.state.menuDisabled}></Menu>
			<Info alertState={this.state.alertState} alertText={this.state.alertText} dismiss={this.dismissAlert}></Info>
			<Tabs defaultActiveKey={1} id="bet-tab">
				<Tab eventKey={1} title={"Add bets"}>
					<AddBets folders={this.state.folders} bets={this.state.unresolvedBets} onUpdate={this.updateData}></AddBets>
				</Tab>
				<Tab eventKey={2} title={"Delete bets"}>
					<DeleteBets folders={this.state.folders} bets={this.state.deleteBetsList} onUpdate={this.updateData}></DeleteBets>
				</Tab>
			</Tabs>
		</div>);
	}

	dismissAlert(){
		this.setState({
			alertState: null,
			alertText: ""
		});
	}

	//updates data. Gets bets, folders and unresolved bets from the api. If folder parameter is not specified, gets all users bets, otherwise
	//gets bets in that folder.
	updateData(folder){
		if (typeof folder === "string"){
			this.getBets(folder);
		}
		else {
			this.getAllBets();
		}
		this.getUnresolvedBets();
		this.getFolders();
	}

	//gets all bets, and sets allBets state variable accordingly.
	getAllBets(){
		var xmlHttp = new XMLHttpRequest();

		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					console.log(xmlHttp.status);
					this.setState({
						deleteBetsList: JSON.parse(xmlHttp.responseText)
					});
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					this.setState({
						alertState: xmlHttp.status,
						alertText: "Authorization failed, please login again"
					});
					console.log(xmlHttp.status);
				}

        });
		xmlHttp.open("GET", ConstVars.URI + "bets/");
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.send();
	}

	//gets bets from selected folder and changes bets state variable accordingly.
	getBets(folder){
		var xmlHttp = new XMLHttpRequest();

		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					console.log(xmlHttp.status);
					this.setState({
						deleteBetsList: JSON.parse(xmlHttp.responseText)
					});
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					this.setState({
						alertState: xmlHttp.status,
						alertText: "Authorization failed, please login again"
					});
					console.log(xmlHttp.status);
				}

        });
		xmlHttp.open("GET", ConstVars.URI + "bets?folder=" + folder);
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.send();
	}

	// gets a list of unresolved bets and sets the unresolvedBets list.
	getUnresolvedBets(){
		var xmlHttp = new XMLHttpRequest();

		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					console.log(xmlHttp.status);
					this.setState({
						unresolvedBets: JSON.parse(xmlHttp.responseText)
					});
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					this.setState({
						alertState: xmlHttp.status,
						alertText: "Authorization failed, please login again"
					});
					console.log(xmlHttp.status);
				}

        });
		xmlHttp.open("GET", ConstVars.URI + "bets?finished=false");
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.send();
	}

	getFolders(){
		var xmlHttp = new XMLHttpRequest();

		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					console.log(xmlHttp.status);
					this.setState({
						folders: JSON.parse(xmlHttp.responseText)
					});
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					this.setState({
						alertState: xmlHttp.status,
						alertText: "Authorization failed, please login again"
					});
					console.log(xmlHttp.status);
				}

        });
		xmlHttp.open("GET", ConstVars.URI + "folders/");
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.send();
	}
}

export default Bets;

import React, { Component } from 'react';
import logo from './icon.svg';
import './css/App.css';
import Menu from './Menu.jsx';
import Tabs from 'react-bootstrap/lib/Tabs';
import Tab from 'react-bootstrap/lib/Tab';
import AddBets from './AddBets.jsx';
import DeleteBets from './DeleteBets.jsx';
import ConstVars from './Consts.js';
import Alert from 'react-bootstrap/lib/Alert';
import Button from 'react-bootstrap/lib/Button';

class Bets extends Component{
	constructor(props){
		super(props);
		
		this.state = {
			menuDisabled: [false, true, false, false, false],
			deleteBetsList: [],
			folders: [],
			unresolvedBets: [],
			alertState: null
		};

		this.getAllBets = this.getAllBets.bind(this);
		this.getFolders = this.getFolders.bind(this);
		this.getBets = this.getBets.bind(this);
		this.getUnresolvedBets = this.getUnresolvedBets.bind(this);
		this.updateData = this.updateData.bind(this);
		this.renderAlert = this.renderAlert.bind(this);
		this.dismissAlert = this.dismissAlert.bind(this);
	}
	
	render(){
		var alert = this.renderAlert();
		
		return(
		<div className="App" onLoad={this.updateData}>
			<header className="App-header">
				<img src={logo} className="App-logo" alt="logo" />
				<h1 className="App-title">{"Logged in as " + window.sessionStorage.getItem('loggedUser')}</h1>
			</header>
			<Menu disable={this.state.menuDisabled}></Menu>
			<div>{alert}</div>
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
	
	renderAlert(){
		switch(this.state.alertState){

			case "Authorization":
				return (<Alert bsStyle="danger" onDismiss={this.dismissAlert}><p>{"Authorization failed, please login"}</p>
						<Button onClick={this.dismissAlert}>{"Hide"}</Button></Alert>);
				
			default: 
				return;
		}
	}
	
	dismissAlert(){
		this.setState({
			alertState: null
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
						alertState: "Authorization"
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
						alertState: "Authorization"
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
						alertState: "Authorization"
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
						alertState: "Authorization"
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
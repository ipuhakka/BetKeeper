import React, { Component } from 'react';
import './css/App.css';
import MenuItem from 'react-bootstrap/lib/MenuItem';
import Button from 'react-bootstrap/lib/Button';
import ControlLabel from 'react-bootstrap/lib/ControlLabel';
import ListGroup from 'react-bootstrap/lib/ListGroup';
import ListGroupItem from 'react-bootstrap/lib/ListGroupItem';
import Alert from 'react-bootstrap/lib/Alert';
import DropdownButton from 'react-bootstrap/lib/DropdownButton';
import ConstVars from './Consts.js';
import Row from 'react-bootstrap/lib/Grid';
import Col from 'react-bootstrap/lib/Grid';

class DeleteBets extends Component{
	constructor(props){
		super(props);
		console.log("delete bets");
		this.state = {
			bets: [],
			selectedBet: -1,
			allFolders: [],
			allFoldersSelected: -1,
			folders: [],
			selectedFolders: [],
			alertState: null
		};
		
		this.onLoad = this.onLoad.bind(this);
		this.setBets = this.setBets.bind(this);
		this.setBetsFolders = this.setBetsFolders.bind(this);
		this.getBetsFolders = this.getBetsFolders.bind(this);
		this.onPressedFolder = this.onPressedFolder.bind(this);
		this.deleteBet = this.deleteBet.bind(this);
		this.dismissAlert = this.dismissAlert.bind(this);
		this.getAlert = this.getAlert.bind(this);
		this.renderDropdown = this.renderDropdown.bind(this);
		this.renderFolderList = this.renderFolderList.bind(this);
		this.renderBetsList = this.renderBetsList.bind(this);
		this.showFromFolder = this.showFromFolder.bind(this);
	}
	
	componentDidMount(){
		this.onLoad();
	}
	
	render(){
		var	alertState = this.getAlert();
		
		var betItems = this.renderBetsList();	
		var folderItems = this.renderFolderList();		
		var menuItems = this.renderDropdown();
		
		return(
			<div className="App">
				<Row className="show-grid">
					<Col className="col-md-6 col-xs-12">
						<div className="dropDownList">
						<DropdownButton 
							bsStyle="info"
							title={"Show from folder"}
							id={1}>
							{menuItems}
						</DropdownButton>
						</div>
						<div className="list">
							<ListGroup>{betItems}</ListGroup>
						</div>
					</Col>
					<Col className="col-md-6 col-xs-12">
						<div className="list">
							<ControlLabel>{"Delete only from selected folders"}</ControlLabel>
							<ListGroup>{folderItems}</ListGroup>
						</div>
					</Col>
				</Row>
				<Row>
					<Col className="col-md-6 col-xs-12">
						<Button className="button" onClick={this.deleteBet} bsStyle="warning">Delete</Button>
						<div>{alertState}</div>
					</Col>
				</Row>
			</div>
		);
	}
	
	renderBetsList(){
		var betItems = [];
		var isSelected = false;
		for (var i = this.state.bets.length -1; i >= 0; i--){
			if (i === this.state.selectedBet || i.toString() === this.state.selectedBet)
				isSelected = true;
			else
				isSelected = false;
			
			var result = "Unresolved";
			if (this.state.bets[i].bet_won)
				result = "Won";
			else if (!this.state.bets[i].bet_won)
				result = "Lost";
			if (this.state.bets[i].bet_won === null || this.state.bets[i].bet_won.toString() === 'null')
				result = "Unresolved";
			betItems.push(<ListGroupItem onClick={this.onPressedBet.bind(this, i)} bsStyle={isSelected ?  'info': null} key={i} header={this.state.bets[i].name + " " + this.state.bets[i].datetime}>{"Odd: " + this.state.bets[i].odd + " Bet: " + this.state.bets[i].bet + " " + result}</ListGroupItem>)
		}
		return betItems;
	}
	
	renderDropdown(){
		var menuItems = [];
		menuItems.push(<MenuItem onClick={this.showFromFolder.bind(this, -1)} key={-1} active={this.state.allFoldersSelected === -1} eventKey={-1}>{"show all"}</MenuItem>);
		var active = false;
		if (this.state.allFolders.length > 0){
			for (var k = 0; k < this.state.allFolders.length; k++){
				active = false;
				if (k === this.state.allFoldersSelected || k.toString() === this.state.allFoldersSelected)
					active = true;
				menuItems.push(<MenuItem onClick={this.showFromFolder.bind(this, k)} key={k} active={active} eventKey={k}>{this.state.allFolders[k]}</MenuItem>);
			}
		}
		
		return menuItems;
	}
	
	renderFolderList(){
		var folderItems = [];
		for (var j = 0; j < this.state.folders.length; j++){
			folderItems.push(<ListGroupItem onClick={this.onPressedFolder.bind(this, j)} bsStyle={this.state.selectedFolders[j] ?  'info': null} key={j}>{this.state.folders[j]}</ListGroupItem>)
		}
		return folderItems;
	}
	
	getAlert(){
		switch(this.state.alertState){
			case "OK":
				return(<Alert bsStyle="success" onDismiss={this.dismissAlert}>
						<p>{"Deleted successfully"}</p>
						<Button onClick={this.dismissAlert}>{"Hide"}</Button>
						</Alert>);
						
			case "Not found":
				return (<Alert bsStyle="danger" onDismiss={this.dismissAlert}><p>{"Bet not found"}</p>
						<Button onClick={this.dismissAlert}>{"Hide"}</Button></Alert>);
				
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
	
	//Get bets from selected folder.
	showFromFolder(key){
		var xmlHttp = new XMLHttpRequest();
		
		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					console.log(xmlHttp.status);
					this.setBets(JSON.parse(xmlHttp.responseText));
					this.setState({
						folders: [],
						selectedFolders: []			
					});
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					console.log(xmlHttp.status);
				}		

        });
		var uri = ConstVars.URI + "bets?folder=";
		if (key !== '-1' && key !== -1)
			uri = uri + this.state.allFolders[key];
		xmlHttp.open("GET", uri);
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.send();
		
		this.setState({
			allFoldersSelected: key
		});
	}
	
	//Creates a request to delete the selected bet. If any folders are selected, bet is only deleted from selected folders.
	deleteBet(){
		if (this.state.selectedBet === -1 || this.state.selectedBet === '-1')
			return;
		
		var folders = [];
		for (var i = 0; i < this.state.folders.length; i++){
			if (this.state.selectedFolders[i])
				folders.push(this.state.folders[i]);
		}		
		var uri = ConstVars.URI + "bets/" + this.state.bets[this.state.selectedBet].bet_id;
		
		if (folders.length > 0){
			uri = uri + "?";
			for (var j = 0; j < folders.length; j++){
				uri = uri + "folders=" + folders[j];
				if (j < folders.length - 1)
					uri = uri + "&";
			}
		}
		var xmlHttp = new XMLHttpRequest();	
		
		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 204) {
					console.log(xmlHttp.status);
					this.setState({
						alertState: "OK"
					});
					this.onLoad(); ///get new bet list
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					console.log(xmlHttp.status);
					this.setState({
						alertState: "Authorization"
					});
				}	
				if (xmlHttp.readyState === 4 && xmlHttp.status === 404) {
					this.setState({
						alertState: "Not found"
					});
					console.log(xmlHttp.status);
				}	

        });
		xmlHttp.open("DELETE", uri);
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.send();
	}
	
	///set new selectedBet, if one is chosen get folders in which bet belongs to.
	onPressedBet(key){
		var value = -1;
		if (this.state.selectedBet !== key){ //set key and get folders.
			value = key;
			this.getBetsFolders(this.state.bets[key].bet_id);
		}
		else {
			this.setState({
				folders: [],
				selectedFolders: []			
			});
		}
		this.setState({
			selectedBet: value
		});
	}
	
	onPressedFolder(key){
		var selected = this.state.selectedFolders;		
		selected[key] = !selected[key];
		
		this.setState({
			selectedFolders: selected
		});
	}
	
	//Get all bets and folders.
	onLoad(){
		var xmlHttp = new XMLHttpRequest(); //bets
		
		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					console.log(xmlHttp.status);
					this.setBets(JSON.parse(xmlHttp.responseText));
					this.setState({
						folders: [],
						selectedFolders: []			
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
		
		var xmlHttp2 = new XMLHttpRequest(); //folders
		
		xmlHttp2.onreadystatechange =( () => {
				if (xmlHttp2.readyState === 4 && xmlHttp2.status === 200) {
					console.log(xmlHttp2.status);
					this.setState({
						allFolders: JSON.parse(xmlHttp2.responseText)	
					});
				}
				if (xmlHttp2.readyState === 4 && xmlHttp2.status === 401) {
					console.log(xmlHttp.status);
				}		

        });
		xmlHttp2.open("GET", ConstVars.URI + "folders/");
		xmlHttp2.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp2.send();
	}
	
	setBets(data){	
		this.setState({
			bets: data,
			selectedBet: -1
		});
	}
	
	getBetsFolders(id){
		var xmlHttp = new XMLHttpRequest();
		
		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					console.log(xmlHttp.status);
					this.setBetsFolders(JSON.parse(xmlHttp.responseText));
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					console.log(xmlHttp.status);
				}		

        });
		xmlHttp.open("GET", ConstVars.URI + "folders?bet_id=" + id);
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.send();
	}
	
	setBetsFolders(data){
		var sel = [];
		for (var i = 0; i < data.length; i++){
			sel.push(false);
		}
		
		this.setState({
			selectedFolders: sel,
			folders: data
		});
	}
}

export default DeleteBets;
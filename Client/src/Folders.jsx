import React, { Component } from 'react';
import logo from './icon.svg';
import './css/App.css';
import './css/folders.css';
import Menu from './Menu.jsx';
import Button from 'react-bootstrap/lib/Button';
import ListGroup from 'react-bootstrap/lib/ListGroup';
import ListGroupItem from 'react-bootstrap/lib/ListGroupItem';
import Row from 'react-bootstrap/lib/Grid';
import Col from 'react-bootstrap/lib/Grid';
import FormControl from 'react-bootstrap/lib/FormControl';
import Alert from 'react-bootstrap/lib/Alert';
import ConstVars from './Consts.js';

class Folders extends Component {
	constructor(props){
		super(props);
		
		this.state = {
			disabled: [false, false, false, true, false],
			folders: [],
			deleteDisabled: true,
			newFolder: "",
			deleteAlertState: null,
			addAlertState: null
		};
		
		this.onLoad = this.onLoad.bind(this);
		this.renderFoldersList = this.renderFoldersList.bind(this);
		this.deleteFolder = this.deleteFolder.bind(this);
		this.getDeleteAlertState = this.getDeleteAlertState.bind(this);
		this.getAddAlertState = this.getAddAlertState.bind(this);
		this.dismissDeleteAlert = this.dismissDeleteAlert.bind(this);
		this.dismissAddAlert = this.dismissAddAlert.bind(this);
		this.handleNewFolderChange = this.handleNewFolderChange.bind(this);
		this.addFolder = this.addFolder.bind(this);
	}
	
	render(){
		var folders = this.renderFoldersList();
		var deleteAlertState = this.getDeleteAlertState();
		var addAlertState = this.getAddAlertState();
		return(
			<div className="App" onLoad={this.onLoad}>
				<header className="App-header">
					<img src={logo} className="App-logo" alt="logo"/>
					<h1 className="App-title">{"Logged in as " + window.sessionStorage.getItem('loggedUser')}</h1>
				</header>
				<Menu disable={this.state.disabled}></Menu>
				<Row className="show-grid">
					<Col className="col-md-6 col-xs-12">
						<ListGroup>{folders}</ListGroup>
						<Button className="button" disabled={this.state.deleteDisabled} onClick={this.deleteFolder} bsStyle="warning">Delete</Button>
						<div>{deleteAlertState}</div>
					</Col>
					<Col className="col-md-6 col-xs-12">
						<FormControl 
							className="list margins"
							value={this.state.newFolder}
							onChange={this.handleNewFolderChange}
							placeholder="Add new folder"/>
						<Button onClick={this.addFolder} className="button" disabled={this.state.newFolder === ""} bsStyle="success">{"Create folder"}</Button>
						<div>{addAlertState}</div>
					</Col>
				</Row>
			</div>
		);
	}
	
	handleNewFolderChange(e){
		this.setState({
			newFolder: e.target.value
		});
	}
	
	renderFoldersList(){
		var items = [];
		for (var i = 0; i < this.state.folders.length; i++){
			items.push(<ListGroupItem onClick={this.clickedListItem.bind(this, i)} key={i} bsStyle={this.state.folders[i].selected ? 'info': null}>{this.state.folders[i].name}</ListGroupItem>)	
		}
		return items;
	}
	
	dismissAddAlert(){
		this.setState({
			addAlertState: null
		});
	}
	
	dismissDeleteAlert(){
		this.setState({
			deleteAlertState: null
		});
	}
	
	getAddAlertState(){
		switch(this.state.addAlertState){
			case "OK":
				return(<Alert bsStyle="success" onDismiss={this.dismissAddAlert}>
						<p>{"Created successfully"}</p>
						<Button onClick={this.dismissAddAlert}>{"Hide"}</Button>
						</Alert>);
			case "Bad Request":
				return (<Alert bsStyle="danger" onDismiss={this.dismissAddAlert}><p>{"Something went wrong with the request"}</p>
						<Button onClick={this.dismissAddAlert}>{"Hide"}</Button></Alert>);
				
			case "Unauthorized":
				return (<Alert bsStyle="danger" onDismiss={this.dismissAddAlert}><p>{"Authorization failed, please login again"}</p>
						<Button onClick={this.dismissAddAlert}>{"Hide"}</Button></Alert>);
			case "Conflict":
				return (<Alert bsStyle="danger" onDismiss={this.dismissAddAlert}><p>{"User already has folder of that name"}</p>
						<Button onClick={this.dismissAddAlert}>{"Hide"}</Button></Alert>);
			case "Content-Type":
				return (<Alert bsStyle="danger" onDismiss={this.dismissAddAlert}><p>{"Missing content-type header in request"}</p>
						<Button onClick={this.dismissAddAlert}>{"Hide"}</Button></Alert>);
			default:
				return;
		}
	}
	
	getDeleteAlertState(){
		switch(this.state.deleteAlertState){
			case "OK":
				return(<Alert bsStyle="success" onDismiss={this.dismissDeleteAlert}>
						<p>{"Deleted successfully"}</p>
						<Button onClick={this.dismissDeleteAlert}>{"Hide"}</Button>
						</Alert>);
			case "Bad request":
				return (<Alert bsStyle="danger" onDismiss={this.dismissDeleteAlert}><p>{"Something went wrong with the request"}</p>
						<Button onClick={this.dismissDeleteAlert}>{"Hide"}</Button></Alert>);
			case "Not found":
				return (<Alert bsStyle="danger" onDismiss={this.dismissDeleteAlert}><p>{"Bet not found"}</p>
						<Button onClick={this.dismissDeleteAlert}>{"Hide"}</Button></Alert>);
				
			case "Unauthorized":
				return (<Alert bsStyle="danger" onDismiss={this.dismissDeleteAlert}><p>{"Authorization failed, please login again"}</p>
						<Button onClick={this.dismissDeleteAlert}>{"Hide"}</Button></Alert>);
				
			default: 
				return;
		}
	}
	
	addFolder(){
		var data = {
			folder: this.state.newFolder
		};		
		var xmlHttp = new XMLHttpRequest();
		
		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 201) {
					console.log(xmlHttp.status);
					this.setState({
						addAlertState: "OK"
					});
					this.onLoad();
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 400) {
					console.log(xmlHttp.status);
					this.setState({
						addAlertState: "Bad request"
					});
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					this.setState({
						addAlertState: "Unauthorized"
					});
					console.log(xmlHttp.status);
				}	
				if (xmlHttp.readyState === 4 && xmlHttp.status === 409) {
					this.setState({
						addAlertState: "Conflict"
					});
					console.log(xmlHttp.status);
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 415) {
					this.setState({
						addAlertState: "Content-Type"
					});
					console.log(xmlHttp.status);
				}

        });
		console.log(ConstVars.URI + "folders");
		xmlHttp.open("POST", ConstVars.URI + "folders");
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.send(JSON.stringify(data));
	}
	
	deleteFolder(){
		var folder = null;
		for (var i = 0; i < this.state.folders.length; i++){
			if (this.state.folders[i].selected)
				folder = this.state.folders[i].name;
		}
		
		if (folder === null){
			console.log("no folder selected");
			return;
		}

		var xmlHttp = new XMLHttpRequest();
		
		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 204) {
					console.log(xmlHttp.status);
					this.setState({
						deleteAlertState: "OK"
					});
					this.onLoad();
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 400) {
					console.log(xmlHttp.status);
					this.setState({
						deleteAlertState: "Bad request"
					});
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					this.setState({
						deleteAlertState: "Unauthorized"
					});
					console.log(xmlHttp.status);
				}	
				if (xmlHttp.readyState === 4 && xmlHttp.status === 404) {
					this.setState({
						deleteAlertState: "Not found"
					});
					console.log(xmlHttp.status);
				}	

        });
		console.log(ConstVars.URI + "folders?folder=" + folder);
		xmlHttp.open("DELETE", ConstVars.URI + "folders?folder=" + folder);
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.send();
	}
	
	clickedListItem(key){
		var folders = this.state.folders;	
		var anySelected = false;
		for (var i = 0; i < folders.length; i++){
			if (i !== key)
				folders[i].selected = false;
		}
		folders[key].selected = !folders[key].selected;
		
		if (!folders[key].selected)
			anySelected = true;
		
		this.setState({
			folders: folders,
			deleteDisabled: anySelected
		});
	}

	//get folders and set the state.
	onLoad(){
		var xmlHttp = new XMLHttpRequest();
		
		xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					console.log(xmlHttp.status);
					this.setFolders(JSON.parse(xmlHttp.responseText));
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					console.log(xmlHttp.status);
				}		

        });
		xmlHttp.open("GET", ConstVars.URI + "folders/");
		xmlHttp.setRequestHeader('Authorization', sessionStorage.getItem('token'));
        xmlHttp.send();
	}
	
	setFolders(data){
		var folders = [];
		for (var i = 0; i < data.length; i++){
			folders.push({
				selected: false,
				name: data[i]
			});
		}
		
		this.setState({
			folders: folders
		});
	}
	
}

export default Folders;
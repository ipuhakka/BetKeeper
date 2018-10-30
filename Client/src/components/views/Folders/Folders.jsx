import React, { Component } from 'react';
import Button from 'react-bootstrap/lib/Button';
import ListGroup from 'react-bootstrap/lib/ListGroup';
import ListGroupItem from 'react-bootstrap/lib/ListGroupItem';
import Row from 'react-bootstrap/lib/Grid';
import Col from 'react-bootstrap/lib/Grid';
import FormControl from 'react-bootstrap/lib/FormControl';
import Info from '../../Info/Info.jsx';
import Header from '../../Header/Header.jsx';
import Menu from '../../Menu/Menu.jsx';
import ConstVars from '../../../js/Consts.js';
import './Folders.css';

class Folders extends Component {
	constructor(props){
		super(props);

		this.state = {
			disabled: [false, false, false, true, false],
			folders: [],
			deleteDisabled: true,
			newFolder: "",
			alertState: null,
			alertText: ""
		};

		this.onLoad = this.onLoad.bind(this);
		this.renderFoldersList = this.renderFoldersList.bind(this);
		this.deleteFolder = this.deleteFolder.bind(this);
		this.dismissAlert = this.dismissAlert.bind(this);
		this.handleNewFolderChange = this.handleNewFolderChange.bind(this);
		this.addFolder = this.addFolder.bind(this);
	}

	render(){
		var folders = this.renderFoldersList();
		return(
			<div className="content" onLoad={this.onLoad}>
				<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
				<Menu disable={this.state.disabled}></Menu>
				<Info alertState={this.state.alertState} alertText={this.state.alertText} dismiss={this.dismissAlert}></Info>
				<Row className="show-grid">
					<Col className="col-md-6 col-xs-12">
						<ListGroup>{folders}</ListGroup>
						<Button className="button" disabled={this.state.deleteDisabled} onClick={this.deleteFolder} bsStyle="warning">Delete</Button>
					</Col>
					<Col className="col-md-6 col-xs-12">
						<FormControl
							className="list margins"
							value={this.state.newFolder}
							onChange={this.handleNewFolderChange}
							placeholder="Add new folder"/>
						<Button onClick={this.addFolder} className="button" disabled={this.state.newFolder === ""} bsStyle="success">{"Create folder"}</Button>
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

	dismissAlert(){
		this.setState({
			alertState: null,
			alertText: ""
		});
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
						alertState: xmlHttp.status,
						alertText: "Folder added successfully"
					});
					this.onLoad();
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 400) {
					console.log(xmlHttp.status);
					this.setState({
						alertState: xmlHttp.status,
						alertText: "Something went wrong with the request"
					});
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					this.setState({
						alertState: xmlHttp.status,
						alertText: "Session expired, please login again"
					});
					console.log(xmlHttp.status);
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 409) {
					this.setState({
						alertState: xmlHttp.status,
						alertText: "Create failed: User already has a folder of same name"
					});
					console.log(xmlHttp.status);
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 415) {
					this.setState({
						alertState: xmlHttp.status,
						alertText: "Missing Content-Type header in request"
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
						alertState: xmlHttp.status,
						alertText: "Folder deleted successfully"
					});
					this.onLoad();
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 400) {
					console.log(xmlHttp.status);
					this.setState({
						alertState: xmlHttp.status,
						alertText: "Something went wrong with the request"
					});
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 401) {
					this.setState({
						alertState: xmlHttp.status,
						alertText: "Session expired, please login again"
					});
					console.log(xmlHttp.status);
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status === 404) {
					this.setState({
						alertState: xmlHttp.status,
						alertText: "Delete failed: folder trying to be deleted was not found"
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
					this.setState({
						alertState: xmlHttp.status,
						alertText: "Session expired, please login again"
					});
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
			folders: folders,
			deleteDisabled: true,
			newFolder: ""
		});
	}

}

export default Folders;

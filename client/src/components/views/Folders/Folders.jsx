import React, { Component } from 'react';
import Alert from 'react-bootstrap/lib/Alert';
import Button from 'react-bootstrap/lib/Button';
import ListGroup from 'react-bootstrap/lib/ListGroup';
import ListGroupItem from 'react-bootstrap/lib/ListGroupItem';
import Row from 'react-bootstrap/lib/Grid';
import Col from 'react-bootstrap/lib/Grid';
import FormControl from 'react-bootstrap/lib/FormControl';
import Info from '../../Info/Info.jsx';
import Header from '../../Header/Header.jsx';
import Menu from '../../Menu/Menu.jsx';
import {deleteFolder, postFolder, getFolders} from '../../../js/Requests/Folders.js';
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
		this.dismissAlert = this.dismissAlert.bind(this);
		this.handleNewFolderChange = this.handleNewFolderChange.bind(this);
		this.addFolder = this.addFolder.bind(this);
		this.deleteFolder = this.deleteFolder.bind(this);
		this.handleDelete = this.handleDelete.bind(this);
		this.handlePost = this.handlePost.bind(this);
		this.handleGet = this.handleGet.bind(this);
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
		if (items.length > 0){
			return items;
		} else {
			return <Alert>You don't have any folders: perhaps start by adding some?</Alert>;
		}
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

	dismissAlert(){
		this.setState({
			alertState: null,
			alertText: ""
		});
	}

	addFolder(){
		postFolder(this.state.newFolder, this.handlePost);
	}

	/* Callback function for posting a request.*/
	handlePost(status){
		var text;
		if (status === 201) {
				text = "Folder added successfully";
				this.onLoad();
		}
		if (status === 400) {
				text = "Something went wrong with the request";
		}
		if (status === 401) {
				text = "Session expired, please login again";
		}
		if (status === 409) {
				text = "Create failed: User already has a folder of same name";
		}

		this.setState({
			alertState: status,
			alertText: text
		});
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
		deleteFolder(folder, this.handleDelete);
	}

  /* Callback function for deleting a folder.*/
	handleDelete(status){
		var text;
		if (status === 204) {
				text = "Folder deleted successfully";
				this.onLoad();
		}
		if (status === 400) {
				text = "Something went wrong with the request";
		}
		if (status === 401) {
				text = "Session expired, please login again";
		}
		if (status === 404) {
				text = "Delete failed: folder trying to be deleted was not found";
		}

		this.setState({
			alertState: status,
			alertText: text
		});
	}

	//get folders and set the state.
	onLoad(){
		getFolders(this.handleGet);
	}

	///Callback function that handles state after receiving folders data from api.
	handleGet(status, data){
		if (status === 200) {
      this.setFolders(JSON.parse(data));
    }
    if (status === 401) {
      this.setState({
        alertState: status,
        alertText: "Session expired, please login again"
      });
    }
	}
}

export default Folders;

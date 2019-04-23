import React, { Component } from 'react';
import { connect } from 'react-redux';
import store from '../../store';
import {fetchFolders} from '../../actions/foldersActions';
import Alert from 'react-bootstrap/lib/Alert';
import Button from 'react-bootstrap/lib/Button';
import ListGroup from 'react-bootstrap/lib/ListGroup';
import ListGroupItem from 'react-bootstrap/lib/ListGroupItem';
import Row from 'react-bootstrap/lib/Grid';
import Col from 'react-bootstrap/lib/Grid';
import FormControl from 'react-bootstrap/lib/FormControl';
import Confirm from '../../components/Confirm/Confirm.jsx';
import Info from '../../components/Info/Info.jsx';
import Header from '../../components/Header/Header.jsx';
import Menu from '../../components/Menu/Menu.jsx';
import {isValidString} from '../../js/utils.js';
import './Folders.css';

class Folders extends Component {
	constructor(props){
		super(props);

		this.state = {
			disabled: [false, false, false, true, false],
			folders: [],
			deleteDisabled: true,
			newFolder: "",
			showConfirm: false
		};
	}

	componentWillReceiveProps(nextProps){
		if (nextProps.folders.length !== this.state.folders.length){
			this.setFolders(nextProps.folders);
		}
	}

	render(){
		var folders = this.renderFoldersList();
		return(
			<div className="content" onLoad={this.onLoad}>
				<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
				<Menu disable={this.state.disabled}></Menu>
				<Confirm bsStyle="danger" headerText="Delete folder?" visible={this.state.showConfirm} confirmAction={this.deleteFolder} cancelAction={this.toggleConfirm}/>
				<Info></Info>
				<Row className="show-grid">
					<Col className="col-md-6 col-xs-12">
						<ListGroup>{folders}</ListGroup>
						<Button className="button" disabled={this.state.deleteDisabled} onClick={this.toggleConfirm} bsStyle="warning">Delete</Button>
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

	handleNewFolderChange = (e) => {
		this.setState({
			newFolder: e.target.value
		});
	}

	renderFoldersList = () => {
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

	clickedListItem = (key) => {
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

	setFolders = (data) => {
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

	addFolder = () => {
		if (!isValidString(this.state.newFolder)){
			store.dispatch({type: 'SET_ALERT_STATUS',
				status: -1,
				message: "Name contains invalid characters"
			});

			return;
		}

		store.dispatch({type: 'POST_FOLDER', payload: {
      newFolderName: this.state.newFolder
    	}
  	});
	}

	deleteFolder = () => {
		this.toggleConfirm();

		var folder = null;
		for (var i = 0; i < this.state.folders.length; i++){
			if (this.state.folders[i].selected)
				folder = this.state.folders[i].name;
		}
		if (folder === null){
			console.log("no folder selected");
			return;
		}
		store.dispatch({type: 'DELETE_FOLDER', payload: {
      folderToDelete: folder
    	}
  	});
	}

	toggleConfirm = () => {
		this.setState({
			showConfirm: !this.state.showConfirm
		});
	}
	//get folders.
	onLoad = () => {
		this.props.fetchFolders();
	}
}

const mapStateToProps = (state, ownProps) => {
  return { ...state.folders}
};

const mapDispatchToProps = (dispatch) => ({
  fetchFolders: () => dispatch(fetchFolders())
});

export default connect(mapStateToProps, mapDispatchToProps)(Folders);

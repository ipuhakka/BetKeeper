import React, { Component } from 'react';
import MenuItem from 'react-bootstrap/lib/MenuItem';
import Button from 'react-bootstrap/lib/Button';
import ControlLabel from 'react-bootstrap/lib/ControlLabel';
import ListGroup from 'react-bootstrap/lib/ListGroup';
import ListGroupItem from 'react-bootstrap/lib/ListGroupItem';
import DropdownButton from 'react-bootstrap/lib/DropdownButton';
import Row from 'react-bootstrap/lib/Grid';
import Col from 'react-bootstrap/lib/Grid';
import Info from '../../../Info/Info.jsx';
import {getFoldersOfBet} from '../../../../js/Requests/Folders.js';
import {deleteBet} from '../../../../js/Requests/Bets.js';
import './DeleteBets.css';

class DeleteBets extends Component{
	constructor(props){
		super(props);

		this.state = {
			selectedBet: -1,
			allFoldersSelected: -1,
			folders: [], //components own state variable folders[] contains folders for the selected bet.
			selectedFolders: [], //contains folders for selected bet which have been selected.
			alertState: null,
			alertText: ""
		};
		this.setBetsFolders = this.setBetsFolders.bind(this);
		this.getBetsFolders = this.getBetsFolders.bind(this);
		this.onPressedFolder = this.onPressedFolder.bind(this);
		this.deleteBet = this.deleteBet.bind(this);
		this.handleDeletedBet = this.handleDeletedBet.bind(this);
		this.handleGetBetsFolders = this.handleGetBetsFolders.bind(this);
		this.dismissAlert = this.dismissAlert.bind(this);
		this.renderDropdown = this.renderDropdown.bind(this);
		this.renderFolderList = this.renderFolderList.bind(this);
		this.renderBetsList = this.renderBetsList.bind(this);
		this.showFromFolder = this.showFromFolder.bind(this);
	}

	render(){
		var betItems = this.renderBetsList();
		var folderItems = this.renderFolderList();
		var menuItems = this.renderDropdown();

		return(
			<div className="content">
				<Info alertState={this.state.alertState} alertText={this.state.alertText} dismiss={this.dismissAlert}></Info>
				<Row className="show-grid">
					<Col className="col-md-6 col-xs-12">
						<div>
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
							<ControlLabel className={this.state.selectedFolders.length !== 0 ? null:'hide'}>{"Delete only from selected folders"}</ControlLabel>
							<ListGroup>{folderItems}</ListGroup>
						</div>
						<Button disabled={this.state.selectedBet === -1} className="button" onClick={this.deleteBet} bsStyle="warning">Delete</Button>
					</Col>
				</Row>
			</div>
		);
	}

	renderBetsList(){
		var betItems = [];
		var isSelected = false;
		for (var i = this.props.bets.length -1; i >= 0; i--){
			if (i === this.state.selectedBet || i.toString() === this.state.selectedBet)
				isSelected = true;
			else
				isSelected = false;

			var result = "Unresolved";
			if (this.props.bets[i].bet_won)
				result = "Won";
			else if (!this.props.bets[i].bet_won)
				result = "Lost";
			if (this.props.bets[i].bet_won === null || this.props.bets[i].bet_won.toString() === 'null')
				result = "Unresolved";
			betItems.push(<ListGroupItem onClick={this.onPressedBet.bind(this, i)} bsStyle={isSelected ?  'info': null} key={i} header={this.props.bets[i].name + " " + this.props.bets[i].datetime}>{"Odd: " + this.props.bets[i].odd + " Bet: " + this.props.bets[i].bet + " " + result}</ListGroupItem>)
		}
		return betItems;
	}

	renderDropdown(){
		var menuItems = [];
		menuItems.push(<MenuItem onClick={this.showFromFolder.bind(this, -1)} key={-1} active={this.state.allFoldersSelected === -1} eventKey={-1}>{"show all"}</MenuItem>);
		var active = false;
		for (var k = 0; k < this.props.folders.length; k++){
			active = false;
			if (k === this.state.allFoldersSelected || k.toString() === this.state.allFoldersSelected)
				active = true;
			menuItems.push(<MenuItem onClick={this.showFromFolder.bind(this, k)} key={k} active={active} eventKey={k}>{this.props.folders[k]}</MenuItem>);
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

	dismissAlert(){
		this.setState({
			alertState: null,
			alertText: ""
		});
	}

	//Get bets from selected folder.
	showFromFolder(key){
		this.setState({
			folders: [],
			selectedFolders: [],
			allFoldersSelected: key,
			selectedBet: -1
		});

		if (key !== '-1' && key !== -1)
			this.props.onUpdate(this.props.folders[key]);
		else
			this.props.onUpdate();

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
		deleteBet(this.props.bets[this.state.selectedBet].bet_id, folders, this.handleDeletedBet);
	}

	/*
	Callback function for delete bet request.
	*/
	handleDeletedBet(status, data){
		let text;

		if (status === 204) {
      this.setState({
        folders: [],
        selectedFolders: [],
        alertState: status,
        alertText: "Bet deleted successfully",
        selectedBet: -1
      });
      this.props.onUpdate();
			return;
    }

		if (status === 200){
			this.setState({
        folders: [],
        selectedFolders: [],
        alertState: status,
        alertText: "Bet deleted successfully",
        selectedBet: -1
      });
      this.props.onUpdate();
			text = "Deleted bet from folders: " + data;
		}

    else if (status === 401) {
			text = "Session expired, please login again";
    }
    else if (status === 404) {
      text = "Bet trying to be deleted was not found";
    }

		this.setState({
			alertState: status,
			alertText: text
		});
	}
	///set new selectedBet, if one is chosen get folders in which bet belongs to.
	onPressedBet(key){
		var value = -1;
		if (this.state.selectedBet !== key){ //set key and get folders.
			value = key;
			this.getBetsFolders(this.props.bets[key].bet_id);
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

	getBetsFolders(id){
		getFoldersOfBet(id, this.handleGetBetsFolders);
	}

	handleGetBetsFolders(status, data){
		if (status === 200) {
			this.setBetsFolders(JSON.parse(data));
		}
		else if (status === 401) {
			this.setState({
				alertState: status,
				alertText: "Session expired, please login again"
			});
		}
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

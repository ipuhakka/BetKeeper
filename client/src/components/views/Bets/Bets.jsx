import React, { Component } from 'react';
import Tab from 'react-bootstrap/lib/Tab';
import Tabs from 'react-bootstrap/lib/Tabs';
import AddBets from './AddBets/AddBets.jsx';
import DeleteBets from './DeleteBets/DeleteBets.jsx';
import Header from '../../Header/Header.jsx';
import Info from '../../Info/Info.jsx';
import Menu from '../../Menu/Menu.jsx';
import {getFolders} from '../../../js/Requests/Folders.js';
import {getUnresolvedBets, getAllBetsByUser, getBetsFromFolder} from '../../../js/Requests/Bets.js';
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

	dismissAlert = () => {
		this.setState({
			alertState: null,
			alertText: ""
		});
	}

	//updates data. Gets bets, folders and unresolved bets from the api. If folder parameter is not specified, gets all users bets, otherwise
	//gets bets in that folder.
	updateData = (folder) => {
		if (typeof folder === "string"){
			getBetsFromFolder(folder, this.handleGetBetsFromFolder);
		}
		else {
			getAllBetsByUser(this.handleGetAllBets);
		}
		getUnresolvedBets(this.handleUnresolvedBets);
		getFolders(this.handleGetFolders);
	}

	//gets all bets, and sets allBets state variable accordingly.
	handleGetAllBets = (status, data) => {
		if (status === 200){
			this.setState({
        deleteBetsList: JSON.parse(data)
      });
		}
		if (status === 401){
			this.setState({
        alertState: status,
        alertText: "Authorization failed, please login again"
      });
		}
	}

	//gets bets from selected folder and changes bets state variable accordingly.
	handleGetBetsFromFolder = (status, data) => {
		if (status === 200){
			this.setState({
				deleteBetsList: JSON.parse(data)
			});
		}
		else if (status === 401){
			this.setState({
				alertState: status,
				alertText: "Authorization failed, please login again"
			});
		}
	}

	handleUnresolvedBets = (status, data) => {
		if (status === 200) {
			this.setState({
				unresolvedBets: JSON.parse(data)
			});
		}
		else if (status === 401) {
			this.setState({
				alertState: status,
				alertText: "Authorization failed, please login again"
			});
	}
}
	handleGetFolders = (status, data) => {
		if (status === 200){
			this.setState({
				folders: JSON.parse(data)
			});
		}
		else if (status === 401){
			this.setState({
				alertState: status,
				alertText: "Authorization failed, please login again"
			});
		}
	}
}

export default Bets;

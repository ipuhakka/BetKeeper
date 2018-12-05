import React, { Component } from 'react';
import { connect } from 'react-redux';
import store from '../../store';
import {fetchBets} from '../../actions/betsActions';
import {fetchFolders} from '../../actions/foldersActions';
import {clearAlert} from '../../actions/alertActions';
import Tab from 'react-bootstrap/lib/Tab';
import Tabs from 'react-bootstrap/lib/Tabs';
import AddBets from './AddBets/AddBets.jsx';
import DeleteBets from './DeleteBets/DeleteBets.jsx';
import Header from '../../components/Header/Header.jsx';
import Info from '../../components/Info/Info.jsx';
import Menu from '../../components/Menu/Menu.jsx';
import {getUnresolvedBets} from '../../js/Requests/Bets.js';
import './Bets.css';

class Bets extends Component{
	constructor(props){
		super(props);

		this.state = {
			menuDisabled: [false, true, false, false, false],
			deleteListFromFolder: false, //Tells whether all bets are open in delete list, or bets from a folder.
			deleteBetsList: [],
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
					<AddBets folders={this.props.folders} bets={this.state.unresolvedBets} onUpdate={this.updateData}></AddBets>
				</Tab>
				<Tab eventKey={2} title={"Delete bets"}>
					<DeleteBets folders={this.props.folders} bets={this.state.deleteListFromFolder ? this.props.betsFromFolder : this.props.allBets} onUpdate={this.updateData}></DeleteBets>
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
			this.setState({
				deleteListFromFolder: true
			});
			store.dispatch({type: 'FETCH_BETS_FROM_FOLDER', payload: {
	      folder: folder
	    	}
	  	});
		}
		else {
			this.setState({
				deleteListFromFolder: false
			});
			this.props.fetchBets();
		}
		getUnresolvedBets(this.handleUnresolvedBets);
		this.props.fetchFolders();
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

}

const mapStateToProps = (state, ownProps) => {
  return { ...state.bets, ...state.alert, ...state.folders}
};

const mapDispatchToProps = (dispatch) => ({
  fetchBets: () => dispatch(fetchBets()),
	fetchFolders: () => dispatch(fetchFolders()),
	clearAlert: () => dispatch(clearAlert())
});

export default connect(mapStateToProps, mapDispatchToProps)(Bets);

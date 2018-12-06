import React, { Component } from 'react';
import { connect } from 'react-redux';
import store from '../../store';
import {fetchBets, fetchUnresolvedBets} from '../../actions/betsActions';
import {fetchFolders} from '../../actions/foldersActions';
import Tab from 'react-bootstrap/lib/Tab';
import Tabs from 'react-bootstrap/lib/Tabs';
import AddBets from './AddBets/AddBets.jsx';
import DeleteBets from './DeleteBets/DeleteBets.jsx';
import Header from '../../components/Header/Header.jsx';
import Info from '../../components/Info/Info.jsx';
import Menu from '../../components/Menu/Menu.jsx';
import './Bets.css';

class Bets extends Component{
	constructor(props){
		super(props);

		this.state = {
			menuDisabled: [false, true, false, false, false],
			deleteListFromFolder: false, //Tells whether all bets are open in delete list, or bets from a folder.
			alertState: null,
			alertText: ""
		};
	}

	render(){
		return(
		<div className="content" onLoad={this.updateData}>
			<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
			<Menu disable={this.state.menuDisabled}></Menu>
			<Info></Info>
			<Tabs defaultActiveKey={1} id="bet-tab">
				<Tab eventKey={1} title={"Add bets"}>
					<AddBets folders={this.props.folders} bets={this.props.unresolvedBets} onUpdate={this.updateData}></AddBets>
				</Tab>
				<Tab eventKey={2} title={"Delete bets"}>
					<DeleteBets folders={this.props.folders} bets={this.state.deleteListFromFolder ? this.props.betsFromFolder.bets : this.props.allBets} onUpdate={this.updateData}></DeleteBets>
				</Tab>
			</Tabs>
		</div>);
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
		this.props.fetchUnresolvedBets();
		this.props.fetchFolders();
	}

}

const mapStateToProps = (state, ownProps) => {
  return { ...state.bets, ...state.folders}
};

const mapDispatchToProps = (dispatch) => ({
  fetchBets: () => dispatch(fetchBets()),
	fetchUnresolvedBets: () => dispatch(fetchUnresolvedBets()),
	fetchFolders: () => dispatch(fetchFolders())
});

export default connect(mapStateToProps, mapDispatchToProps)(Bets);

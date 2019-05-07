import React, { Component } from 'react';
import { connect } from 'react-redux';
import store from '../../store';
import {fetchBets, fetchUnresolvedBets} from '../../actions/betsActions';
import {fetchFolders} from '../../actions/foldersActions';
import DropdownButton from 'react-bootstrap/DropdownButton';
import ListGroup from 'react-bootstrap/ListGroup';
import ListGroupItem from 'react-bootstrap/ListGroupItem';
import DropdownItem from 'react-bootstrap/DropdownItem';
import Row from 'react-bootstrap/Container';
import Col from 'react-bootstrap/Container';
import AddBet from '../../components/AddBet/AddBet.jsx';
import Bet from '../../components/Bet/Bet.jsx';
import Header from '../../components/Header/Header.jsx';
import Info from '../../components/Info/Info.jsx';
import Menu from '../../components/Menu/Menu.jsx';
import UnresolvedBets from '../../components/UnresolvedBets/UnresolvedBets.jsx';
import './Bets.css';

class Bets extends Component{
  constructor(props){
		super(props);

		this.state = {
			menuDisabled: [false, true, false, false, false],
    	deleteListFromFolder: false, //Tells whether all bets are open in delete list, or bets from a folder.
      selectedBet: -1,
      folders: [],
      selectedFolders: [],
      showModal: false
		};
	}

  render(){
    var betItems = this.renderBetsList();
    var menuItems = this.renderDropdown();
    var betView = this.renderBetView(); //displays either list of unresolved bets, or data of specific bet.

    return (
      <div className="content" onLoad={this.updateData}>
    		<Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
    		<Menu disable={this.state.menuDisabled}></Menu>
    		<Info></Info>
        <i className="fas fa-plus-circle fa-2x addButton" onClick={this.showModal}></i>
        <AddBet show={this.state.showModal} hide={this.hideModal} folders={this.props.folders}/>
        <Row className="show-grid">
          <Col className="col-md-6 col-xs-12 col-md-push-6">
            <div className="betView">
              {betView}
            </div>
          </Col>
          <Col className="col-md-6 col-xs-12 col-md-pull-6">
            <DropdownButton
              variant="primary"
              title={"Show from folder"}
              id={1}>
              {menuItems}
            </DropdownButton>
            <div className="betList">
              <ListGroup>{betItems}</ListGroup>
            </div>
          </Col>
        </Row>
      </div>);
  }

  renderBetView = () => {
    if (this.state.selectedBet !== -1)
    {
      let key = this.state.selectedBet;
      let bet = this.state.deleteListFromFolder ? this.props.betsFromFolder.bets[key] : this.props.allBets[key];
      return <Bet bet={bet} allFolders={this.props.folders} foldersOfBet={this.props.foldersOfBet} onDelete={this.betDeleted} updateFolders={this.getBetsFolders}></Bet>;
    }
    else {
      return this.props.unresolvedBets.length > 0 ?
        <UnresolvedBets bets={this.props.unresolvedBets}></UnresolvedBets> :
        null;
    }
  }

  renderBetsList = () => {
    let bets = this.state.deleteListFromFolder ? this.props.betsFromFolder.bets : this.props.allBets;
		var betItems = [];
		var isSelected = false;
		for (var i = bets.length -1; i >= 0; i--){
			if (i === this.state.selectedBet || i.toString() === this.state.selectedBet)
				isSelected = true;
			else
				isSelected = false;
			var result = "Unresolved";
			if (bets[i].bet_won)
				result = "Won";
			else if (!bets[i].bet_won)
				result = "Lost";
			if (bets[i].bet_won === null || bets[i].bet_won.toString() === 'null')
				result = "Unresolved";
			betItems.push(<ListGroupItem onClick={this.onPressedBet.bind(this, i)} variant={isSelected ?  'info': null} key={i} header={bets[i].name + " " + bets[i].datetime}>{"Odd: " + bets[i].odd + " Bet: " + bets[i].bet + " " + result}</ListGroupItem>)
		}
		return betItems;
	}

  renderFolderList = () => {
    var folderItems = [];
    for (var j = 0; j < this.props.foldersOfBet.length; j++){
      folderItems.push(<ListGroupItem onClick={this.onPressedFolder.bind(this, j)} variant={this.state.selectedFolders[j] ?  'info': null} key={j}>{this.state.folders[j]}</ListGroupItem>)
    }
    return folderItems;
  }

  renderDropdown = () => {
    var menuItems = [];
    menuItems.push(<DropdownItem onClick={this.showFromFolder.bind(this, -1)} key={-1}
      active={this.state.allFoldersSelected === -1} eventKey={-1}>{"show all"}</DropdownItem>);
    var active = false;
    for (var k = 0; k < this.props.folders.length; k++){
      active = false;
      if (k === this.state.allFoldersSelected || k.toString() === this.state.allFoldersSelected)
        active = true;
      menuItems.push(<DropdownItem onClick={this.showFromFolder.bind(this, k)} key={k} active={active}
        eventKey={k}>{this.props.folders[k]}</DropdownItem>);
    }

    return menuItems;
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

  //Get bets from selected folder.
	showFromFolder = (key) => {
		this.setState({
			folders: [],
			selectedFolders: [],
			allFoldersSelected: key,
			selectedBet: -1
		});

		if (key !== '-1' && key !== -1)
			this.updateData(this.props.folders[key]);
		else
			this.updateData();

	}

  showModal = () => {
    this.setState({
      showModal: true
    });
  }

  hideModal = () => {
    this.setState({
      showModal: false
    });
  }

  ///set new selectedBet, if one is chosen get folders in which bet belongs to.
	onPressedBet = (key) => {
    let bets = this.state.deleteListFromFolder ? this.props.betsFromFolder.bets : this.props.allBets;
		var value = -1;
		if (this.state.selectedBet !== key){ //set key and get folders.
			value = key;
			this.getBetsFolders(bets[key].bet_id);
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

  betDeleted = () => {
    this.setState({
      selectedBet: -1
    });
  };

  getBetsFolders = (id) => {
		store.dispatch({type: 'FETCH_FOLDERS_OF_BET', payload: {
				bet_id: id
			}
		});
	}

};

const mapStateToProps = (state, ownProps) => {
  return { ...state.bets, ...state.folders}
};

const mapDispatchToProps = (dispatch) => ({
  fetchBets: () => dispatch(fetchBets()),
	fetchUnresolvedBets: () => dispatch(fetchUnresolvedBets()),
	fetchFolders: () => dispatch(fetchFolders())
});

export default connect(mapStateToProps, mapDispatchToProps)(Bets);

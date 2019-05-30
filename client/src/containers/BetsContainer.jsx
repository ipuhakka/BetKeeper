import React, { Component } from 'react';
import { connect } from 'react-redux';
import store from '../store';
import {fetchBets, fetchUnresolvedBets} from '../actions/betsActions';
import {fetchFolders} from '../actions/foldersActions';
import Bets from '../views/Bets/Bets.jsx';

class BetsContainer extends Component {

  constructor(props){
		super(props);

		this.state = {
      selectedBet: -1,
      folders: [],
      selectedFolders: [],
    	deleteListFromFolder: false //Whether all bets are visible or bets from selected folder
		};
	}

  componentWillMount(){
    this.updateData();
  }

  render() {
    const {props, state} = this;

    return(<Bets {...state} {...props} onChange={this.updateData} getBetsFolders={this.getBetsFolders}
      onDeleteBet={this.betDeleted} onPressedBet={this.onPressedBet} onShowFromFolder={this.showFromFolder}/>);
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

  getBetsFolders = (id) => {
    store.dispatch({type: 'FETCH_FOLDERS_OF_BET', payload: {
        bet_id: id
      }
    });
  }

  //Get bets from selected folder.
  showFromFolder = (key) => {
    const {props} = this;

    this.setState({
      folders: [],
      selectedFolders: [],
      allFoldersSelected: key,
      selectedBet: -1
    });

    if (key !== '-1' && key !== -1)
      this.updateData(props.folders[key]);
    else
      this.updateData();
  }

  betDeleted = () => {
    this.setState({
      selectedBet: -1
    });
  };

  ///set new selectedBet, if one is chosen get folders in which bet belongs to.
	onPressedBet = (key) => {

    const {props, state} = this;

    let bets = props.deleteListFromFolder ? props.betsFromFolder.bets : props.allBets;

		var value = -1;

		if (state.selectedBet !== key){ //set key and get folders.
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

};

const mapStateToProps = (state) => {
  return { ...state.bets, ...state.folders}
};

const mapDispatchToProps = (dispatch) => ({
  fetchBets: () => dispatch(fetchBets()),
	fetchUnresolvedBets: () => dispatch(fetchUnresolvedBets()),
	fetchFolders: () => dispatch(fetchFolders())
});

export default connect(mapStateToProps, mapDispatchToProps)(BetsContainer);